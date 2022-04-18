using System.Text;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using memflowNET.Interop;

/// <summary>
/// Version 1.0.0.0
/// </summary>
namespace memflowNET
{
    /// <summary>
    /// A memflow connection to a VM.
    /// </summary>
    public unsafe class MFconnection: IDisposable
    {
        /// <summary>
        /// Determining whether this instance initialized successfully.
        /// </summary>
        public readonly bool Success;

        /// <summary>
        /// Internal cache for log level.
        /// </summary>
        internal readonly int _loglevel;

        /// <summary>
        /// A settable log function.
        /// </summary>
        internal Action<string> PrintLog { get; set; }

        /// <summary>
        /// Internal inventory.
        /// </summary>
        private readonly Inventory* _inventory;

        /// <summary>
        /// Internal connector.
        /// </summary>
        private readonly ConnectorInstance_CBox_c_void_____CArc_c_void* _connector;

        /// <summary>
        /// Internal OS plugin.
        /// </summary>
        internal readonly OsInstance_CBox_c_void_____CArc_c_void* _osPlugin;

        /// <summary>
        /// Connects to memflow using the provided connector.
        /// </summary>
        /// <param name="connector">A memflow connector, currently supported are 'kvm' and 'qemu_procfs'.</param>
        /// <param name="loglevel">The logging level of memflow.</param>
        /// <param name="logging">The logging action that takes a single string as parameter.</param>
        public unsafe MFconnection(string connector, int loglevel = 1, Action<string>? logging = null)
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) 
                throw new PlatformNotSupportedException("Only Linux systems are supported at this time.");
            if (!Environment.Is64BitProcess || IntPtr.Size != 8) 
                throw new PlatformNotSupportedException("Only 64 bit systems are supported.");

            // check for root privilege
            if (ShellHelper.Bash($"id -u {Environment.UserName}") != "0") 
                throw new UnauthorizedAccessException("Must be started as root.");

            // set logger
            if (logging != null)
                PrintLog = logging;
            else
                PrintLog = Logger;

            // enable debug level logging
            Interop.Methods.log_init((Interop.LevelFilter)loglevel);
            this._loglevel = loglevel;

            // load all available plugins
            this._inventory = Interop.Methods.inventory_scan();
            if (loglevel > 2)
                PrintLog($"### Inventory initialized: 0x{(IntPtr)this._inventory:X}");

            // alloc connector struct
            this._connector = (ConnectorInstance_CBox_c_void_____CArc_c_void*) Marshal.AllocCoTaskMem(sizeof(ConnectorInstance_CBox_c_void_____CArc_c_void));
            
            // initialize the connector
            sbyte[] bName = Array.ConvertAll(Encoding.ASCII.GetBytes(connector + "\0"), b => unchecked((sbyte)b));
            sbyte[] bArgs = new sbyte[1] {0x0}; // empty C string
            fixed (sbyte* cName = &bName[0], cArgs = &bArgs[0])
            {
                if (Interop.Methods.inventory_create_connector(this._inventory, cName, cArgs, this._connector) != 0)
                {
                    PrintLog($"### Unable to initialize connector '{connector}'");
                    return;
                }
            }
            if (loglevel > 2)
                PrintLog($"### Connector initialized: 0x{(IntPtr)(*this._connector).container.instance.instance:X}");

            // alloc OS Plugin struct
            this._osPlugin = (OsInstance_CBox_c_void_____CArc_c_void*) Marshal.AllocCoTaskMem(sizeof(OsInstance_CBox_c_void_____CArc_c_void));
            
            // initialize the OS plugin
            bName = Array.ConvertAll(Encoding.ASCII.GetBytes("win32\0"), b => unchecked((sbyte)b));
            fixed (sbyte* osName = &bName[0], osArgs = &bArgs[0])
            {
                if (Interop.Methods.inventory_create_os(this._inventory, osName, osArgs, this._connector, this._osPlugin) != 0) 
                {
                    PrintLog("### Unable to initialize OS plugin 'win32'");
                    return;
                }
            }
            if (loglevel > 2)
                PrintLog($"### OS plugin initialized: 0x{(IntPtr)(*this._osPlugin).container.instance.instance:X}");
            this.Success = true;
        }

        /// <summary>
        /// Clean up all objects.
        /// </summary>
        public void Dispose()
        {
            if (this._osPlugin != null)
            {
                // we don't need to drop connector as it was handed into osplugin
                Interop.Methods.os_drop(this._osPlugin);
                Marshal.FreeCoTaskMem((IntPtr)this._osPlugin);
                Marshal.FreeCoTaskMem((IntPtr)this._connector);
                if (this._loglevel > 2)
                    PrintLog("### OS plugin/Connector freed");
            }
            else if (this._connector != null)
            {
                Interop.Methods.connector_drop(this._connector);
                Marshal.FreeCoTaskMem((IntPtr)this._connector);
                if (this._loglevel > 2)
                    PrintLog("### Connector freed");
            }
            Interop.Methods.inventory_free(this._inventory);
            if (this._loglevel > 2)
                PrintLog("### Inventory freed");
        }

        /// <summary>
        /// Logs a message to console/output.
        /// </summary>
        /// <param name="msg">The message to log.</param>
        private static void Logger(string msg) 
        {
            Console.WriteLine(msg);
            Debug.WriteLine(msg);
        }
    }

    /// <summary>
    /// A memflow process module info.
    /// </summary>
    public readonly struct MFprocessmodule
    {
        /// <summary>
        /// The base address of the module.
        /// </summary>
        public readonly ulong BaseAddress;

        /// <summary>
        /// The total size of the module.
        /// </summary>
        public readonly uint Size;

        /// <summary>
        /// The name of the module.
        /// </summary>
        public readonly string Name;

        public MFprocessmodule(ulong baseAddr, uint size, string name)
        {
            this.BaseAddress = baseAddr;
            this.Size = size;
            this.Name = name;
        }
    }

    /// <summary>
    /// A memflow process instance.
    /// </summary>
    public unsafe class MFprocess: IDisposable
    {
        /// <summary>
        /// Determining whether this instance initialized successfully.
        /// </summary>
        public readonly bool Success;

        /// <summary>
        /// The base address of the process. On Windows this is the address to [_EPROCESS].
        /// </summary>
        public readonly ulong Address;

        /// <summary>
        /// The process' ID.
        /// </summary>
        public readonly uint PId;

        /// <summary>
        /// The process' name.
        /// </summary>
        public readonly string? Name;

        /// <summary>
        /// The full path to the process' executable.
        /// </summary>
        public readonly string? Path;

        /// <summary>
        /// The arguments the process has been started with.
        /// </summary>
        public readonly string? Commandline;

        /// <summary>
        /// The main module of the process.
        /// </summary>
        public readonly MFprocessmodule MainModule;

        /// <summary>
        /// Internal log action.
        /// </summary>
        private Action<string> PrintLog { get; set; }

        /// <summary>
        /// Internal cache for log level.
        /// </summary>
        private readonly int _loglevel;

        /// <summary>
        /// Internal process.
        /// </summary>
        private readonly ProcessInstance_CBox_c_void_____CArc_c_void* _process;

        /// <summary>
        /// Internal OS plugin from MFconnection.
        /// </summary>
        private readonly OsInstance_CBox_c_void_____CArc_c_void* _osPlugin;

        /// <summary>
        /// Pointer to a 4096 bytes allocated unmanaged memory region used to directly cache read/write results.
        /// </summary>
        private readonly byte* _rwBuffer;

        /// <summary>
        /// Gets a process inside the VM by name.
        /// </summary>
        /// <param name="connection">A fully initialized MFconnection.</param>
        /// <param name="processName">The processes name.</param>
        public unsafe MFprocess(ref MFconnection connection, string processName)
        {
            this._loglevel = connection._loglevel;
            this.PrintLog = connection.PrintLog;
            if (!connection.Success)
            {
                PrintLog("### MFconnection not valid!");
                return;
            }

            // alloc process struct
            this._process = (ProcessInstance_CBox_c_void_____CArc_c_void*) Marshal.AllocCoTaskMem(sizeof(ProcessInstance_CBox_c_void_____CArc_c_void));

            // define search
            string procName = processName;
            if (!procName.ToLower().EndsWith(".exe"))
                procName += ".exe";
            byte[] bName = Encoding.ASCII.GetBytes(procName);
            fixed (byte* cName = &bName[0]) 
            {
                CSliceRef_u8 processSearch = new()
                {
                    data = cName,
                    len = (uint)procName.Length
                };
                // find a specific process based on its name
                if (Interop.Methods.mf_osinstance_process_by_name(connection._osPlugin, processSearch, this._process) != 0)
                {
                    // in some rare cases dropping a failed process struct through memflow will trigger a memory exception so we we have to manually dispose it here for now
                    Marshal.FreeCoTaskMem((IntPtr)this._process);
                    this._process = null;
                    if (this._loglevel > 0)
                        PrintLog($"### Process '{procName}' could not be found!");
                    return;
                }
            }
            
            // get process infos
            ProcessInfo* info = Interop.Methods.mf_processinstance_info(this._process);
            this.Address = (*info).address;
            this.PId = (*info).pid;
            this.Name = GetCStringFromPtr((*info).name);
            this.Path = GetCStringFromPtr((*info).path);
            this.Commandline = GetCStringFromPtr((*info).command_line);
            
            // get main module
            ModuleInfo mainModule = new();
            if (Interop.Methods.mf_processinstance_primary_module(this._process, &mainModule) != 0)
            {
                if (this._loglevel > 0)
                    PrintLog("### Process main module could not be found!");
                return;
            }
            this.MainModule = new MFprocessmodule(mainModule.@base, (uint)mainModule.size, this.Name);
            if (this._loglevel > 2)
                PrintLog($"### Process found: 0x{this.MainModule.BaseAddress:X} | {this.MainModule.Size / 1024} kiB | {this.PId} | {this.Name} | {this.Path}");

            // allocate read/write buffer
            this._rwBuffer = (byte*)NativeMemory.AllocZeroed(4096);

            this._osPlugin = connection._osPlugin;
            this.Success = true;
        }

        /// <summary>
        /// Reads a null-terminated ASCII string from a pointer.
        /// </summary>
        /// <param name="ptr">The pointer.</param>
        /// <returns>The ASCII representation of the string the pointer points to.</returns>
        private string GetCStringFromPtr(sbyte* ptr)
        {
            //return Marshal.PtrToStringAnsi((IntPtr)ptr);
            for (int i = 0; i < 256; i++) // sanity
            {
                // find terminator
                if (*(ptr + i) == (byte)0x00)
                    return Encoding.ASCII.GetString((byte*)ptr, i);
            }
            return "";
        }

        /// <summary>
        /// Gets a process' module by name.
        /// </summary>
        /// <param name="name">The full name of the module.</param>
        /// <returns>The process' module.</returns>
        public MFprocessmodule GetModule(string name)
        {
            ModuleInfo moduleInfo = new();
            byte[] bName = Encoding.ASCII.GetBytes(name);
            fixed (byte* cName = &bName[0]) 
            {
                CSliceRef_u8 processSearch = new()
                {
                    data = cName,
                    len = (uint)name.Length
                };
                if (Interop.Methods.mf_processinstance_module_by_name(this._process, processSearch, &moduleInfo) != 0)
                {
                    if (this._loglevel > 0)
                        PrintLog($"### Module '{name}' could not be found!");
                    return new MFprocessmodule(0, 0, "INVALID");
                }
            }
            return new MFprocessmodule(moduleInfo.@base, (uint)moduleInfo.size, GetCStringFromPtr(moduleInfo.name));
        }

        /// <summary>
        /// If the process instance is up and running.
        /// </summary>
        /// <returns>True if process hasn't exited yet.</returns>
        public bool IsRunning()
        {
            ProcessState state = Interop.Methods.mf_processinstance_state(this._process);
            if (state.tag == ProcessState_Tag.ProcessState_Alive)
                return true;
            return false;
        }

        /// <summary>
        /// Reads a number of bytes from an address of the process.
        /// </summary>
        /// <param name="address">The virtual address to read from.</param>
        /// <param name="data">The pointer to the raw array that will be filled with the read data.</param>
        /// <param name="length">The length of the byte array that should be read.</param>
        /// <returns>True if reading was successful.</returns>
        public bool ReadBytesDirect(ulong address, byte* data, uint length)
        {
            CSliceMut_u8 cBytes = new()
            {
                data = data,
                len = (nuint)length
            };
            if (Interop.Methods.mf_processinstance_read_raw_into(this._process, address, cBytes) != 0)
            {
                if (this._loglevel > 1)
                    PrintLog($"### Failed to read data from 0x{address:X}!");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Reads a number of bytes from an address of the process and stores them in the internal allocated buffer.
        /// </summary>
        /// <param name="address">The virtual address to read from.</param>
        /// <param name="length">The length of the byte array that should be read.</param>
        /// <returns>True if reading was successful.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool ReadBytesCached(ulong address, uint length)
        {
            CSliceMut_u8 cBytes = new()
            {
                data = this._rwBuffer,
                len = (nuint)length
            };
            if (Interop.Methods.mf_processinstance_read_raw_into(this._process, address, cBytes) != 0)
            {
                if (this._loglevel > 1)
                    PrintLog($"### Failed to read data from 0x{address:X}!");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Reads a number of bytes smaller than 4096 bytes from an address of the process.
        /// </summary>
        /// <param name="address">The virtual address to read from.</param>
        /// <param name="length">The length of the byte array that should be read. Maximum size is 4096.</param>
        /// <returns>The raw byte array, zero filled if read was not successful.</returns>
        /// <remarks>Maximum length of the array is 4096 bytes. Use ReadBytesUncapped() if you need more.</remarks>
        public byte[] ReadBytes(ulong address, uint length)
        {
            byte[] ba = new byte[length];
            if (!ReadBytesCached(address, length))
                return ba;
            Unsafe.CopyBlockUnaligned(Unsafe.AsPointer(ref ba[0]), this._rwBuffer, length);
            return ba;
        }

        /// <summary>
        /// Reads a number of bytes from an address of the process.
        /// </summary>
        /// <param name="address">The virtual address to read from.</param>
        /// <param name="length">The length of the byte array that should be read.</param>
        /// <returns>The raw byte array, zero filled if read was not successful.</returns>
        /// <remarks>This is slower than the buffered ReadBytes(), only use this if neccessary.</remarks>
        public byte[] ReadBytesUncapped(ulong address, uint length)
        {
            byte[] ba = new byte[length];
            fixed (byte* ptr = &ba[0])
            {
                CSliceMut_u8 cBytes = new()
                {
                    data = ptr,
                    len = (nuint)length
                };
                if (Interop.Methods.mf_processinstance_read_raw_into(this._process, address, cBytes) != 0)
                {
                    if (this._loglevel > 1)
                        PrintLog($"### Failed to read data from 0x{address:X}!");
                }
                return ba;
            }
        }

        /// <summary>
        /// Reads a given type from memory.
        /// </summary>
        /// <param name="address">The virtual address to read from.</param>
        /// <returns>The given type representation of the data, 0 or empty if read was not successful.</returns>
        public T? Read<T>(ulong address)
        {
            if (!ReadBytesCached(address, (uint)Unsafe.SizeOf<T>()))
                return default(T);
            return Unsafe.Read<T>(this._rwBuffer);
        }

        /// <summary>
        /// Reads an unsigned byte from memory.
        /// </summary>
        /// <param name="address">The virtual address to read from.</param>
        /// <returns>The byte representation of the data, 0 if read was not successful.</returns>
        public Byte ReadByte(ulong address)
        {
            if (!ReadBytesCached(address, 1))
                return 0;
            return Unsafe.Read<Byte>(this._rwBuffer);
        }

        /// <summary>
        /// Reads a 2 byte integer from memory.
        /// </summary>
        /// <param name="address">The virtual address to read from.</param>
        /// <returns>The integer representation of the data, 0 if read was not successful.</returns>
        public Int16 ReadInt16(ulong address)
        {
            if (!ReadBytesCached(address, 2))
                return 0;
            //return *(Int16*)this._rwBuffer;
            return Unsafe.Read<Int16>(this._rwBuffer);
        }

        /// <summary>
        /// Reads a 4 byte integer from memory.
        /// </summary>
        /// <param name="address">The virtual address to read from.</param>
        /// <returns>The integer representation of the data, 0 if read was not successful.</returns>
        public Int32 ReadInt32(ulong address)
        {
            if (!ReadBytesCached(address, 4))
                return 0;
            return Unsafe.Read<Int32>(this._rwBuffer);
        }

        /// <summary>
        /// Reads a 8 byte integer from memory.
        /// </summary>
        /// <param name="address">The virtual address to read from.</param>
        /// <returns>The integer representation of the data, 0 if read was not successful.</returns>
        public Int64 ReadInt64(ulong address)
        {
            if (!ReadBytesCached(address, 8))
                return 0;
            return Unsafe.Read<Int64>(this._rwBuffer);
        }

        /// <summary>
        /// Reads a 2 byte unsigned integer from memory.
        /// </summary>
        /// <param name="address">The virtual address to read from.</param>
        /// <returns>The integer representation of the data, 0 if read was not successful.</returns>
        public UInt16 ReadUInt16(ulong address)
        {
            if (!ReadBytesCached(address, 2))
                return 0;
            return Unsafe.Read<UInt16>(this._rwBuffer);
        }

        /// <summary>
        /// Reads a 4 byte unsigned integer from memory.
        /// </summary>
        /// <param name="address">The virtual address to read from.</param>
        /// <returns>The integer representation of the data, 0 if read was not successful.</returns>
        public UInt32 ReadUInt32(ulong address)
        {
            if (!ReadBytesCached(address, 4))
                return 0;
            return Unsafe.Read<UInt32>(this._rwBuffer);
        }

        /// <summary>
        /// Reads a 8 byte unsigned integer from memory.
        /// </summary>
        /// <param name="address">The virtual address to read from.</param>
        /// <returns>The integer representation of the data, 0 if read was not successful.</returns>
        public UInt64 ReadUInt64(ulong address)
        {
            if (!ReadBytesCached(address, 8))
                return 0;
            return Unsafe.Read<UInt64>(this._rwBuffer);
        }

        /// <summary>
        /// Reads a 4 byte single-precision float from memory.
        /// </summary>
        /// <param name="address">The virtual address to read from.</param>
        /// <returns>The float representation of the data, 0 if read was not successful.</returns>
        public Single ReadSingle(ulong address)
        {
            if (!ReadBytesCached(address, 4))
                return 0;
            return Unsafe.Read<Single>(this._rwBuffer);
        }

        /// <summary>
        /// Reads a 8 byte double-precision float from memory.
        /// </summary>
        /// <param name="address">The virtual address to read from.</param>
        /// <returns>The float representation of the data, 0 if read was not successful.</returns>
        public Double ReadDouble(ulong address)
        {
            if (!ReadBytesCached(address, 8))
                return 0;
            return Unsafe.Read<Double>(this._rwBuffer);
        }

        /// <summary>
        /// Reads a fixed length string from memory.
        /// </summary>
        /// <param name="address">The virtual address to read from.</param>
        /// <param name="length">The length of the string in bytes.</param>
        /// <param name="encoding">The encoding to use when reading.</param>
        /// <returns>The string representation of the data, empty if read was not successful.</returns>
        public string ReadFixedString(ulong address, uint length, Encoding encoding)
        {
            if (!ReadBytesCached(address, length))
                return "";
            return encoding.GetString(this._rwBuffer, (int)length);
        }

        /// <summary>
        /// Reads an unknown length ASCII string, up to the first terminator, from memory.
        /// </summary>
        /// <param name="address">The virtual address to read from.</param>
        /// <param name="length">The maximum length of the string in bytes.</param>
        /// <returns>The ASCII representation of the data, empty if read was not successful.</returns>
        public string ReadVariableString(ulong address, uint maxLength = 32)
        {
            if (!ReadBytesCached(address, maxLength))
                return "";
            byte* p = this._rwBuffer;
            for (int i = 0; i < maxLength; i++, p++) 
            {
                // find terminator
                if (*(p) == (byte)0x00)
                    return Encoding.ASCII.GetString(this._rwBuffer, i);
                    //return new string((sbyte*)this._rwBuffer); // DANGER! this will create a buffer-overflow if no null-terminator is present
            }
            return Encoding.ASCII.GetString(this._rwBuffer, (int)maxLength);
        }

        /// <summary>
        /// Writes a byte array to an address of the process.
        /// </summary>
        /// <param name="address">The virtual address to write to.</param>
        /// <param name="data">The data to write.</param>
        /// <param name="length">The length of the byte array that should be written.</param>
        /// <returns>True if writing was successful.</returns>
        public bool WriteBytesDirect(ulong address, byte* data, int length)
        {
            CSliceRef_u8 cBytes = new()
            {
                data = data,
                len = (nuint)length,
            };
            if (Interop.Methods.mf_processinstance_write_raw(this._process, address, cBytes) != 0)
            {
                if (this._loglevel > 1)
                    PrintLog($"### Failed to write data to 0x{address:X}!");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Writes a byte array from the internal buffer to an address of the process.
        /// </summary>
        /// <param name="address">The virtual address to write to.</param>
        /// <param name="length">The length of the data to write from buffer.</param>
        /// <returns>True if writing was successful.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool WriteBytesCached(ulong address, int length)
        {
            CSliceRef_u8 cBytes = new()
            {
                data = this._rwBuffer,
                len = (nuint)length,
            };
            if (Interop.Methods.mf_processinstance_write_raw(this._process, address, cBytes) != 0)
            {
                if (this._loglevel > 1)
                    PrintLog($"### Failed to write data to 0x{address:X}!");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Writes a byte array smaller than 4096 bytes to an address of the process.
        /// </summary>
        /// <param name="address">The virtual address to write to.</param>
        /// <param name="data">The data to write. Maximum size is 4096 bytes.</param>
        /// <returns>True if writing was successful.</returns>
        /// <remarks>Maximum length of the array is 4096 bytes. Use WriteBytesUncapped() if you need more.</remarks>
        public bool WriteBytes(ulong address, byte[] data)
        {
            Unsafe.CopyBlockUnaligned(this._rwBuffer, Unsafe.AsPointer(ref data[0]), (uint)data.Length);
            return WriteBytesCached(address, data.Length);
        }

        /// <summary>
        /// Writes a byte array to an address of the process.
        /// </summary>
        /// <param name="address">The virtual address to write to.</param>
        /// <param name="data">The data to write.</param>
        /// <returns>True if writing was successful.</returns>
        /// <remarks>This is slower than the buffered WriteBytes(), only use this if neccessary.</remarks>
        public bool WriteBytesUncapped(ulong address, byte[] data)
        {
            fixed (byte* ptr = &data[0]) 
            {
                CSliceRef_u8 cBytes = new()
                {
                    data = ptr,
                    len = (nuint)data.Length,
                };
                if (Interop.Methods.mf_processinstance_write_raw(this._process, address, cBytes) != 0)
                {
                    if (this._loglevel > 1)
                        PrintLog($"### Failed to write data to 0x{address:X}!");
                    return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Writes a given type to memory.
        /// </summary>
        /// <param name="address">The virtual address to write to.</param>
        /// <param name="data">The data to write.</param>
        /// <returns>True if writing was successful.</returns>
        public bool Write<T>(ulong address, T data)
        {
            Unsafe.Write<T>(this._rwBuffer, data);
            return WriteBytesCached(address, Unsafe.SizeOf<T>());
        }

        /// <summary>
        /// Writes an unsigned byte to memory.
        /// </summary>
        /// <param name="address">The virtual address to write to.</param>
        /// <param name="data">The byte to write.</param>
        /// <returns>True if writing was successful.</returns>
        public bool WriteByte(ulong address, byte data)
        {
            Unsafe.Write<Byte>(this._rwBuffer, data);
            return WriteBytesCached(address, 1);
        }

        /// <summary>
        /// Writes a 2 byte integer to memory.
        /// </summary>
        /// <param name="address">The virtual address to write to.</param>
        /// <param name="data">The integer to write.</param>
        /// <returns>True if writing was successful.</returns>
        public bool WriteInt16(ulong address, Int16 data)
        {
            Unsafe.Write<Int16>(this._rwBuffer, data);
            return WriteBytesCached(address, 2);
        }

        /// <summary>
        /// Writes a 4 byte integer to memory.
        /// </summary>
        /// <param name="address">The virtual address to write to.</param>
        /// <param name="data">The integer to write.</param>
        /// <returns>True if writing was successful.</returns>
        public bool WriteInt32(ulong address, Int32 data)
        {
            Unsafe.Write<Int32>(this._rwBuffer, data);
            return WriteBytesCached(address, 4);
        }

        /// <summary>
        /// Writes a 8 byte integer to memory.
        /// </summary>
        /// <param name="address">The virtual address to write to.</param>
        /// <param name="data">The integer to write.</param>
        /// <returns>True if writing was successful.</returns>
        public bool WriteInt64(ulong address, Int64 data)
        {
            Unsafe.Write<Int64>(this._rwBuffer, data);
            return WriteBytesCached(address, 8);
        }

        /// <summary>
        /// Writes a 2 byte unsigned integer to memory.
        /// </summary>
        /// <param name="address">The virtual address to write to.</param>
        /// <param name="data">The integer to write.</param>
        /// <returns>True if writing was successful.</returns>
        public bool WriteUInt16(ulong address, UInt16 data)
        {
            Unsafe.Write<UInt16>(this._rwBuffer, data);
            return WriteBytesCached(address, 2);
        }

        /// <summary>
        /// Writes a 4 byte unsigned integer to memory.
        /// </summary>
        /// <param name="address">The virtual address to write to.</param>
        /// <param name="data">The integer to write.</param>
        /// <returns>True if writing was successful.</returns>
        public bool WriteUInt32(ulong address, UInt32 data)
        {
            Unsafe.Write<UInt32>(this._rwBuffer, data);
            return WriteBytesCached(address, 4);
        }

        /// <summary>
        /// Writes a 8 byte unsigned integer to memory.
        /// </summary>
        /// <param name="address">The virtual address to write to.</param>
        /// <param name="data">The integer to write.</param>
        /// <returns>True if writing was successful.</returns>
        public bool WriteUInt64(ulong address, UInt64 data)
        {
            Unsafe.Write<UInt64>(this._rwBuffer, data);
            return WriteBytesCached(address, 8);
        }

        /// <summary>
        /// Writes a 4 byte single-precision float to memory.
        /// </summary>
        /// <param name="address">The virtual address to write to.</param>
        /// <param name="data">The float to write.</param>
        /// <returns>True if writing was successful.</returns>
        public bool WriteSingle(ulong address, float data)
        {
            Unsafe.Write<Single>(this._rwBuffer, data);
            return WriteBytesCached(address, 4);
        }

        /// <summary>
        /// Writes a 8 byte double-precision float to memory.
        /// </summary>
        /// <param name="address">The virtual address to write to.</param>
        /// <param name="data">The float to write.</param>
        /// <returns>True if writing was successful.</returns>
        public bool WriteDouble(ulong address, float data)
        {
            Unsafe.Write<Double>(this._rwBuffer, data);
            return WriteBytesCached(address, 8);
        }

        /// <summary>
        /// Writes a managed string as an encoded byte sequence to memory. 
        /// If encoding uses single byte representation this will add a null-terminator.
        /// </summary>
        /// <param name="address">The virtual address to write to.</param>
        /// <param name="data">The string to write.</param>
        /// <param name="encoding">The encoding tu use when writing.</param>
        /// <returns>True if writing was successful.</returns>
        public bool WriteString(ulong address, string data, Encoding encoding)
        {
            byte[] str = encoding.GetBytes(data);
            if (encoding.IsSingleByte) 
            {
                byte[] terminatedStr = new byte[str.Length + 1];   // string byte representation + 00 terminator
                Unsafe.CopyBlockUnaligned(ref terminatedStr[0], ref str[0], (uint)str.Length);
                return WriteBytes(address, terminatedStr);
            }
            else 
                return WriteBytes(address, str);
        }

        /// <summary>
        /// Reads a compile-time static, relative 4 bytes offset from an instruction and dereferences it.
        /// </summary>
        /// <param name="addressToRelativeOffset">The address the offset is located at.</param>
        /// <returns>The actual, non-relative address the offset points to.</returns>
        /// <remarks>
        /// In assembler, fixed addresses are mostly relative offset from the current instruction address, for example: 
        /// 00000150 | 48:8B05 02010000 | mov rax, qword ptr ds:[00000277]. 
        /// The instruction is located at address 0x150. 
        /// The length of the instruction is 7 bytes. 
        /// The instruction contains a relative offset which is already shown resolved as [00000277]. 
        /// The relative offset starts after 3 bytes and is 4 bytes long. 
        /// The relative offset is 02 01 00 00 -> 00 00 01 20 = 0x120 as numbers are reversed in memory due to little endianess. 
        /// The offset points to 0x150 (address) + 7 (instruction length) + 0x120 (relative offset) = 0x277. 
        /// This function takes the addressToRelativeOffset; in this example 0x150 + 3 and returns 0x277.
        /// </remarks>
        public ulong DereferenceRelativeOffset(ulong addressToRelativeOffset)
        {
            return addressToRelativeOffset + ReadUInt32(addressToRelativeOffset) + 0x4;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        public void Dispose()
        {
            if (this._process != null) 
            {
                Interop.Methods.mf_processinstance_drop(*this._process);
                Marshal.FreeCoTaskMem((IntPtr)this._process);
                if (this._loglevel > 2)
                    PrintLog("### Process freed");
            }
            if (this.Success) 
                NativeMemory.Free(this._rwBuffer);
        }
    }

    /// <summary>
    /// Runs shell commands and returns stdout.
    /// </summary>
    public static class ShellHelper
    {
        public static string Bash(this string cmd)
        {
            var escapedArgs = cmd.Replace("\"", "\\\"");

            var process = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{escapedArgs}\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            process.Start();
            string result = process.StandardOutput.ReadToEnd();
            process.WaitForExit();

            return result.Replace("\n", "");
        }
    }
}
