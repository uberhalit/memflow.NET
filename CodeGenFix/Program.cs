using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.CodeAnalysis.Text;
using Microsoft.CodeAnalysis.CSharp;
using CppAst;

namespace CodeGenFix
{
    public class Program
    {
        /// <summary>
        /// Fixes all issues in an auto-generated memflowInterop.cs from ClangSharp v13.
        /// </summary>
        /// <param name="args"></param>
        /// <exception cref="FileNotFoundException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        public static void Main(string[] args) 
        {
            Console.Title = "CodeGenFix";

            // get interop file
            string filePath = Path.Combine(Path.Combine(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "..", "..", "..", "..", ".."), "memflow.NET", "memflow.NET"), "memflowInterop.cs");
            if (args.Length == 1)
                filePath = args[0];
            Console.WriteLine($"Trying to parse '{filePath}'");
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Generated memflow interop file not found!");

            // read all text
            string srcText = File.ReadAllText(filePath);
            List<string> newSrcTextLines = File.ReadAllLines(filePath).ToList();

            // find source errors by compiling
            using MemoryStream peStream = new MemoryStream();
            while (true)
            {
                // compile file
                peStream.SetLength(0);
                EmitResult compResult = CompileFromText(srcText, out SyntaxTree parsedSource).Emit(peStream);
                IEnumerable<SyntaxNode> parsedNodes = parsedSource.GetRoot().DescendantNodes();

                // check compilation success
                if (compResult.Success)
                    goto FixStructs;
                else
                {
                    // check if any errors
                    IEnumerable<Diagnostic> codeErrors = compResult.Diagnostics.Where(x => x.Severity == DiagnosticSeverity.Error);

                    // if we fixed all compilation errors fix all explicit structs
                    if (!codeErrors.Any())
                        goto FixStructs;

                    List<int> fixedLines = new List<int>();

                    // check each error
                    foreach (Diagnostic codeIssue in codeErrors)
                    {
                        //Console.WriteLine($"L{codeIssue.Location.GetLineSpan().StartLinePosition.Line}: ({codeIssue.Id}) {codeIssue.GetMessage()}");
                        string errMsg = codeIssue.GetMessage();
                        int errLineNo = codeIssue.Location.GetLineSpan().StartLinePosition.Line;
                        string errLine = newSrcTextLines[errLineNo];
                        switch (codeIssue.Id)
                        {
                            // The type or namespace name 'type/namespace' could not be found (are you missing a using directive or an assembly reference?)
                            // NativeTypeName and NativeTypeNameAttribute get inserted by ClangSharp but have to be manually defined
                            case "CS0246":
                                // find all usings directives
                                var Usings = parsedNodes.Where(x => x.Kind() == SyntaxKind.UsingDirective);

                                // find namespace line
                                var NameSpace = parsedNodes.First(x => x.Kind() == SyntaxKind.NamespaceDeclaration);
                                int line = NameSpace.GetLocation().GetLineSpan().StartLinePosition.Line + 2;

                                if (errMsg.Contains("NativeTypeNameAttribute") || errMsg.Contains("NativeTypeName"))
                                {
                                    // insert using directives needed for definition
                                    if (!Usings.Any(x => x.ToString().StartsWith("using System;")))
                                        newSrcTextLines.Insert(0, "using System;");
                                    if (!Usings.Any(x => x.ToString().StartsWith("using System.Diagnostics;")))
                                        newSrcTextLines.Insert(1, "using System.Diagnostics;");

                                    // insert NativeInheritanceAttribute definition
                                    List<string> def = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "SourceDefinitions", "NativeInheritanceAttribute.cx")).ToList();
                                    def.Add("");
                                    newSrcTextLines.InsertRange(line + 2, def);

                                    // insert NativeTypeNameAttribute definition
                                    def = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "SourceDefinitions", "NativeTypeNameAttribute.cx")).ToList();
                                    def.Add("");
                                    newSrcTextLines.InsertRange(line + 2, def);

                                    goto Recompile;
                                }
                                break;

                            // Type byte, sbyte, short, ushort, int, uint, long, or ulong expected
                            // ClangSharp uses nuint instead of uint
                            case "CS1008":
                                if (errLine.Contains("nuint"))
                                    newSrcTextLines[errLineNo] = errLine.Replace("nuint", "uint");
                                else
                                    goto ThrowError;
                                break;

                            // Invalid expression term 'character'
                            // Clangsharp breaks some regular '=' operators and generates '.operator=' directives
                            case "CS1001":
                            case "CS1002":
                            case "CS1513":
                                if (codeErrors.Any(x => x.Id == "CS1525"))
                                    break;
                                else
                                    goto ThrowError;
                            case "CS1525":
                                if (errLine.Contains(".operator="))
                                    newSrcTextLines[errLineNo] = errLine.Replace(".operator=", "=");
                                else
                                    goto ThrowError;
                                break;

                            // The operation in question is undefined on void pointers
                            // ClangSharp mistakenly interprets some struct pointers as void*
                            case "CS0242":
                                // don't fix line again as there can be multiple void* issues on the same line
                                if (fixedLines.Contains(errLineNo))
                                    break;

                                // fix casting issue
                                if (errLine.Contains("(self)->vtbl") && errLine.Contains("(&("))
                                {
                                    int i1 = errLine.IndexOf(")", errLine.IndexOf("(self)->vtbl") + 8);
                                    string newLine = errLine.Remove(i1, 1);
                                    newLine = newLine.Replace("(self)->", "self)->");
                                    newLine = newLine.Replace("(&(", "(&((");
                                    newSrcTextLines[errLineNo] = newLine;
                                }
                                else
                                    goto ThrowError;
                                fixedLines.Add(errLineNo);
                                break;

                            // Cannot implicitly convert type 'type' to 'type'
                            // ClangSharp converts some booleans to byte
                            case "CS0029":
                                if (errLine.Contains("bool __ret") && errLine.Contains(");"))
                                    newSrcTextLines[errLineNo] = errLine.Replace(");", ") != 0;");
                                else
                                    goto ThrowError;
                                break;

                            // Argument 'number' cannot convert from TypeA to TypeB
                            // From previous issue, some functions require booleans as byte again
                            // Additionally some ints require casting to uint
                            case "CS1503":
                                if (fixedLines.Contains(errLineNo))
                                    break;
                                if (errMsg.Contains("uint") && errLine.Contains("sizeof("))
                                    newSrcTextLines[errLineNo] = errLine.Replace("sizeof(", "(uint)sizeof(");
                                else if (errMsg.Contains("uint") && errLine.Contains("CopyBlock") && errLine.Contains("elem_size);"))
                                    newSrcTextLines[errLineNo] = errLine.Replace("elem_size);", "(uint)elem_size);");
                                else if (errMsg.Contains("uint") && errLine.Contains("CopyBlock") && errLine.Contains("iter->sz_elem);"))
                                    newSrcTextLines[errLineNo] = errLine.Replace("iter->sz_elem);", "(uint)iter->sz_elem);");
                                else if (errMsg.Contains("byte") && errLine.Contains(");"))
                                    newSrcTextLines[errLineNo] = errLine.Replace(");", " ? (byte)0x1 : (byte)0x0);");
                                else
                                    goto ThrowError;
                                fixedLines.Add(errLineNo);
                                break;

                            // The name 'identifier' does not exist in the current context
                            // realloc() needs to be replaced
                            case "CS0103":
                                if (errLine.Contains("realloc"))
                                    newSrcTextLines[errLineNo] = errLine.Replace("realloc", "NativeMemory.Realloc");
                                else
                                    goto ThrowError;
                                break;

                            // make sure we don't miss any errors
                            default:
                                goto ThrowError;
                        }
                        continue;

                    ThrowError:
                        throw new NotSupportedException($"Code contained an unknown error: \nL{errLineNo}: ({codeIssue.Id}) {errMsg}");
                    }
                Recompile:
                    srcText = string.Join('\n', newSrcTextLines);
                }

                continue;
            FixStructs:
                // replace lazy typed structs with explicit ones
                var Structs = parsedNodes.Where(x => x.Kind() == SyntaxKind.StructDeclaration);

                // ProcessInfo
                if (Structs.Any(x => x.ToString().Contains("struct ProcessInfo")))
                {
                    var structDef = Structs.First(x => x.ToString().Contains("struct ProcessInfo"));
                    int lineStart = structDef.GetLocation().GetLineSpan().StartLinePosition.Line;
                    int lineEnd = structDef.GetLocation().GetLineSpan().EndLinePosition.Line;

                    List<string> def = File.ReadAllLines(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "SourceDefinitions", "ProcessInfo.cx")).ToList();
                    newSrcTextLines.RemoveRange(lineStart, (lineEnd - lineStart) + 1);
                    newSrcTextLines.InsertRange(lineStart, def);
                }

                srcText = string.Join('\n', newSrcTextLines);

                // recompile to update line numbers
                peStream.SetLength(0);
                compResult = CompileFromText(srcText, out parsedSource).Emit(peStream);
                parsedNodes = parsedSource.GetRoot().DescendantNodes();

                // build index from line numbers to find new lines numbers after a comment got inserted
                int[] lineIndex = new int[newSrcTextLines.Count];
                for (int i = 0; i < lineIndex.Length; i++)
                    lineIndex[i] = i;

                // add all comments from original file
                CppCompilation parsedHeader = CppParser.ParseFile(Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "SourceDefinitions", "memflow.h"));

                // enums
                var Enums = parsedNodes.Where(x => x.Kind() == SyntaxKind.EnumDeclaration);
                foreach (CppEnum cDef in parsedHeader.Enums)
                {
                    if (cDef.TypeKind == CppTypeKind.Enum && cDef.Comment != null)
                    {
                        // find C# definition
                        SyntaxNode? csDef = Enums.FirstOrDefault(x => x.ChildTokens().FirstOrDefault(y => y.Kind() == SyntaxKind.IdentifierToken).Text.Equals(cDef.Name, StringComparison.OrdinalIgnoreCase));
                        InsertCppComment(csDef, cDef.Comment, ref lineIndex, ref newSrcTextLines);
                    }
                }

                // structs
                Structs = parsedNodes.Where(x => x.Kind() == SyntaxKind.StructDeclaration);
                foreach (CppClass cDef in parsedHeader.Classes)
                {
                    if (cDef.TypeKind == CppTypeKind.StructOrClass && cDef.Comment != null)
                    {
                        // find C# definition
                        SyntaxNode? csDef = Structs.FirstOrDefault(x => x.ChildTokens().FirstOrDefault(y => y.Kind() == SyntaxKind.IdentifierToken).Text.Equals(cDef.Name, StringComparison.OrdinalIgnoreCase));
                        InsertCppComment(csDef, cDef.Comment, ref lineIndex, ref newSrcTextLines);
                    }
                }

                // functions
                var Classes = parsedNodes.Where(x => x.Kind() == SyntaxKind.ClassDeclaration);
                var Functions = Classes.First(x => x.ChildTokens().FirstOrDefault(y => y.Kind() == SyntaxKind.IdentifierToken).Text.Equals("Methods", StringComparison.OrdinalIgnoreCase)).ChildNodes();
                foreach (CppFunction cDef in parsedHeader.Functions)
                {
                    // find C# field definition in Methods struct
                    SyntaxNode? csDef = Functions.FirstOrDefault(x => x.ChildTokens().FirstOrDefault(y => y.Kind() == SyntaxKind.IdentifierToken).Text.Equals(cDef.Name, StringComparison.OrdinalIgnoreCase));
                    InsertCppComment(csDef, cDef.Comment, ref lineIndex, ref newSrcTextLines, 2);
                }

                srcText = string.Join('\n', newSrcTextLines);
                break;
            }

            File.WriteAllText(filePath, srcText);
            Console.WriteLine("Finished...");
        }

        /// <summary>
        /// Inserts a C/C++ comment into its C# equivalent source text file.
        /// </summary>
        /// <param name="csDefinition">The C# definition of the C/C++ object.</param>
        /// <param name="cComment">The C/C++ commentary.</param>
        /// <param name="lineIndex">The line index that should be updated with new locations after comment got inserted.</param>
        /// <param name="newScrLines">The source lines in which the comment should be inserted into.</param>
        /// <param name="lineIntent">How many tab stops a comment should have before insertation.</param>
        private static void InsertCppComment(SyntaxNode? csDefinition, CppComment cComment, ref int[] lineIndex, ref List<string> newScrLines, int lineIntent = 1)
        {
            if (cComment == null || cComment.Children.Count < 1)
                return;
            if (csDefinition == null)
                return;

            int originalLine = csDefinition.GetLocation().GetLineSpan().StartLinePosition.Line;
            int lineStart = lineIndex[originalLine];

            // build comment
            List<string> comment = new();
            string intent = "";
            for (int i = 0; i < lineIntent; i++)
                intent += "    ";
            comment.Add($"{intent}/// <summary>");
            foreach (var rawCommLine in cComment.Children)
                comment.Add($"{intent}/// {rawCommLine.ChildrenToString()}");
            comment.Add($"{intent}/// </summary>");
            newScrLines.InsertRange(lineStart, comment);

            // update line index
            for (int i = originalLine; i < lineIndex.Length; i++)
                lineIndex[i] += comment.Count;
        }

        /// <summary>
        /// Compiles a C# text in memory.
        /// </summary>
        /// <param name="srcText">The full source code to compile.</param>
        /// <returns>The compile result.</returns>
        private static CSharpCompilation CompileFromText(string srcText, out SyntaxTree parsedSyntax)
        {
            // try to compile file
            SourceText codeString = SourceText.From(srcText);
            CSharpParseOptions options = CSharpParseOptions.Default.WithLanguageVersion(LanguageVersion.Latest);
            parsedSyntax = SyntaxFactory.ParseSyntaxTree(codeString, options);

            var references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location), "System.Runtime.InteropServices.dll")),
                MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location), "System.Runtime.CompilerServices.Unsafe.dll")),
                MetadataReference.CreateFromFile(Path.Combine(Path.GetDirectoryName(typeof(object).Assembly.Location), "System.Runtime.dll")),
                MetadataReference.CreateFromFile(typeof(object).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Console).Assembly.Location),
                MetadataReference.CreateFromFile(typeof(Microsoft.CSharp.RuntimeBinder.CSharpArgumentInfo).Assembly.Location),
            };
            return CSharpCompilation.Create("temp.dll",
                new[] { parsedSyntax },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary,
                    optimizationLevel: OptimizationLevel.Release,
                    allowUnsafe: true,
                    assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default));
        }
    }
}