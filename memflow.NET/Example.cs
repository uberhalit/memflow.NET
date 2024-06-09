using System.Text;
using memflowNET;

namespace memflowCS
{
    class Example
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Testing memflow.NET");

            // connect to VM
            MFconnection mfConnection = new MFconnection("kvm", loglevel: 2);
            if (!mfConnection.Success) 
            {
                Console.WriteLine("### Failed to initialize memflow connection!");
                mfConnection.Dispose();
                return;
            }

            // find process
            MFprocess mfProcess = new MFprocess(ref mfConnection, "notepad");
            if (!mfProcess.Success) 
            {
                Console.WriteLine("### Failed to initialize target process!");
                mfProcess.Dispose();
                mfConnection.Dispose();
                return;
            }

            // read process memory
            byte[] header = mfProcess.ReadBytes(mfProcess.MainModule.BaseAddress + 0x1000, 48);
            Console.WriteLine("### Module start: ");
            PrintBytes(header);

            // get relative offset to NotepadContext-Class pointer at .text base+0x19 or .text base+0xEA
            ulong pContext = mfProcess.DereferenceRelativeOffset(mfProcess.MainModule.BaseAddress + 0x1000 + 0x19);
            // then get pointer to TextManager at NotepadContext+0x1288
            ulong ppTextBuffer = mfProcess.Read<ulong>(pContext + 0x1288);
            // if first offset was wrong, try to get the second one
            if (!IsValidAddress(ppTextBuffer))
            {
                pContext = mfProcess.DereferenceRelativeOffset(mfProcess.MainModule.BaseAddress + 0x1000 + 0xEA);
                ppTextBuffer = mfProcess.Read<ulong>(pContext + 0x1288);
            }
            // finally get address of text buffer at TextManager+0x0
            if (IsValidAddress(ppTextBuffer))
            {
                ulong pTextBuffer = mfProcess.Read<ulong>(ppTextBuffer);
                if (IsValidAddress (pTextBuffer))
                {
                    // read first 5 letters from notepad text
                    string notepadText = mfProcess.ReadFixedString(pTextBuffer, 10, Encoding.Unicode);
                    Console.WriteLine($"### Notepad Text: {notepadText}");
                    // write a new string to notepad window
                    mfProcess.WriteString(pTextBuffer, "memflow FTW!", Encoding.Unicode);
                }
            }
            // Cleanup
            mfProcess.Dispose();
            mfConnection.Dispose();
        }

        /// <summary>
        /// Pretty prints a byte[] to console.
        /// </summary>
        /// <param name="data">The byte array to show.</param>
        private static void PrintBytes(byte[] data)
        {
            Console.WriteLine();
            Console.Write("\t");
            string[] blocks = BitConverter.ToString(data).Split("-");
            int i = 0;
            while (i < blocks.Length) 
            {
                string block = blocks[i];
                i++;
                if (i % 16 == 0)
                {
                    Console.WriteLine(block);
                    Console.Write("\t");
                }
                else if (i % 4 == 0)
                    Console.Write(block + " | ");
                else
                    Console.Write(block + " ");
            }
            if (i % 16 != 0)
                Console.WriteLine();
            Console.WriteLine();
        }

        /// <summary>
        /// Checks if an address is valid.
        /// </summary>
        /// <param name="address">The address (the pointer points to).</param>
        /// <returns>True if (pointer points to) a valid address.</returns>
        private static bool IsValidAddress(UInt64 address)
        {
            return (address >= 0x10000 && address < 0x000F000000000000);
        }
    }
}