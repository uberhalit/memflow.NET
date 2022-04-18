using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace memflowNET.Interop
{
    /// <summary>Defines the type of a member as it was used in the native signature.</summary>
    [AttributeUsage(AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.ReturnValue, AllowMultiple = false, Inherited = true)]
    [Conditional("DEBUG")]
    internal sealed partial class NativeTypeNameAttribute : Attribute
    {
        private readonly string _name;

        /// <summary>Initializes a new instance of the <see cref="NativeTypeNameAttribute" /> class.</summary>
        /// <param name="name">The name of the type that was used in the native signature.</param>
        public NativeTypeNameAttribute(string name)
        {
            _name = name;
        }

        /// <summary>Gets the name of the type that was used in the native signature.</summary>
        public string Name => _name;
    }

    /// <summary>Defines the base type of a struct as it was in the native signature.</summary>
    [AttributeUsage(AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    [Conditional("DEBUG")]
    internal sealed partial class NativeInheritanceAttribute : Attribute
    {
        private readonly string _name;

        /// <summary>Initializes a new instance of the <see cref="NativeInheritanceAttribute" /> class.</summary>
        /// <param name="name">The name of the base type that was inherited from in the native signature.</param>
        public NativeInheritanceAttribute(string name)
        {
            _name = name;
        }

        /// <summary>Gets the name of the base type that was inherited from in the native signature.</summary>
        public string Name => _name;
    }

    /// <summary>
    /// Identifies the byte order of a architecture
    /// This enum is used when reading/writing to/from the memory of a target system.The memory will be automatically converted to the endianess memflow is currently running on.
    /// See the [wikipedia article](https://en.wikipedia.org/wiki/Endianness) for more information on the subject.
    /// </summary>
    [NativeTypeName("uint8_t")]
    public enum Endianess : byte
    {
        Endianess_LittleEndian,
        Endianess_BigEndian,
    }

    /// <summary>
    /// An enum representing the available verbosity levels of the logger.
    /// Typical usage includes: checking if a certain `Level` is enabled with[`log_enabled!`](macro.log_enabled.html), specifying the `Level` of[`log!`](macro.log.html), and comparing a `Level` directly to a[`LevelFilter`](enum.LevelFilter.html).
    /// </summary>
    [NativeTypeName("uintptr_t")]
    public enum Level : uint
    {
        Level_Error = 1,
        Level_Warn,
        Level_Info,
        Level_Debug,
        Level_Trace,
    }

    /// <summary>
    /// An enum representing the available verbosity level filters of the logger.
    /// A `LevelFilter` may be compared directly to a [`Level`]. Use this typeto get and set the maximum log level with [`max_level()`] and [`set_max_level`].
    /// [`Level`]: enum.Level.html[`max_level()`]: fn.max_level.html[`set_max_level`]: fn.set_max_level.html
    /// </summary>
    [NativeTypeName("uintptr_t")]
    public enum LevelFilter : uint
    {
        LevelFilter_Off,
        LevelFilter_Error,
        LevelFilter_Warn,
        LevelFilter_Info,
        LevelFilter_Debug,
        LevelFilter_Trace,
    }

    public partial struct ArchitectureObj
    {
    }

    /// <summary>
    /// The core of the plugin system
    /// It scans system directories and collects valid memflow plugins. They can then be instantiatedeasily. The reason the libraries are collected is to allow for reuse, and save performance
    /// # Examples
    /// Creating a OS instance, the recommended way:
    /// no_runuse memflow::plugins::Inventory;# use memflow::plugins::OsInstanceArcBox;# use memflow::error::Result;# fn test() -> Result< OsInstanceArcBox<'static>> {let inventory = Inventory::scan();inventory.builder().connector("qemu").os("win32").build()# }# test().ok();
    /// Nesting connectors and os plugins: no_runuse memflow::plugins::{Inventory, Args};# use memflow::error::Result;# fn test() -> Result<()> {let inventory = Inventory::scan();let os = inventory.builder().connector("qemu").os("linux").connector("qemu").os("win32").build();# Ok(())# }# test().ok();
    /// </summary>
    public partial struct Inventory
    {
    }

    /// <summary>
    /// This type represents a wrapper over a [address](address/index.html)with additional information about the containing page in the physical memory domain.
    /// This type will mostly be used by the [virtual to physical address translation](todo.html).When a physical address is translated from a virtual address the additional informationabout the allocated page the virtual address points to can be obtained from this structure.
    /// Most architectures have support multiple page sizes (see [huge pages](todo.html))which will be represented by the containing `page` of the `PhysicalAddress` struct.
    /// </summary>
    public partial struct PhysicalAddress
    {
        [NativeTypeName("Address")]
        public ulong address;

        [NativeTypeName("PageType")]
        public byte page_type;

        [NativeTypeName("uint8_t")]
        public byte page_size_log2;
    }

    /// <summary>
    /// FFI-safe box
    /// This box has a static self reference, alongside a custom drop function.
    /// The drop function can be called from anywhere, it will free on correct allocator internally.
    /// </summary>
    public unsafe partial struct CBox_c_void
    {
        public void* instance;

        [NativeTypeName("void (*)(void *)")]
        public delegate* unmanaged[Cdecl]<void*, void> drop_fn;
    }

    /// <summary>
    /// FFI-Safe Arc
    /// This is an FFI-Safe equivalent of Arc<T> and Option<Arc<T>>.
    /// </summary>
    public unsafe partial struct CArc_c_void
    {
        [NativeTypeName("const void *")]
        public void* instance;

        [NativeTypeName("const void *(*)(const void *)")]
        public delegate* unmanaged[Cdecl]<void*, void*> clone_fn;

        [NativeTypeName("void (*)(const void *)")]
        public delegate* unmanaged[Cdecl]<void*, void> drop_fn;
    }

    public partial struct ConnectorInstanceContainer_CBox_c_void_____CArc_c_void
    {
        [NativeTypeName("struct CBox_c_void")]
        public CBox_c_void instance;

        [NativeTypeName("struct CArc_c_void")]
        public CArc_c_void context;
    }

    /// <summary>
    /// CGlue vtable for trait Clone.
    /// This virtual function table contains ABI-safe interface for the given trait.
    /// </summary>
    public unsafe partial struct CloneVtbl_ConnectorInstanceContainer_CBox_c_void_____CArc_c_void
    {
        [NativeTypeName("struct ConnectorInstanceContainer_CBox_c_void_____CArc_c_void (*)(const struct ConnectorInstanceContainer_CBox_c_void_____CArc_c_void *)")]
        public delegate* unmanaged[Cdecl]<ConnectorInstanceContainer_CBox_c_void_____CArc_c_void*, ConnectorInstanceContainer_CBox_c_void_____CArc_c_void> clone;
    }

    /// <summary>
    /// Wrapper around mutable slices.
    /// This is meant as a safe type to pass across the FFI boundary with similar semantics as regularslice. However, not all functionality is present, use the slice conversion functions.
    /// </summary>
    public unsafe partial struct CSliceMut_u8
    {
        [NativeTypeName("uint8_t *")]
        public byte* data;

        [NativeTypeName("uintptr_t")]
        public nuint len;
    }

    /// <summary>
    /// FFI-safe 3 element tuple.
    /// </summary>
    public partial struct CTup3_PhysicalAddress__Address__CSliceMut_u8
    {
        [NativeTypeName("struct PhysicalAddress")]
        public PhysicalAddress _0;

        [NativeTypeName("Address")]
        public ulong _1;

        [NativeTypeName("struct CSliceMut_u8")]
        public CSliceMut_u8 _2;
    }

    /// <summary>
    /// FFI compatible iterator.
    /// Any mutable reference to an iterator can be converted to a `CIterator`.
    /// `CIterator<T>` implements `Iterator<Item= T>`.
    /// # Examples
    /// Using [`AsCIterator`](AsCIterator) helper:
    /// ```use cglue::iter::{CIterator, AsCIterator};
    /// extern "C" fn sum_all(iter: CIterator<usize>) -> usize {iter.sum()}
    /// let mut iter = (0..10).map(|v| v * v);
    /// assert_eq!(sum_all(iter.as_citer()), 285);```
    /// Converting with `Into` trait:
    /// ```use cglue::iter::{CIterator, AsCIterator};
    /// extern "C" fn sum_all(iter: CIterator<usize>) -> usize {iter.sum()}
    /// let mut iter = (0..=10).map(|v| v * v);
    /// assert_eq!(sum_all((&mutiter).into()), 385);```
    /// </summary>
    public unsafe partial struct CIterator_PhysicalReadData
    {
        public void* iter;

        [NativeTypeName("int32_t (*)(void *, PhysicalReadData *)")]
        public delegate* unmanaged[Cdecl]<void*, CTup3_PhysicalAddress__Address__CSliceMut_u8*, int> func;
    }

    /// <summary>
    /// FFI-safe 2 element tuple.
    /// </summary>
    public partial struct CTup2_Address__CSliceMut_u8
    {
        [NativeTypeName("Address")]
        public ulong _0;

        [NativeTypeName("struct CSliceMut_u8")]
        public CSliceMut_u8 _1;
    }

    public unsafe partial struct Callback_c_void__ReadData
    {
        public void* context;

        [NativeTypeName("bool (*)(void *, ReadData)")]
        public delegate* unmanaged[Cdecl]<void*, CTup2_Address__CSliceMut_u8, byte> func;
    }

    /// <summary>
    /// Data needed to perform memory operations.
    /// `inp` is an iterator containing
    /// </summary>
    public unsafe partial struct MemOps_PhysicalReadData__ReadData
    {
        [NativeTypeName("struct CIterator_PhysicalReadData")]
        public CIterator_PhysicalReadData inp;

        [NativeTypeName("OpaqueCallback_ReadData *")]
        public Callback_c_void__ReadData* @out;

        [NativeTypeName("OpaqueCallback_ReadData *")]
        public Callback_c_void__ReadData* out_fail;
    }

    /// <summary>
    /// Wrapper around const slices.
    /// This is meant as a safe type to pass across the FFI boundary with similar semantics as regularslice. However, not all functionality is present, use the slice conversion functions.
    /// # Examples
    /// Simple conversion:
    /// ```use cglue::slice::CSliceRef;
    /// let arr = [0, 5, 3, 2];
    /// let cslice = CSliceRef::from(&arr[..]);
    /// let slice = cslice.as_slice();
    /// assert_eq!(&arr, slice);```
    /// </summary>
    public unsafe partial struct CSliceRef_u8
    {
        [NativeTypeName("const uint8_t *")]
        public byte* data;

        [NativeTypeName("uintptr_t")]
        public nuint len;
    }

    /// <summary>
    /// FFI-safe 3 element tuple.
    /// </summary>
    public partial struct CTup3_PhysicalAddress__Address__CSliceRef_u8
    {
        [NativeTypeName("struct PhysicalAddress")]
        public PhysicalAddress _0;

        [NativeTypeName("Address")]
        public ulong _1;

        [NativeTypeName("struct CSliceRef_u8")]
        public CSliceRef_u8 _2;
    }

    /// <summary>
    /// FFI compatible iterator.
    /// Any mutable reference to an iterator can be converted to a `CIterator`.
    /// `CIterator<T>` implements `Iterator<Item= T>`.
    /// # Examples
    /// Using [`AsCIterator`](AsCIterator) helper:
    /// ```use cglue::iter::{CIterator, AsCIterator};
    /// extern "C" fn sum_all(iter: CIterator<usize>) -> usize {iter.sum()}
    /// let mut iter = (0..10).map(|v| v * v);
    /// assert_eq!(sum_all(iter.as_citer()), 285);```
    /// Converting with `Into` trait:
    /// ```use cglue::iter::{CIterator, AsCIterator};
    /// extern "C" fn sum_all(iter: CIterator<usize>) -> usize {iter.sum()}
    /// let mut iter = (0..=10).map(|v| v * v);
    /// assert_eq!(sum_all((&mutiter).into()), 385);```
    /// </summary>
    public unsafe partial struct CIterator_PhysicalWriteData
    {
        public void* iter;

        [NativeTypeName("int32_t (*)(void *, PhysicalWriteData *)")]
        public delegate* unmanaged[Cdecl]<void*, CTup3_PhysicalAddress__Address__CSliceRef_u8*, int> func;
    }

    /// <summary>
    /// FFI-safe 2 element tuple.
    /// </summary>
    public partial struct CTup2_Address__CSliceRef_u8
    {
        [NativeTypeName("Address")]
        public ulong _0;

        [NativeTypeName("struct CSliceRef_u8")]
        public CSliceRef_u8 _1;
    }

    public unsafe partial struct Callback_c_void__WriteData
    {
        public void* context;

        [NativeTypeName("bool (*)(void *, WriteData)")]
        public delegate* unmanaged[Cdecl]<void*, CTup2_Address__CSliceRef_u8, byte> func;
    }

    /// <summary>
    /// Data needed to perform memory operations.
    /// `inp` is an iterator containing
    /// </summary>
    public unsafe partial struct MemOps_PhysicalWriteData__WriteData
    {
        [NativeTypeName("struct CIterator_PhysicalWriteData")]
        public CIterator_PhysicalWriteData inp;

        [NativeTypeName("OpaqueCallback_WriteData *")]
        public Callback_c_void__WriteData* @out;

        [NativeTypeName("OpaqueCallback_WriteData *")]
        public Callback_c_void__WriteData* out_fail;
    }

    public partial struct PhysicalMemoryMetadata
    {
        [NativeTypeName("Address")]
        public ulong max_address;

        [NativeTypeName("umem")]
        public ulong real_size;

        public bool @readonly;

        [NativeTypeName("uint32_t")]
        public uint ideal_batch_size;
    }

    public partial struct PhysicalMemoryMapping
    {
        [NativeTypeName("Address")]
        public ulong @base;

        [NativeTypeName("umem")]
        public ulong size;

        [NativeTypeName("Address")]
        public ulong real_base;
    }

    /// <summary>
    /// Wrapper around const slices.
    /// This is meant as a safe type to pass across the FFI boundary with similar semantics as regularslice. However, not all functionality is present, use the slice conversion functions.
    /// # Examples
    /// Simple conversion:
    /// ```use cglue::slice::CSliceRef;
    /// let arr = [0, 5, 3, 2];
    /// let cslice = CSliceRef::from(&arr[..]);
    /// let slice = cslice.as_slice();
    /// assert_eq!(&arr, slice);```
    /// </summary>
    public unsafe partial struct CSliceRef_PhysicalMemoryMapping
    {
        [NativeTypeName("const struct PhysicalMemoryMapping *")]
        public PhysicalMemoryMapping* data;

        [NativeTypeName("uintptr_t")]
        public nuint len;
    }

    /// <summary>
    /// FFI-safe 3 element tuple.
    /// </summary>
    public partial struct CTup3_Address__Address__CSliceMut_u8
    {
        [NativeTypeName("Address")]
        public ulong _0;

        [NativeTypeName("Address")]
        public ulong _1;

        [NativeTypeName("struct CSliceMut_u8")]
        public CSliceMut_u8 _2;
    }

    /// <summary>
    /// FFI compatible iterator.
    /// Any mutable reference to an iterator can be converted to a `CIterator`.
    /// `CIterator<T>` implements `Iterator<Item= T>`.
    /// # Examples
    /// Using [`AsCIterator`](AsCIterator) helper:
    /// ```use cglue::iter::{CIterator, AsCIterator};
    /// extern "C" fn sum_all(iter: CIterator<usize>) -> usize {iter.sum()}
    /// let mut iter = (0..10).map(|v| v * v);
    /// assert_eq!(sum_all(iter.as_citer()), 285);```
    /// Converting with `Into` trait:
    /// ```use cglue::iter::{CIterator, AsCIterator};
    /// extern "C" fn sum_all(iter: CIterator<usize>) -> usize {iter.sum()}
    /// let mut iter = (0..=10).map(|v| v * v);
    /// assert_eq!(sum_all((&mutiter).into()), 385);```
    /// </summary>
    public unsafe partial struct CIterator_ReadDataRaw
    {
        public void* iter;

        [NativeTypeName("int32_t (*)(void *, ReadDataRaw *)")]
        public delegate* unmanaged[Cdecl]<void*, CTup3_Address__Address__CSliceMut_u8*, int> func;
    }

    /// <summary>
    /// Data needed to perform memory operations.
    /// `inp` is an iterator containing
    /// </summary>
    public unsafe partial struct MemOps_ReadDataRaw__ReadData
    {
        [NativeTypeName("struct CIterator_ReadDataRaw")]
        public CIterator_ReadDataRaw inp;

        [NativeTypeName("OpaqueCallback_ReadData *")]
        public Callback_c_void__ReadData* @out;

        [NativeTypeName("OpaqueCallback_ReadData *")]
        public Callback_c_void__ReadData* out_fail;
    }

    /// <summary>
    /// FFI-safe 3 element tuple.
    /// </summary>
    public partial struct CTup3_Address__Address__CSliceRef_u8
    {
        [NativeTypeName("Address")]
        public ulong _0;

        [NativeTypeName("Address")]
        public ulong _1;

        [NativeTypeName("struct CSliceRef_u8")]
        public CSliceRef_u8 _2;
    }

    /// <summary>
    /// FFI compatible iterator.
    /// Any mutable reference to an iterator can be converted to a `CIterator`.
    /// `CIterator<T>` implements `Iterator<Item= T>`.
    /// # Examples
    /// Using [`AsCIterator`](AsCIterator) helper:
    /// ```use cglue::iter::{CIterator, AsCIterator};
    /// extern "C" fn sum_all(iter: CIterator<usize>) -> usize {iter.sum()}
    /// let mut iter = (0..10).map(|v| v * v);
    /// assert_eq!(sum_all(iter.as_citer()), 285);```
    /// Converting with `Into` trait:
    /// ```use cglue::iter::{CIterator, AsCIterator};
    /// extern "C" fn sum_all(iter: CIterator<usize>) -> usize {iter.sum()}
    /// let mut iter = (0..=10).map(|v| v * v);
    /// assert_eq!(sum_all((&mutiter).into()), 385);```
    /// </summary>
    public unsafe partial struct CIterator_WriteDataRaw
    {
        public void* iter;

        [NativeTypeName("int32_t (*)(void *, WriteDataRaw *)")]
        public delegate* unmanaged[Cdecl]<void*, CTup3_Address__Address__CSliceRef_u8*, int> func;
    }

    /// <summary>
    /// Data needed to perform memory operations.
    /// `inp` is an iterator containing
    /// </summary>
    public unsafe partial struct MemOps_WriteDataRaw__WriteData
    {
        [NativeTypeName("struct CIterator_WriteDataRaw")]
        public CIterator_WriteDataRaw inp;

        [NativeTypeName("OpaqueCallback_WriteData *")]
        public Callback_c_void__WriteData* @out;

        [NativeTypeName("OpaqueCallback_WriteData *")]
        public Callback_c_void__WriteData* out_fail;
    }

    public partial struct MemoryViewMetadata
    {
        [NativeTypeName("Address")]
        public ulong max_address;

        [NativeTypeName("umem")]
        public ulong real_size;

        public bool @readonly;

        public bool little_endian;

        [NativeTypeName("uint8_t")]
        public byte arch_bits;
    }

    /// <summary>
    /// FFI compatible iterator.
    /// Any mutable reference to an iterator can be converted to a `CIterator`.
    /// `CIterator<T>` implements `Iterator<Item= T>`.
    /// # Examples
    /// Using [`AsCIterator`](AsCIterator) helper:
    /// ```use cglue::iter::{CIterator, AsCIterator};
    /// extern "C" fn sum_all(iter: CIterator<usize>) -> usize {iter.sum()}
    /// let mut iter = (0..10).map(|v| v * v);
    /// assert_eq!(sum_all(iter.as_citer()), 285);```
    /// Converting with `Into` trait:
    /// ```use cglue::iter::{CIterator, AsCIterator};
    /// extern "C" fn sum_all(iter: CIterator<usize>) -> usize {iter.sum()}
    /// let mut iter = (0..=10).map(|v| v * v);
    /// assert_eq!(sum_all((&mutiter).into()), 385);```
    /// </summary>
    public unsafe partial struct CIterator_ReadData
    {
        public void* iter;

        [NativeTypeName("int32_t (*)(void *, ReadData *)")]
        public delegate* unmanaged[Cdecl]<void*, CTup2_Address__CSliceMut_u8*, int> func;
    }

    /// <summary>
    /// Wrapper around mutable slices.
    /// This is meant as a safe type to pass across the FFI boundary with similar semantics as regularslice. However, not all functionality is present, use the slice conversion functions.
    /// </summary>
    public unsafe partial struct CSliceMut_ReadData
    {
        [NativeTypeName("ReadData *")]
        public CTup2_Address__CSliceMut_u8* data;

        [NativeTypeName("uintptr_t")]
        public nuint len;
    }

    /// <summary>
    /// FFI compatible iterator.
    /// Any mutable reference to an iterator can be converted to a `CIterator`.
    /// `CIterator<T>` implements `Iterator<Item= T>`.
    /// # Examples
    /// Using [`AsCIterator`](AsCIterator) helper:
    /// ```use cglue::iter::{CIterator, AsCIterator};
    /// extern "C" fn sum_all(iter: CIterator<usize>) -> usize {iter.sum()}
    /// let mut iter = (0..10).map(|v| v * v);
    /// assert_eq!(sum_all(iter.as_citer()), 285);```
    /// Converting with `Into` trait:
    /// ```use cglue::iter::{CIterator, AsCIterator};
    /// extern "C" fn sum_all(iter: CIterator<usize>) -> usize {iter.sum()}
    /// let mut iter = (0..=10).map(|v| v * v);
    /// assert_eq!(sum_all((&mutiter).into()), 385);```
    /// </summary>
    public unsafe partial struct CIterator_WriteData
    {
        public void* iter;

        [NativeTypeName("int32_t (*)(void *, WriteData *)")]
        public delegate* unmanaged[Cdecl]<void*, CTup2_Address__CSliceRef_u8*, int> func;
    }

    /// <summary>
    /// Wrapper around const slices.
    /// This is meant as a safe type to pass across the FFI boundary with similar semantics as regularslice. However, not all functionality is present, use the slice conversion functions.
    /// # Examples
    /// Simple conversion:
    /// ```use cglue::slice::CSliceRef;
    /// let arr = [0, 5, 3, 2];
    /// let cslice = CSliceRef::from(&arr[..]);
    /// let slice = cslice.as_slice();
    /// assert_eq!(&arr, slice);```
    /// </summary>
    public unsafe partial struct CSliceRef_WriteData
    {
        [NativeTypeName("const WriteData *")]
        public CTup2_Address__CSliceRef_u8* data;

        [NativeTypeName("uintptr_t")]
        public nuint len;
    }

    /// <summary>
    /// Simple CGlue trait object container.
    /// This is the simplest form of container, represented by an instance, clone context, andtemporary return context.
    /// `instance` value usually is either a reference, or a mutable reference, or a `CBox`, whichcontains static reference to the instance, and a dedicated drop function for freeing resources.
    /// `context` is either `PhantomData` representing nothing, or typically a `CArc` that can becloned at will, reference counting some resource, like a `Library` for automatic unloading.
    /// `ret_tmp` is usually `PhantomData` representing nothing, unless the trait has functions thatreturn references to associated types, in which case space is reserved for wrapping structures.
    /// </summary>
    public partial struct CGlueObjContainer_CBox_c_void_____CArc_c_void_____CpuStateRetTmp_CArc_c_void
    {
        [NativeTypeName("struct CBox_c_void")]
        public CBox_c_void instance;

        public CArc_c_void context;
    }

    /// <summary>
    /// CGlue vtable for trait CpuState.
    /// This virtual function table contains ABI-safe interface for the given trait.
    /// </summary>
    public unsafe partial struct CpuStateVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____CpuStateRetTmp_CArc_c_void
    {
        [NativeTypeName("void (*)(struct CGlueObjContainer_CBox_c_void_____CArc_c_void_____CpuStateRetTmp_CArc_c_void *)")]
        public delegate* unmanaged[Cdecl]<CGlueObjContainer_CBox_c_void_____CArc_c_void_____CpuStateRetTmp_CArc_c_void*, void> pause;

        [NativeTypeName("void (*)(struct CGlueObjContainer_CBox_c_void_____CArc_c_void_____CpuStateRetTmp_CArc_c_void *)")]
        public delegate* unmanaged[Cdecl]<CGlueObjContainer_CBox_c_void_____CArc_c_void_____CpuStateRetTmp_CArc_c_void*, void> resume;
    }

    /// <summary>
    /// Forward declarations for vtables and their wrappers
    /// </summary>
    public unsafe partial struct CGlueTraitObj_CBox_c_void_____CpuStateVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____CpuStateRetTmp_CArc_c_void______________CArc_c_void_____CpuStateRetTmp_CArc_c_void
    {
        [NativeTypeName("const struct CpuStateVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____CpuStateRetTmp_CArc_c_void *")]
        public CpuStateVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____CpuStateRetTmp_CArc_c_void* vtbl;

        [NativeTypeName("struct CGlueObjContainer_CBox_c_void_____CArc_c_void_____CpuStateRetTmp_CArc_c_void")]
        public CGlueObjContainer_CBox_c_void_____CArc_c_void_____CpuStateRetTmp_CArc_c_void container;
    }

    public partial struct IntoCpuStateContainer_CBox_c_void_____CArc_c_void
    {
        [NativeTypeName("struct CBox_c_void")]
        public CBox_c_void instance;

        public CArc_c_void context;
    }

    /// <summary>
    /// CGlue vtable for trait Clone.
    /// This virtual function table contains ABI-safe interface for the given trait.
    /// </summary>
    public unsafe partial struct CloneVtbl_IntoCpuStateContainer_CBox_c_void_____CArc_c_void
    {
        [NativeTypeName("struct IntoCpuStateContainer_CBox_c_void_____CArc_c_void (*)(const struct IntoCpuStateContainer_CBox_c_void_____CArc_c_void *)")]
        public delegate* unmanaged[Cdecl]<IntoCpuStateContainer_CBox_c_void_____CArc_c_void*, IntoCpuStateContainer_CBox_c_void_____CArc_c_void> clone;
    }

    /// <summary>
    /// CGlue vtable for trait CpuState.
    /// This virtual function table contains ABI-safe interface for the given trait.
    /// </summary>
    public unsafe partial struct CpuStateVtbl_IntoCpuStateContainer_CBox_c_void_____CArc_c_void
    {
        [NativeTypeName("void (*)(struct IntoCpuStateContainer_CBox_c_void_____CArc_c_void *)")]
        public delegate* unmanaged[Cdecl]<IntoCpuStateContainer_CBox_c_void_____CArc_c_void*, void> pause;

        [NativeTypeName("void (*)(struct IntoCpuStateContainer_CBox_c_void_____CArc_c_void *)")]
        public delegate* unmanaged[Cdecl]<IntoCpuStateContainer_CBox_c_void_____CArc_c_void*, void> resume;
    }

    /// <summary>
    /// Trait group potentially implementing `:: cglue :: ext :: core :: clone :: Clone <> + CpuState <>` traits.
    /// Optional traits are not implemented here, however. There are numerous conversionfunctions available for safely retrieving a concrete collection of traits.
    /// `check_impl_` functions allow to check if the object implements the wanted traits.
    /// `into_impl_` functions consume the object and produce a new final structure thatkeeps only the required information.
    /// `cast_impl_` functions merely check and transform the object into a type that canbe transformed back into `IntoCpuState` without losing data.
    /// `as_ref_`, and `as_mut_` functions obtain references to safe objects, but do notperform any memory transformations either. They are the safest to use, becausethere is no risk of accidentally consuming the whole object.
    /// </summary>
    public unsafe partial struct IntoCpuState_CBox_c_void_____CArc_c_void
    {
        [NativeTypeName("const struct CloneVtbl_IntoCpuStateContainer_CBox_c_void_____CArc_c_void *")]
        public CloneVtbl_IntoCpuStateContainer_CBox_c_void_____CArc_c_void* vtbl_clone;

        [NativeTypeName("const struct CpuStateVtbl_IntoCpuStateContainer_CBox_c_void_____CArc_c_void *")]
        public CpuStateVtbl_IntoCpuStateContainer_CBox_c_void_____CArc_c_void* vtbl_cpustate;

        [NativeTypeName("struct IntoCpuStateContainer_CBox_c_void_____CArc_c_void")]
        public IntoCpuStateContainer_CBox_c_void_____CArc_c_void container;
    }

    /// <summary>
    /// CGlue vtable for trait ConnectorCpuStateInner.
    /// This virtual function table contains ABI-safe interface for the given trait.
    /// </summary>
    public unsafe partial struct ConnectorCpuStateInnerVtbl_ConnectorInstanceContainer_CBox_c_void_____CArc_c_void
    {
        [NativeTypeName("int32_t (*)(struct ConnectorInstanceContainer_CBox_c_void_____CArc_c_void *, CpuStateBase_CBox_c_void_____CArc_c_void *)")]
        public delegate* unmanaged[Cdecl]<ConnectorInstanceContainer_CBox_c_void_____CArc_c_void*, CGlueTraitObj_CBox_c_void_____CpuStateVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____CpuStateRetTmp_CArc_c_void______________CArc_c_void_____CpuStateRetTmp_CArc_c_void*, int> cpu_state;

        [NativeTypeName("int32_t (*)(struct ConnectorInstanceContainer_CBox_c_void_____CArc_c_void, struct IntoCpuState_CBox_c_void_____CArc_c_void *)")]
        public delegate* unmanaged[Cdecl]<ConnectorInstanceContainer_CBox_c_void_____CArc_c_void, IntoCpuState_CBox_c_void_____CArc_c_void*, int> into_cpu_state;
    }

    /// <summary>
    /// Trait group potentially implementing `:: cglue :: ext :: core :: clone :: Clone <> + PhysicalMemory <> + for <'cglue_c > ConnectorCpuStateInner <'cglue_c, >` traits.
    /// Optional traits are not implemented here, however. There are numerous conversionfunctions available for safely retrieving a concrete collection of traits.
    /// `check_impl_` functions allow to check if the object implements the wanted traits.
    /// `into_impl_` functions consume the object and produce a new final structure thatkeeps only the required information.
    /// `cast_impl_` functions merely check and transform the object into a type that canbe transformed back into `ConnectorInstance` without losing data.
    /// `as_ref_`, and `as_mut_` functions obtain references to safe objects, but do notperform any memory transformations either. They are the safest to use, becausethere is no risk of accidentally consuming the whole object.
    /// </summary>
    public unsafe partial struct ConnectorInstance_CBox_c_void_____CArc_c_void
    {
        [NativeTypeName("const struct CloneVtbl_ConnectorInstanceContainer_CBox_c_void_____CArc_c_void *")]
        public CloneVtbl_ConnectorInstanceContainer_CBox_c_void_____CArc_c_void* vtbl_clone;

        [NativeTypeName("const struct PhysicalMemoryVtbl_ConnectorInstanceContainer_CBox_c_void_____CArc_c_void *")]
        public PhysicalMemoryVtbl_ConnectorInstanceContainer_CBox_c_void_____CArc_c_void* vtbl_physicalmemory;

        [NativeTypeName("const struct ConnectorCpuStateInnerVtbl_ConnectorInstanceContainer_CBox_c_void_____CArc_c_void *")]
        public ConnectorCpuStateInnerVtbl_ConnectorInstanceContainer_CBox_c_void_____CArc_c_void* vtbl_connectorcpustateinner;

        [NativeTypeName("struct ConnectorInstanceContainer_CBox_c_void_____CArc_c_void")]
        public ConnectorInstanceContainer_CBox_c_void_____CArc_c_void container;
    }

    public partial struct OsInstanceContainer_CBox_c_void_____CArc_c_void
    {
        [NativeTypeName("struct CBox_c_void")]
        public CBox_c_void instance;

        [NativeTypeName("struct CArc_c_void")]
        public CArc_c_void context;
    }

    /// <summary>
    /// CGlue vtable for trait Clone.
    /// This virtual function table contains ABI-safe interface for the given trait.
    /// </summary>
    public unsafe partial struct CloneVtbl_OsInstanceContainer_CBox_c_void_____CArc_c_void
    {
        [NativeTypeName("struct OsInstanceContainer_CBox_c_void_____CArc_c_void (*)(const struct OsInstanceContainer_CBox_c_void_____CArc_c_void *)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, OsInstanceContainer_CBox_c_void_____CArc_c_void> clone;
    }

    public unsafe partial struct Callback_c_void__Address
    {
        public void* context;

        [NativeTypeName("bool (*)(void *, Address)")]
        public delegate* unmanaged[Cdecl]<void*, ulong, byte> func;
    }

    /// <summary>
    /// The state of a process
    /// # Remarks
    /// In case the exit code isn't known ProcessState::Unknown is set.
    /// </summary>
    public enum ProcessState_Tag
    {
        ProcessState_Unknown,
        ProcessState_Alive,
        ProcessState_Dead,
    }

    public partial struct ProcessState
    {
        public ProcessState_Tag tag;

        [NativeTypeName("ProcessState::(anonymous union at memflow.h:1135:5)")]
        public _Anonymous_e__Union Anonymous;

        public ref int dead
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.Anonymous.dead, 1));
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        public partial struct _Anonymous_e__Union
        {
            [FieldOffset(0)]
            [NativeTypeName("ProcessState::(anonymous struct at memflow.h:1136:9)")]
            public _Anonymous_e__Struct Anonymous;

            public partial struct _Anonymous_e__Struct
            {
                [NativeTypeName("ExitCode")]
                public int dead;
            }
        }
    }

    public enum ArchitectureIdent_Tag
    {
        ArchitectureIdent_Unknown,
        ArchitectureIdent_X86,
        ArchitectureIdent_AArch64,
    }

    public partial struct ArchitectureIdent_X86_Body
    {
        [NativeTypeName("uint8_t")]
        public byte _0;

        public bool _1;
    }

    public partial struct ArchitectureIdent
    {
        public ArchitectureIdent_Tag tag;

        [NativeTypeName("ArchitectureIdent::(anonymous union at memflow.h:1178:5)")]
        public _Anonymous_e__Union Anonymous;

        public ref nuint unknown
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.Anonymous1.unknown, 1));
            }
        }

        public ref ArchitectureIdent_X86_Body x86
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.x86, 1));
            }
        }

        public ref nuint a_arch64
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.Anonymous2.a_arch64, 1));
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        public partial struct _Anonymous_e__Union
        {
            [FieldOffset(0)]
            [NativeTypeName("ArchitectureIdent::(anonymous struct at memflow.h:1179:9)")]
            public _Anonymous1_e__Struct Anonymous1;

            [FieldOffset(0)]
            public ArchitectureIdent_X86_Body x86;

            [FieldOffset(0)]
            [NativeTypeName("ArchitectureIdent::(anonymous struct at memflow.h:1183:9)")]
            public _Anonymous2_e__Struct Anonymous2;

            public partial struct _Anonymous1_e__Struct
            {
                [NativeTypeName("uintptr_t")]
                public nuint unknown;
            }

            public partial struct _Anonymous2_e__Struct
            {
                [NativeTypeName("uintptr_t")]
                public nuint a_arch64;
            }
        }
    }

    /// <summary>
    /// Process information structure
    /// This structure implements basic process information. Architectures are provided both of thesystem, and of the process.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 56)]
    public unsafe partial struct ProcessInfo
    {
        [FieldOffset(0)]
        [NativeTypeName("Address")]
        public ulong address;

        [FieldOffset(8)]
        [NativeTypeName("Pid")]
        public uint pid;

        [FieldOffset(12)]
        [NativeTypeName("struct ProcessState")]
        public ProcessState state;

        [FieldOffset(24)]
        [NativeTypeName("ReprCString")]
        public sbyte* name;

        [FieldOffset(32)]
        [NativeTypeName("ReprCString")]
        public sbyte* path;

        [FieldOffset(40)]
        [NativeTypeName("ReprCString")]
        public sbyte* command_line;

        [FieldOffset(48)]
        [NativeTypeName("struct ArchitectureIdent")]
        public ArchitectureIdent sys_arch;

        [FieldOffset(52)]
        [NativeTypeName("struct ArchitectureIdent")]
        public ArchitectureIdent proc_arch;
    }

    public unsafe partial struct Callback_c_void__ProcessInfo
    {
        public void* context;

        [NativeTypeName("bool (*)(void *, struct ProcessInfo)")]
        public delegate* unmanaged[Cdecl]<void*, ProcessInfo, byte> func;
    }

    /// <summary>
    /// Pair of address and architecture used for callbacks
    /// </summary>
    public partial struct ModuleAddressInfo
    {
        [NativeTypeName("Address")]
        public ulong address;

        [NativeTypeName("struct ArchitectureIdent")]
        public ArchitectureIdent arch;
    }

    public unsafe partial struct Callback_c_void__ModuleAddressInfo
    {
        public void* context;

        [NativeTypeName("bool (*)(void *, struct ModuleAddressInfo)")]
        public delegate* unmanaged[Cdecl]<void*, ModuleAddressInfo, byte> func;
    }

    /// <summary>
    /// Module information structure
    /// </summary>
    public unsafe partial struct ModuleInfo
    {
        [NativeTypeName("Address")]
        public ulong address;

        [NativeTypeName("Address")]
        public ulong parent_process;

        [NativeTypeName("Address")]
        public ulong @base;

        [NativeTypeName("umem")]
        public ulong size;

        [NativeTypeName("ReprCString")]
        public sbyte* name;

        [NativeTypeName("ReprCString")]
        public sbyte* path;

        [NativeTypeName("struct ArchitectureIdent")]
        public ArchitectureIdent arch;
    }

    public unsafe partial struct Callback_c_void__ModuleInfo
    {
        public void* context;

        [NativeTypeName("bool (*)(void *, struct ModuleInfo)")]
        public delegate* unmanaged[Cdecl]<void*, ModuleInfo, byte> func;
    }

    /// <summary>
    /// Import information structure
    /// </summary>
    public unsafe partial struct ImportInfo
    {
        [NativeTypeName("ReprCString")]
        public sbyte* name;

        [NativeTypeName("umem")]
        public ulong offset;
    }

    public unsafe partial struct Callback_c_void__ImportInfo
    {
        public void* context;

        [NativeTypeName("bool (*)(void *, struct ImportInfo)")]
        public delegate* unmanaged[Cdecl]<void*, ImportInfo, byte> func;
    }

    /// <summary>
    /// Export information structure
    /// </summary>
    public unsafe partial struct ExportInfo
    {
        [NativeTypeName("ReprCString")]
        public sbyte* name;

        [NativeTypeName("umem")]
        public ulong offset;
    }

    public unsafe partial struct Callback_c_void__ExportInfo
    {
        public void* context;

        [NativeTypeName("bool (*)(void *, struct ExportInfo)")]
        public delegate* unmanaged[Cdecl]<void*, ExportInfo, byte> func;
    }

    /// <summary>
    /// Section information structure
    /// </summary>
    public unsafe partial struct SectionInfo
    {
        [NativeTypeName("ReprCString")]
        public sbyte* name;

        [NativeTypeName("Address")]
        public ulong @base;

        [NativeTypeName("umem")]
        public ulong size;
    }

    public unsafe partial struct Callback_c_void__SectionInfo
    {
        public void* context;

        [NativeTypeName("bool (*)(void *, struct SectionInfo)")]
        public delegate* unmanaged[Cdecl]<void*, SectionInfo, byte> func;
    }

    /// <summary>
    /// FFI-safe 3 element tuple.
    /// </summary>
    public partial struct CTup3_Address__umem__PageType
    {
        [NativeTypeName("Address")]
        public ulong _0;

        [NativeTypeName("umem")]
        public ulong _1;

        [NativeTypeName("PageType")]
        public byte _2;
    }

    public unsafe partial struct Callback_c_void__MemoryRange
    {
        public void* context;

        [NativeTypeName("bool (*)(void *, MemoryRange)")]
        public delegate* unmanaged[Cdecl]<void*, CTup3_Address__umem__PageType, byte> func;
    }

    /// <summary>
    /// FFI-safe 2 element tuple.
    /// </summary>
    public partial struct CTup2_Address__umem
    {
        [NativeTypeName("Address")]
        public ulong _0;

        [NativeTypeName("umem")]
        public ulong _1;
    }

    /// <summary>
    /// Wrapper around const slices.
    /// This is meant as a safe type to pass across the FFI boundary with similar semantics as regularslice. However, not all functionality is present, use the slice conversion functions.
    /// # Examples
    /// Simple conversion:
    /// ```use cglue::slice::CSliceRef;
    /// let arr = [0, 5, 3, 2];
    /// let cslice = CSliceRef::from(&arr[..]);
    /// let slice = cslice.as_slice();
    /// assert_eq!(&arr, slice);```
    /// </summary>
    public unsafe partial struct CSliceRef_VtopRange
    {
        [NativeTypeName("const VtopRange *")]
        public CTup2_Address__umem* data;

        [NativeTypeName("uintptr_t")]
        public nuint len;
    }

    /// <summary>
    /// Virtual page range information with physical mappings used for callbacks
    /// </summary>
    public partial struct VirtualTranslation
    {
        [NativeTypeName("Address")]
        public ulong in_virtual;

        [NativeTypeName("umem")]
        public ulong size;

        [NativeTypeName("struct PhysicalAddress")]
        public PhysicalAddress out_physical;
    }

    public unsafe partial struct Callback_c_void__VirtualTranslation
    {
        public void* context;

        [NativeTypeName("bool (*)(void *, struct VirtualTranslation)")]
        public delegate* unmanaged[Cdecl]<void*, VirtualTranslation, byte> func;
    }

    public partial struct VirtualTranslationFail
    {
        [NativeTypeName("Address")]
        public ulong from;

        [NativeTypeName("umem")]
        public ulong size;
    }

    public unsafe partial struct Callback_c_void__VirtualTranslationFail
    {
        public void* context;

        [NativeTypeName("bool (*)(void *, struct VirtualTranslationFail)")]
        public delegate* unmanaged[Cdecl]<void*, VirtualTranslationFail, byte> func;
    }

    /// <summary>
    /// A `Page` holds information about a memory page.
    /// More information about paging can be found [here](https://en.wikipedia.org/wiki/Paging).
    /// </summary>
    public partial struct Page
    {
        [NativeTypeName("PageType")]
        public byte page_type;

        [NativeTypeName("Address")]
        public ulong page_base;

        [NativeTypeName("umem")]
        public ulong page_size;
    }

    /// <summary>
    /// FFI-safe Option.
    /// This type is not really meant for general use, but rather as a last-resort conversion for typewrapping.
    /// Typical workflow would include temporarily converting into/from COption.
    /// </summary>
    public enum COption_Address_Tag
    {
        COption_Address_None_Address,
        COption_Address_Some_Address,
    }

    public partial struct COption_Address
    {
        public COption_Address_Tag tag;

        [NativeTypeName("COption_Address::(anonymous union at memflow.h:1533:5)")]
        public _Anonymous_e__Union Anonymous;

        public ref ulong some
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return ref MemoryMarshal.GetReference(MemoryMarshal.CreateSpan(ref Anonymous.Anonymous.some, 1));
            }
        }

        [StructLayout(LayoutKind.Explicit)]
        public partial struct _Anonymous_e__Union
        {
            [FieldOffset(0)]
            [NativeTypeName("COption_Address::(anonymous struct at memflow.h:1534:9)")]
            public _Anonymous_e__Struct Anonymous;

            public partial struct _Anonymous_e__Struct
            {
                [NativeTypeName("Address")]
                public ulong some;
            }
        }
    }

    /// <summary>
    /// Information block about OS
    /// This provides some basic information about the OS in question. `base`, and `size` may beomitted in some circumstances (lack of kernel, or privileges). But architecture should alwaysbe correct.
    /// </summary>
    public partial struct OsInfo
    {
        [NativeTypeName("Address")]
        public ulong @base;

        [NativeTypeName("umem")]
        public ulong size;

        [NativeTypeName("struct ArchitectureIdent")]
        public ArchitectureIdent arch;
    }

    /// <summary>
    /// CGlue vtable for trait OsInner.
    /// This virtual function table contains ABI-safe interface for the given trait.
    /// </summary>
    public unsafe partial struct OsInnerVtbl_OsInstanceContainer_CBox_c_void_____CArc_c_void
    {
        [NativeTypeName("int32_t (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *, AddressCallback)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, Callback_c_void__Address, int> process_address_list_callback;

        [NativeTypeName("int32_t (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *, ProcessInfoCallback)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, Callback_c_void__ProcessInfo, int> process_info_list_callback;

        [NativeTypeName("int32_t (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *, Address, struct ProcessInfo *)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, ulong, ProcessInfo*, int> process_info_by_address;

        [NativeTypeName("int32_t (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *, struct CSliceRef_u8, struct ProcessInfo *)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, CSliceRef_u8, ProcessInfo*, int> process_info_by_name;

        [NativeTypeName("int32_t (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *, Pid, struct ProcessInfo *)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, uint, ProcessInfo*, int> process_info_by_pid;

        [NativeTypeName("int32_t (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *, struct ProcessInfo, struct ProcessInstance_CBox_c_void_____CArc_c_void *)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, ProcessInfo, ProcessInstance_CBox_c_void_____CArc_c_void*, int> process_by_info;

        [NativeTypeName("int32_t (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void, struct ProcessInfo, struct IntoProcessInstance_CBox_c_void_____CArc_c_void *)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void, ProcessInfo, IntoProcessInstance_CBox_c_void_____CArc_c_void*, int> into_process_by_info;

        [NativeTypeName("int32_t (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *, Address, struct ProcessInstance_CBox_c_void_____CArc_c_void *)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, ulong, ProcessInstance_CBox_c_void_____CArc_c_void*, int> process_by_address;

        [NativeTypeName("int32_t (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *, struct CSliceRef_u8, struct ProcessInstance_CBox_c_void_____CArc_c_void *)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, CSliceRef_u8, ProcessInstance_CBox_c_void_____CArc_c_void*, int> process_by_name;

        [NativeTypeName("int32_t (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *, Pid, struct ProcessInstance_CBox_c_void_____CArc_c_void *)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, uint, ProcessInstance_CBox_c_void_____CArc_c_void*, int> process_by_pid;

        [NativeTypeName("int32_t (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void, Address, struct IntoProcessInstance_CBox_c_void_____CArc_c_void *)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void, ulong, IntoProcessInstance_CBox_c_void_____CArc_c_void*, int> into_process_by_address;

        [NativeTypeName("int32_t (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void, struct CSliceRef_u8, struct IntoProcessInstance_CBox_c_void_____CArc_c_void *)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void, CSliceRef_u8, IntoProcessInstance_CBox_c_void_____CArc_c_void*, int> into_process_by_name;

        [NativeTypeName("int32_t (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void, Pid, struct IntoProcessInstance_CBox_c_void_____CArc_c_void *)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void, uint, IntoProcessInstance_CBox_c_void_____CArc_c_void*, int> into_process_by_pid;

        [NativeTypeName("int32_t (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *, AddressCallback)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, Callback_c_void__Address, int> module_address_list_callback;

        [NativeTypeName("int32_t (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *, ModuleInfoCallback)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, Callback_c_void__ModuleInfo, int> module_list_callback;

        [NativeTypeName("int32_t (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *, Address, struct ModuleInfo *)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, ulong, ModuleInfo*, int> module_by_address;

        [NativeTypeName("int32_t (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *, struct CSliceRef_u8, struct ModuleInfo *)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, CSliceRef_u8, ModuleInfo*, int> module_by_name;

        [NativeTypeName("int32_t (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *, Address *)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, ulong*, int> primary_module_address;

        [NativeTypeName("int32_t (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *, struct ModuleInfo *)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, ModuleInfo*, int> primary_module;

        [NativeTypeName("int32_t (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *, const struct ModuleInfo *, ImportCallback)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, ModuleInfo*, Callback_c_void__ImportInfo, int> module_import_list_callback;

        [NativeTypeName("int32_t (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *, const struct ModuleInfo *, ExportCallback)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, ModuleInfo*, Callback_c_void__ExportInfo, int> module_export_list_callback;

        [NativeTypeName("int32_t (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *, const struct ModuleInfo *, SectionCallback)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, ModuleInfo*, Callback_c_void__SectionInfo, int> module_section_list_callback;

        [NativeTypeName("int32_t (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *, const struct ModuleInfo *, struct CSliceRef_u8, struct ImportInfo *)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, ModuleInfo*, CSliceRef_u8, ImportInfo*, int> module_import_by_name;

        [NativeTypeName("int32_t (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *, const struct ModuleInfo *, struct CSliceRef_u8, struct ExportInfo *)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, ModuleInfo*, CSliceRef_u8, ExportInfo*, int> module_export_by_name;

        [NativeTypeName("int32_t (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *, const struct ModuleInfo *, struct CSliceRef_u8, struct SectionInfo *)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, ModuleInfo*, CSliceRef_u8, SectionInfo*, int> module_section_by_name;

        [NativeTypeName("const struct OsInfo *(*)(const struct OsInstanceContainer_CBox_c_void_____CArc_c_void *)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, OsInfo*> info;
    }

    /// <summary>
    /// CGlue vtable for trait MemoryView.
    /// This virtual function table contains ABI-safe interface for the given trait.
    /// </summary>
    public unsafe partial struct MemoryViewVtbl_OsInstanceContainer_CBox_c_void_____CArc_c_void
    {
        [NativeTypeName("int32_t (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *, ReadRawMemOps)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, MemOps_ReadDataRaw__ReadData, int> read_raw_iter;

        [NativeTypeName("int32_t (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *, WriteRawMemOps)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, MemOps_WriteDataRaw__WriteData, int> write_raw_iter;

        [NativeTypeName("struct MemoryViewMetadata (*)(const struct OsInstanceContainer_CBox_c_void_____CArc_c_void *)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, MemoryViewMetadata> metadata;

        [NativeTypeName("int32_t (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *, struct CIterator_ReadData, ReadCallback *, ReadCallback *)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, CIterator_ReadData, Callback_c_void__ReadData*, Callback_c_void__ReadData*, int> read_iter;

        [NativeTypeName("int32_t (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *, struct CSliceMut_ReadData)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, CSliceMut_ReadData, int> read_raw_list;

        [NativeTypeName("int32_t (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *, Address, struct CSliceMut_u8)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, ulong, CSliceMut_u8, int> read_raw_into;

        [NativeTypeName("int32_t (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *, struct CIterator_WriteData, WriteCallback *, WriteCallback *)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, CIterator_WriteData, Callback_c_void__WriteData*, Callback_c_void__WriteData*, int> write_iter;

        [NativeTypeName("int32_t (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *, struct CSliceRef_WriteData)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, CSliceRef_WriteData, int> write_raw_list;

        [NativeTypeName("int32_t (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *, Address, struct CSliceRef_u8)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, ulong, CSliceRef_u8, int> write_raw;
    }

    /// <summary>
    /// Simple CGlue trait object container.
    /// This is the simplest form of container, represented by an instance, clone context, andtemporary return context.
    /// `instance` value usually is either a reference, or a mutable reference, or a `CBox`, whichcontains static reference to the instance, and a dedicated drop function for freeing resources.
    /// `context` is either `PhantomData` representing nothing, or typically a `CArc` that can becloned at will, reference counting some resource, like a `Library` for automatic unloading.
    /// `ret_tmp` is usually `PhantomData` representing nothing, unless the trait has functions thatreturn references to associated types, in which case space is reserved for wrapping structures.
    /// </summary>
    public partial struct CGlueObjContainer_CBox_c_void_____CArc_c_void_____KeyboardRetTmp_CArc_c_void
    {
        [NativeTypeName("struct CBox_c_void")]
        public CBox_c_void instance;

        public CArc_c_void context;
    }

    /// <summary>
    /// Simple CGlue trait object container.
    /// This is the simplest form of container, represented by an instance, clone context, andtemporary return context.
    /// `instance` value usually is either a reference, or a mutable reference, or a `CBox`, whichcontains static reference to the instance, and a dedicated drop function for freeing resources.
    /// `context` is either `PhantomData` representing nothing, or typically a `CArc` that can becloned at will, reference counting some resource, like a `Library` for automatic unloading.
    /// `ret_tmp` is usually `PhantomData` representing nothing, unless the trait has functions thatreturn references to associated types, in which case space is reserved for wrapping structures.
    /// </summary>
    public partial struct CGlueObjContainer_CBox_c_void_____CArc_c_void_____KeyboardStateRetTmp_CArc_c_void
    {
        [NativeTypeName("struct CBox_c_void")]
        public CBox_c_void instance;

        public CArc_c_void context;
    }

    /// <summary>
    /// CGlue vtable for trait KeyboardState.
    /// This virtual function table contains ABI-safe interface for the given trait.
    /// </summary>
    public unsafe partial struct KeyboardStateVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____KeyboardStateRetTmp_CArc_c_void
    {
        [NativeTypeName("bool (*)(const struct CGlueObjContainer_CBox_c_void_____CArc_c_void_____KeyboardStateRetTmp_CArc_c_void *, int32_t)")]
        public delegate* unmanaged[Cdecl]<CGlueObjContainer_CBox_c_void_____CArc_c_void_____KeyboardStateRetTmp_CArc_c_void*, int, byte> is_down;
    }

    /// <summary>
    /// Simple CGlue trait object.
    /// This is the simplest form of CGlue object, represented by a container and vtable for a singletrait.
    /// Container merely is a this pointer with some optional temporary return reference context.
    /// </summary>
    public unsafe partial struct CGlueTraitObj_CBox_c_void_____KeyboardStateVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____KeyboardStateRetTmp_CArc_c_void______________CArc_c_void_____KeyboardStateRetTmp_CArc_c_void
    {
        [NativeTypeName("const struct KeyboardStateVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____KeyboardStateRetTmp_CArc_c_void *")]
        public KeyboardStateVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____KeyboardStateRetTmp_CArc_c_void* vtbl;

        [NativeTypeName("struct CGlueObjContainer_CBox_c_void_____CArc_c_void_____KeyboardStateRetTmp_CArc_c_void")]
        public CGlueObjContainer_CBox_c_void_____CArc_c_void_____KeyboardStateRetTmp_CArc_c_void container;
    }

    /// <summary>
    /// CGlue vtable for trait Keyboard.
    /// This virtual function table contains ABI-safe interface for the given trait.
    /// </summary>
    public unsafe partial struct KeyboardVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____KeyboardRetTmp_CArc_c_void
    {
        [NativeTypeName("bool (*)(struct CGlueObjContainer_CBox_c_void_____CArc_c_void_____KeyboardRetTmp_CArc_c_void *, int32_t)")]
        public delegate* unmanaged[Cdecl]<CGlueObjContainer_CBox_c_void_____CArc_c_void_____KeyboardRetTmp_CArc_c_void*, int, byte> is_down;

        [NativeTypeName("void (*)(struct CGlueObjContainer_CBox_c_void_____CArc_c_void_____KeyboardRetTmp_CArc_c_void *, int32_t, bool)")]
        public delegate* unmanaged[Cdecl]<CGlueObjContainer_CBox_c_void_____CArc_c_void_____KeyboardRetTmp_CArc_c_void*, int, byte, void> set_down;

        [NativeTypeName("int32_t (*)(struct CGlueObjContainer_CBox_c_void_____CArc_c_void_____KeyboardRetTmp_CArc_c_void *, KeyboardStateBase_CBox_c_void_____CArc_c_void *)")]
        public delegate* unmanaged[Cdecl]<CGlueObjContainer_CBox_c_void_____CArc_c_void_____KeyboardRetTmp_CArc_c_void*, CGlueTraitObj_CBox_c_void_____KeyboardStateVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____KeyboardStateRetTmp_CArc_c_void______________CArc_c_void_____KeyboardStateRetTmp_CArc_c_void*, int> state;
    }

    /// <summary>
    /// Simple CGlue trait object.
    /// This is the simplest form of CGlue object, represented by a container and vtable for a singletrait.
    /// Container merely is a this pointer with some optional temporary return reference context.
    /// </summary>
    public unsafe partial struct CGlueTraitObj_CBox_c_void_____KeyboardVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____KeyboardRetTmp_CArc_c_void______________CArc_c_void_____KeyboardRetTmp_CArc_c_void
    {
        [NativeTypeName("const struct KeyboardVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____KeyboardRetTmp_CArc_c_void *")]
        public KeyboardVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____KeyboardRetTmp_CArc_c_void* vtbl;

        [NativeTypeName("struct CGlueObjContainer_CBox_c_void_____CArc_c_void_____KeyboardRetTmp_CArc_c_void")]
        public CGlueObjContainer_CBox_c_void_____CArc_c_void_____KeyboardRetTmp_CArc_c_void container;
    }

    public partial struct IntoKeyboardContainer_CBox_c_void_____CArc_c_void
    {
        [NativeTypeName("struct CBox_c_void")]
        public CBox_c_void instance;

        public CArc_c_void context;
    }

    /// <summary>
    /// CGlue vtable for trait Clone.
    /// This virtual function table contains ABI-safe interface for the given trait.
    /// </summary>
    public unsafe partial struct CloneVtbl_IntoKeyboardContainer_CBox_c_void_____CArc_c_void
    {
        [NativeTypeName("struct IntoKeyboardContainer_CBox_c_void_____CArc_c_void (*)(const struct IntoKeyboardContainer_CBox_c_void_____CArc_c_void *)")]
        public delegate* unmanaged[Cdecl]<IntoKeyboardContainer_CBox_c_void_____CArc_c_void*, IntoKeyboardContainer_CBox_c_void_____CArc_c_void> clone;
    }

    /// <summary>
    /// CGlue vtable for trait Keyboard.
    /// This virtual function table contains ABI-safe interface for the given trait.
    /// </summary>
    public unsafe partial struct KeyboardVtbl_IntoKeyboardContainer_CBox_c_void_____CArc_c_void
    {
        [NativeTypeName("bool (*)(struct IntoKeyboardContainer_CBox_c_void_____CArc_c_void *, int32_t)")]
        public delegate* unmanaged[Cdecl]<IntoKeyboardContainer_CBox_c_void_____CArc_c_void*, int, byte> is_down;

        [NativeTypeName("void (*)(struct IntoKeyboardContainer_CBox_c_void_____CArc_c_void *, int32_t, bool)")]
        public delegate* unmanaged[Cdecl]<IntoKeyboardContainer_CBox_c_void_____CArc_c_void*, int, byte, void> set_down;

        [NativeTypeName("int32_t (*)(struct IntoKeyboardContainer_CBox_c_void_____CArc_c_void *, KeyboardStateBase_CBox_c_void_____CArc_c_void *)")]
        public delegate* unmanaged[Cdecl]<IntoKeyboardContainer_CBox_c_void_____CArc_c_void*, CGlueTraitObj_CBox_c_void_____KeyboardStateVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____KeyboardStateRetTmp_CArc_c_void______________CArc_c_void_____KeyboardStateRetTmp_CArc_c_void*, int> state;
    }

    /// <summary>
    /// Trait group potentially implementing `:: cglue :: ext :: core :: clone :: Clone <> + Keyboard <>` traits.
    /// Optional traits are not implemented here, however. There are numerous conversionfunctions available for safely retrieving a concrete collection of traits.
    /// `check_impl_` functions allow to check if the object implements the wanted traits.
    /// `into_impl_` functions consume the object and produce a new final structure thatkeeps only the required information.
    /// `cast_impl_` functions merely check and transform the object into a type that canbe transformed back into `IntoKeyboard` without losing data.
    /// `as_ref_`, and `as_mut_` functions obtain references to safe objects, but do notperform any memory transformations either. They are the safest to use, becausethere is no risk of accidentally consuming the whole object.
    /// </summary>
    public unsafe partial struct IntoKeyboard_CBox_c_void_____CArc_c_void
    {
        [NativeTypeName("const struct CloneVtbl_IntoKeyboardContainer_CBox_c_void_____CArc_c_void *")]
        public CloneVtbl_IntoKeyboardContainer_CBox_c_void_____CArc_c_void* vtbl_clone;

        [NativeTypeName("const struct KeyboardVtbl_IntoKeyboardContainer_CBox_c_void_____CArc_c_void *")]
        public KeyboardVtbl_IntoKeyboardContainer_CBox_c_void_____CArc_c_void* vtbl_keyboard;

        [NativeTypeName("struct IntoKeyboardContainer_CBox_c_void_____CArc_c_void")]
        public IntoKeyboardContainer_CBox_c_void_____CArc_c_void container;
    }

    /// <summary>
    /// CGlue vtable for trait OsKeyboardInner.
    /// This virtual function table contains ABI-safe interface for the given trait.
    /// </summary>
    public unsafe partial struct OsKeyboardInnerVtbl_OsInstanceContainer_CBox_c_void_____CArc_c_void
    {
        [NativeTypeName("int32_t (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *, KeyboardBase_CBox_c_void_____CArc_c_void *)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, CGlueTraitObj_CBox_c_void_____KeyboardVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____KeyboardRetTmp_CArc_c_void______________CArc_c_void_____KeyboardRetTmp_CArc_c_void*, int> keyboard;

        [NativeTypeName("int32_t (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void, struct IntoKeyboard_CBox_c_void_____CArc_c_void *)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void, IntoKeyboard_CBox_c_void_____CArc_c_void*, int> into_keyboard;
    }

    /// <summary>
    /// CGlue vtable for trait PhysicalMemory.
    /// This virtual function table contains ABI-safe interface for the given trait.
    /// </summary>
    public unsafe partial struct PhysicalMemoryVtbl_OsInstanceContainer_CBox_c_void_____CArc_c_void
    {
        [NativeTypeName("int32_t (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *, PhysicalReadMemOps)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, MemOps_PhysicalReadData__ReadData, int> phys_read_raw_iter;

        [NativeTypeName("int32_t (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *, PhysicalWriteMemOps)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, MemOps_PhysicalWriteData__WriteData, int> phys_write_raw_iter;

        [NativeTypeName("struct PhysicalMemoryMetadata (*)(const struct OsInstanceContainer_CBox_c_void_____CArc_c_void *)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, PhysicalMemoryMetadata> metadata;

        [NativeTypeName("void (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *, struct CSliceRef_PhysicalMemoryMapping)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, CSliceRef_PhysicalMemoryMapping, void> set_mem_map;

        [NativeTypeName("MemoryViewBase_CBox_c_void_____CArc_c_void (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void, CGlueTraitObj_CBox_c_void_____MemoryViewVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void______________CArc_c_void_____MemoryViewRetTmp_CArc_c_void> into_phys_view;

        [NativeTypeName("MemoryViewBase_CBox_c_void_____CArc_c_void (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, CGlueTraitObj_CBox_c_void_____MemoryViewVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void______________CArc_c_void_____MemoryViewRetTmp_CArc_c_void> phys_view;
    }

    /// <summary>
    /// CGlue vtable for trait VirtualTranslate.
    /// This virtual function table contains ABI-safe interface for the given trait.
    /// </summary>
    public unsafe partial struct VirtualTranslateVtbl_OsInstanceContainer_CBox_c_void_____CArc_c_void
    {
        [NativeTypeName("void (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *, struct CSliceRef_VtopRange, VirtualTranslationCallback, VirtualTranslationFailCallback)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, CSliceRef_VtopRange, Callback_c_void__VirtualTranslation, Callback_c_void__VirtualTranslationFail, void> virt_to_phys_list;

        [NativeTypeName("void (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *, Address, Address, VirtualTranslationCallback)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, ulong, ulong, Callback_c_void__VirtualTranslation, void> virt_to_phys_range;

        [NativeTypeName("void (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *, Address, Address, VirtualTranslationCallback)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, ulong, ulong, Callback_c_void__VirtualTranslation, void> virt_translation_map_range;

        [NativeTypeName("void (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *, imem, Address, Address, MemoryRangeCallback)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, long, ulong, ulong, Callback_c_void__MemoryRange, void> virt_page_map_range;

        [NativeTypeName("int32_t (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *, Address, struct PhysicalAddress *)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, ulong, PhysicalAddress*, int> virt_to_phys;

        [NativeTypeName("int32_t (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *, Address, struct Page *)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, ulong, Page*, int> virt_page_info;

        [NativeTypeName("void (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *, VirtualTranslationCallback)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, Callback_c_void__VirtualTranslation, void> virt_translation_map;

        [NativeTypeName("struct COption_Address (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *, Address)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, ulong, COption_Address> phys_to_virt;

        [NativeTypeName("void (*)(struct OsInstanceContainer_CBox_c_void_____CArc_c_void *, imem, MemoryRangeCallback)")]
        public delegate* unmanaged[Cdecl]<OsInstanceContainer_CBox_c_void_____CArc_c_void*, long, Callback_c_void__MemoryRange, void> virt_page_map;
    }

    /// <summary>
    /// Trait group potentially implementing `:: cglue :: ext :: core :: clone :: Clone <> + for <'cglue_c > OsInner <'cglue_c, > + MemoryView <> + for <'cglue_c > OsKeyboardInner <'cglue_c, > + PhysicalMemory <> + VirtualTranslate <>` traits.
    /// Optional traits are not implemented here, however. There are numerous conversionfunctions available for safely retrieving a concrete collection of traits.
    /// `check_impl_` functions allow to check if the object implements the wanted traits.
    /// `into_impl_` functions consume the object and produce a new final structure thatkeeps only the required information.
    /// `cast_impl_` functions merely check and transform the object into a type that canbe transformed back into `OsInstance` without losing data.
    /// `as_ref_`, and `as_mut_` functions obtain references to safe objects, but do notperform any memory transformations either. They are the safest to use, becausethere is no risk of accidentally consuming the whole object.
    /// </summary>
    public unsafe partial struct OsInstance_CBox_c_void_____CArc_c_void
    {
        [NativeTypeName("const struct CloneVtbl_OsInstanceContainer_CBox_c_void_____CArc_c_void *")]
        public CloneVtbl_OsInstanceContainer_CBox_c_void_____CArc_c_void* vtbl_clone;

        [NativeTypeName("const struct OsInnerVtbl_OsInstanceContainer_CBox_c_void_____CArc_c_void *")]
        public OsInnerVtbl_OsInstanceContainer_CBox_c_void_____CArc_c_void* vtbl_osinner;

        [NativeTypeName("const struct MemoryViewVtbl_OsInstanceContainer_CBox_c_void_____CArc_c_void *")]
        public MemoryViewVtbl_OsInstanceContainer_CBox_c_void_____CArc_c_void* vtbl_memoryview;

        [NativeTypeName("const struct OsKeyboardInnerVtbl_OsInstanceContainer_CBox_c_void_____CArc_c_void *")]
        public OsKeyboardInnerVtbl_OsInstanceContainer_CBox_c_void_____CArc_c_void* vtbl_oskeyboardinner;

        [NativeTypeName("const struct PhysicalMemoryVtbl_OsInstanceContainer_CBox_c_void_____CArc_c_void *")]
        public PhysicalMemoryVtbl_OsInstanceContainer_CBox_c_void_____CArc_c_void* vtbl_physicalmemory;

        [NativeTypeName("const struct VirtualTranslateVtbl_OsInstanceContainer_CBox_c_void_____CArc_c_void *")]
        public VirtualTranslateVtbl_OsInstanceContainer_CBox_c_void_____CArc_c_void* vtbl_virtualtranslate;

        [NativeTypeName("struct OsInstanceContainer_CBox_c_void_____CArc_c_void")]
        public OsInstanceContainer_CBox_c_void_____CArc_c_void container;
    }

    public partial struct ProcessInstanceContainer_CBox_c_void_____CArc_c_void
    {
        [NativeTypeName("struct CBox_c_void")]
        public CBox_c_void instance;

        [NativeTypeName("struct CArc_c_void")]
        public CArc_c_void context;
    }

    /// <summary>
    /// CGlue vtable for trait MemoryView.
    /// This virtual function table contains ABI-safe interface for the given trait.
    /// </summary>
    public unsafe partial struct MemoryViewVtbl_ProcessInstanceContainer_CBox_c_void_____CArc_c_void
    {
        [NativeTypeName("int32_t (*)(struct ProcessInstanceContainer_CBox_c_void_____CArc_c_void *, ReadRawMemOps)")]
        public delegate* unmanaged[Cdecl]<ProcessInstanceContainer_CBox_c_void_____CArc_c_void*, MemOps_ReadDataRaw__ReadData, int> read_raw_iter;

        [NativeTypeName("int32_t (*)(struct ProcessInstanceContainer_CBox_c_void_____CArc_c_void *, WriteRawMemOps)")]
        public delegate* unmanaged[Cdecl]<ProcessInstanceContainer_CBox_c_void_____CArc_c_void*, MemOps_WriteDataRaw__WriteData, int> write_raw_iter;

        [NativeTypeName("struct MemoryViewMetadata (*)(const struct ProcessInstanceContainer_CBox_c_void_____CArc_c_void *)")]
        public delegate* unmanaged[Cdecl]<ProcessInstanceContainer_CBox_c_void_____CArc_c_void*, MemoryViewMetadata> metadata;

        [NativeTypeName("int32_t (*)(struct ProcessInstanceContainer_CBox_c_void_____CArc_c_void *, struct CIterator_ReadData, ReadCallback *, ReadCallback *)")]
        public delegate* unmanaged[Cdecl]<ProcessInstanceContainer_CBox_c_void_____CArc_c_void*, CIterator_ReadData, Callback_c_void__ReadData*, Callback_c_void__ReadData*, int> read_iter;

        [NativeTypeName("int32_t (*)(struct ProcessInstanceContainer_CBox_c_void_____CArc_c_void *, struct CSliceMut_ReadData)")]
        public delegate* unmanaged[Cdecl]<ProcessInstanceContainer_CBox_c_void_____CArc_c_void*, CSliceMut_ReadData, int> read_raw_list;

        [NativeTypeName("int32_t (*)(struct ProcessInstanceContainer_CBox_c_void_____CArc_c_void *, Address, struct CSliceMut_u8)")]
        public delegate* unmanaged[Cdecl]<ProcessInstanceContainer_CBox_c_void_____CArc_c_void*, ulong, CSliceMut_u8, int> read_raw_into;

        [NativeTypeName("int32_t (*)(struct ProcessInstanceContainer_CBox_c_void_____CArc_c_void *, struct CIterator_WriteData, WriteCallback *, WriteCallback *)")]
        public delegate* unmanaged[Cdecl]<ProcessInstanceContainer_CBox_c_void_____CArc_c_void*, CIterator_WriteData, Callback_c_void__WriteData*, Callback_c_void__WriteData*, int> write_iter;

        [NativeTypeName("int32_t (*)(struct ProcessInstanceContainer_CBox_c_void_____CArc_c_void *, struct CSliceRef_WriteData)")]
        public delegate* unmanaged[Cdecl]<ProcessInstanceContainer_CBox_c_void_____CArc_c_void*, CSliceRef_WriteData, int> write_raw_list;

        [NativeTypeName("int32_t (*)(struct ProcessInstanceContainer_CBox_c_void_____CArc_c_void *, Address, struct CSliceRef_u8)")]
        public delegate* unmanaged[Cdecl]<ProcessInstanceContainer_CBox_c_void_____CArc_c_void*, ulong, CSliceRef_u8, int> write_raw;
    }

    /// <summary>
    /// CGlue vtable for trait Process.
    /// This virtual function table contains ABI-safe interface for the given trait.
    /// </summary>
    public unsafe partial struct ProcessVtbl_ProcessInstanceContainer_CBox_c_void_____CArc_c_void
    {
        [NativeTypeName("struct ProcessState (*)(struct ProcessInstanceContainer_CBox_c_void_____CArc_c_void *)")]
        public delegate* unmanaged[Cdecl]<ProcessInstanceContainer_CBox_c_void_____CArc_c_void*, ProcessState> state;

        [NativeTypeName("int32_t (*)(struct ProcessInstanceContainer_CBox_c_void_____CArc_c_void *, const struct ArchitectureIdent *, ModuleAddressCallback)")]
        public delegate* unmanaged[Cdecl]<ProcessInstanceContainer_CBox_c_void_____CArc_c_void*, ArchitectureIdent*, Callback_c_void__ModuleAddressInfo, int> module_address_list_callback;

        [NativeTypeName("int32_t (*)(struct ProcessInstanceContainer_CBox_c_void_____CArc_c_void *, const struct ArchitectureIdent *, ModuleInfoCallback)")]
        public delegate* unmanaged[Cdecl]<ProcessInstanceContainer_CBox_c_void_____CArc_c_void*, ArchitectureIdent*, Callback_c_void__ModuleInfo, int> module_list_callback;

        [NativeTypeName("int32_t (*)(struct ProcessInstanceContainer_CBox_c_void_____CArc_c_void *, Address, struct ArchitectureIdent, struct ModuleInfo *)")]
        public delegate* unmanaged[Cdecl]<ProcessInstanceContainer_CBox_c_void_____CArc_c_void*, ulong, ArchitectureIdent, ModuleInfo*, int> module_by_address;

        [NativeTypeName("int32_t (*)(struct ProcessInstanceContainer_CBox_c_void_____CArc_c_void *, struct CSliceRef_u8, const struct ArchitectureIdent *, struct ModuleInfo *)")]
        public delegate* unmanaged[Cdecl]<ProcessInstanceContainer_CBox_c_void_____CArc_c_void*, CSliceRef_u8, ArchitectureIdent*, ModuleInfo*, int> module_by_name_arch;

        [NativeTypeName("int32_t (*)(struct ProcessInstanceContainer_CBox_c_void_____CArc_c_void *, struct CSliceRef_u8, struct ModuleInfo *)")]
        public delegate* unmanaged[Cdecl]<ProcessInstanceContainer_CBox_c_void_____CArc_c_void*, CSliceRef_u8, ModuleInfo*, int> module_by_name;

        [NativeTypeName("int32_t (*)(struct ProcessInstanceContainer_CBox_c_void_____CArc_c_void *, Address *)")]
        public delegate* unmanaged[Cdecl]<ProcessInstanceContainer_CBox_c_void_____CArc_c_void*, ulong*, int> primary_module_address;

        [NativeTypeName("int32_t (*)(struct ProcessInstanceContainer_CBox_c_void_____CArc_c_void *, struct ModuleInfo *)")]
        public delegate* unmanaged[Cdecl]<ProcessInstanceContainer_CBox_c_void_____CArc_c_void*, ModuleInfo*, int> primary_module;

        [NativeTypeName("int32_t (*)(struct ProcessInstanceContainer_CBox_c_void_____CArc_c_void *, const struct ModuleInfo *, ImportCallback)")]
        public delegate* unmanaged[Cdecl]<ProcessInstanceContainer_CBox_c_void_____CArc_c_void*, ModuleInfo*, Callback_c_void__ImportInfo, int> module_import_list_callback;

        [NativeTypeName("int32_t (*)(struct ProcessInstanceContainer_CBox_c_void_____CArc_c_void *, const struct ModuleInfo *, ExportCallback)")]
        public delegate* unmanaged[Cdecl]<ProcessInstanceContainer_CBox_c_void_____CArc_c_void*, ModuleInfo*, Callback_c_void__ExportInfo, int> module_export_list_callback;

        [NativeTypeName("int32_t (*)(struct ProcessInstanceContainer_CBox_c_void_____CArc_c_void *, const struct ModuleInfo *, SectionCallback)")]
        public delegate* unmanaged[Cdecl]<ProcessInstanceContainer_CBox_c_void_____CArc_c_void*, ModuleInfo*, Callback_c_void__SectionInfo, int> module_section_list_callback;

        [NativeTypeName("int32_t (*)(struct ProcessInstanceContainer_CBox_c_void_____CArc_c_void *, const struct ModuleInfo *, struct CSliceRef_u8, struct ImportInfo *)")]
        public delegate* unmanaged[Cdecl]<ProcessInstanceContainer_CBox_c_void_____CArc_c_void*, ModuleInfo*, CSliceRef_u8, ImportInfo*, int> module_import_by_name;

        [NativeTypeName("int32_t (*)(struct ProcessInstanceContainer_CBox_c_void_____CArc_c_void *, const struct ModuleInfo *, struct CSliceRef_u8, struct ExportInfo *)")]
        public delegate* unmanaged[Cdecl]<ProcessInstanceContainer_CBox_c_void_____CArc_c_void*, ModuleInfo*, CSliceRef_u8, ExportInfo*, int> module_export_by_name;

        [NativeTypeName("int32_t (*)(struct ProcessInstanceContainer_CBox_c_void_____CArc_c_void *, const struct ModuleInfo *, struct CSliceRef_u8, struct SectionInfo *)")]
        public delegate* unmanaged[Cdecl]<ProcessInstanceContainer_CBox_c_void_____CArc_c_void*, ModuleInfo*, CSliceRef_u8, SectionInfo*, int> module_section_by_name;

        [NativeTypeName("const struct ProcessInfo *(*)(const struct ProcessInstanceContainer_CBox_c_void_____CArc_c_void *)")]
        public delegate* unmanaged[Cdecl]<ProcessInstanceContainer_CBox_c_void_____CArc_c_void*, ProcessInfo*> info;

        [NativeTypeName("void (*)(struct ProcessInstanceContainer_CBox_c_void_____CArc_c_void *, imem, Address, Address, MemoryRangeCallback)")]
        public delegate* unmanaged[Cdecl]<ProcessInstanceContainer_CBox_c_void_____CArc_c_void*, long, ulong, ulong, Callback_c_void__MemoryRange, void> mapped_mem_range;

        [NativeTypeName("void (*)(struct ProcessInstanceContainer_CBox_c_void_____CArc_c_void *, imem, MemoryRangeCallback)")]
        public delegate* unmanaged[Cdecl]<ProcessInstanceContainer_CBox_c_void_____CArc_c_void*, long, Callback_c_void__MemoryRange, void> mapped_mem;
    }

    /// <summary>
    /// CGlue vtable for trait VirtualTranslate.
    /// This virtual function table contains ABI-safe interface for the given trait.
    /// </summary>
    public unsafe partial struct VirtualTranslateVtbl_ProcessInstanceContainer_CBox_c_void_____CArc_c_void
    {
        [NativeTypeName("void (*)(struct ProcessInstanceContainer_CBox_c_void_____CArc_c_void *, struct CSliceRef_VtopRange, VirtualTranslationCallback, VirtualTranslationFailCallback)")]
        public delegate* unmanaged[Cdecl]<ProcessInstanceContainer_CBox_c_void_____CArc_c_void*, CSliceRef_VtopRange, Callback_c_void__VirtualTranslation, Callback_c_void__VirtualTranslationFail, void> virt_to_phys_list;

        [NativeTypeName("void (*)(struct ProcessInstanceContainer_CBox_c_void_____CArc_c_void *, Address, Address, VirtualTranslationCallback)")]
        public delegate* unmanaged[Cdecl]<ProcessInstanceContainer_CBox_c_void_____CArc_c_void*, ulong, ulong, Callback_c_void__VirtualTranslation, void> virt_to_phys_range;

        [NativeTypeName("void (*)(struct ProcessInstanceContainer_CBox_c_void_____CArc_c_void *, Address, Address, VirtualTranslationCallback)")]
        public delegate* unmanaged[Cdecl]<ProcessInstanceContainer_CBox_c_void_____CArc_c_void*, ulong, ulong, Callback_c_void__VirtualTranslation, void> virt_translation_map_range;

        [NativeTypeName("void (*)(struct ProcessInstanceContainer_CBox_c_void_____CArc_c_void *, imem, Address, Address, MemoryRangeCallback)")]
        public delegate* unmanaged[Cdecl]<ProcessInstanceContainer_CBox_c_void_____CArc_c_void*, long, ulong, ulong, Callback_c_void__MemoryRange, void> virt_page_map_range;

        [NativeTypeName("int32_t (*)(struct ProcessInstanceContainer_CBox_c_void_____CArc_c_void *, Address, struct PhysicalAddress *)")]
        public delegate* unmanaged[Cdecl]<ProcessInstanceContainer_CBox_c_void_____CArc_c_void*, ulong, PhysicalAddress*, int> virt_to_phys;

        [NativeTypeName("int32_t (*)(struct ProcessInstanceContainer_CBox_c_void_____CArc_c_void *, Address, struct Page *)")]
        public delegate* unmanaged[Cdecl]<ProcessInstanceContainer_CBox_c_void_____CArc_c_void*, ulong, Page*, int> virt_page_info;

        [NativeTypeName("void (*)(struct ProcessInstanceContainer_CBox_c_void_____CArc_c_void *, VirtualTranslationCallback)")]
        public delegate* unmanaged[Cdecl]<ProcessInstanceContainer_CBox_c_void_____CArc_c_void*, Callback_c_void__VirtualTranslation, void> virt_translation_map;

        [NativeTypeName("struct COption_Address (*)(struct ProcessInstanceContainer_CBox_c_void_____CArc_c_void *, Address)")]
        public delegate* unmanaged[Cdecl]<ProcessInstanceContainer_CBox_c_void_____CArc_c_void*, ulong, COption_Address> phys_to_virt;

        [NativeTypeName("void (*)(struct ProcessInstanceContainer_CBox_c_void_____CArc_c_void *, imem, MemoryRangeCallback)")]
        public delegate* unmanaged[Cdecl]<ProcessInstanceContainer_CBox_c_void_____CArc_c_void*, long, Callback_c_void__MemoryRange, void> virt_page_map;
    }

    /// <summary>
    /// Trait group potentially implementing `MemoryView <> + Process <> + VirtualTranslate <>` traits.
    /// Optional traits are not implemented here, however. There are numerous conversionfunctions available for safely retrieving a concrete collection of traits.
    /// `check_impl_` functions allow to check if the object implements the wanted traits.
    /// `into_impl_` functions consume the object and produce a new final structure thatkeeps only the required information.
    /// `cast_impl_` functions merely check and transform the object into a type that canbe transformed back into `ProcessInstance` without losing data.
    /// `as_ref_`, and `as_mut_` functions obtain references to safe objects, but do notperform any memory transformations either. They are the safest to use, becausethere is no risk of accidentally consuming the whole object.
    /// </summary>
    public unsafe partial struct ProcessInstance_CBox_c_void_____CArc_c_void
    {
        [NativeTypeName("const struct MemoryViewVtbl_ProcessInstanceContainer_CBox_c_void_____CArc_c_void *")]
        public MemoryViewVtbl_ProcessInstanceContainer_CBox_c_void_____CArc_c_void* vtbl_memoryview;

        [NativeTypeName("const struct ProcessVtbl_ProcessInstanceContainer_CBox_c_void_____CArc_c_void *")]
        public ProcessVtbl_ProcessInstanceContainer_CBox_c_void_____CArc_c_void* vtbl_process;

        [NativeTypeName("const struct VirtualTranslateVtbl_ProcessInstanceContainer_CBox_c_void_____CArc_c_void *")]
        public VirtualTranslateVtbl_ProcessInstanceContainer_CBox_c_void_____CArc_c_void* vtbl_virtualtranslate;

        [NativeTypeName("struct ProcessInstanceContainer_CBox_c_void_____CArc_c_void")]
        public ProcessInstanceContainer_CBox_c_void_____CArc_c_void container;
    }

    public partial struct IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void
    {
        [NativeTypeName("struct CBox_c_void")]
        public CBox_c_void instance;

        [NativeTypeName("struct CArc_c_void")]
        public CArc_c_void context;
    }

    /// <summary>
    /// CGlue vtable for trait Clone.
    /// This virtual function table contains ABI-safe interface for the given trait.
    /// </summary>
    public unsafe partial struct CloneVtbl_IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void
    {
        [NativeTypeName("struct IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void (*)(const struct IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void *)")]
        public delegate* unmanaged[Cdecl]<IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void*, IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void> clone;
    }

    /// <summary>
    /// CGlue vtable for trait MemoryView.
    /// This virtual function table contains ABI-safe interface for the given trait.
    /// </summary>
    public unsafe partial struct MemoryViewVtbl_IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void
    {
        [NativeTypeName("int32_t (*)(struct IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void *, ReadRawMemOps)")]
        public delegate* unmanaged[Cdecl]<IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void*, MemOps_ReadDataRaw__ReadData, int> read_raw_iter;

        [NativeTypeName("int32_t (*)(struct IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void *, WriteRawMemOps)")]
        public delegate* unmanaged[Cdecl]<IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void*, MemOps_WriteDataRaw__WriteData, int> write_raw_iter;

        [NativeTypeName("struct MemoryViewMetadata (*)(const struct IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void *)")]
        public delegate* unmanaged[Cdecl]<IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void*, MemoryViewMetadata> metadata;

        [NativeTypeName("int32_t (*)(struct IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void *, struct CIterator_ReadData, ReadCallback *, ReadCallback *)")]
        public delegate* unmanaged[Cdecl]<IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void*, CIterator_ReadData, Callback_c_void__ReadData*, Callback_c_void__ReadData*, int> read_iter;

        [NativeTypeName("int32_t (*)(struct IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void *, struct CSliceMut_ReadData)")]
        public delegate* unmanaged[Cdecl]<IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void*, CSliceMut_ReadData, int> read_raw_list;

        [NativeTypeName("int32_t (*)(struct IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void *, Address, struct CSliceMut_u8)")]
        public delegate* unmanaged[Cdecl]<IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void*, ulong, CSliceMut_u8, int> read_raw_into;

        [NativeTypeName("int32_t (*)(struct IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void *, struct CIterator_WriteData, WriteCallback *, WriteCallback *)")]
        public delegate* unmanaged[Cdecl]<IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void*, CIterator_WriteData, Callback_c_void__WriteData*, Callback_c_void__WriteData*, int> write_iter;

        [NativeTypeName("int32_t (*)(struct IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void *, struct CSliceRef_WriteData)")]
        public delegate* unmanaged[Cdecl]<IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void*, CSliceRef_WriteData, int> write_raw_list;

        [NativeTypeName("int32_t (*)(struct IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void *, Address, struct CSliceRef_u8)")]
        public delegate* unmanaged[Cdecl]<IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void*, ulong, CSliceRef_u8, int> write_raw;
    }

    /// <summary>
    /// CGlue vtable for trait Process.
    /// This virtual function table contains ABI-safe interface for the given trait.
    /// </summary>
    public unsafe partial struct ProcessVtbl_IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void
    {
        [NativeTypeName("struct ProcessState (*)(struct IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void *)")]
        public delegate* unmanaged[Cdecl]<IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void*, ProcessState> state;

        [NativeTypeName("int32_t (*)(struct IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void *, const struct ArchitectureIdent *, ModuleAddressCallback)")]
        public delegate* unmanaged[Cdecl]<IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void*, ArchitectureIdent*, Callback_c_void__ModuleAddressInfo, int> module_address_list_callback;

        [NativeTypeName("int32_t (*)(struct IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void *, const struct ArchitectureIdent *, ModuleInfoCallback)")]
        public delegate* unmanaged[Cdecl]<IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void*, ArchitectureIdent*, Callback_c_void__ModuleInfo, int> module_list_callback;

        [NativeTypeName("int32_t (*)(struct IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void *, Address, struct ArchitectureIdent, struct ModuleInfo *)")]
        public delegate* unmanaged[Cdecl]<IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void*, ulong, ArchitectureIdent, ModuleInfo*, int> module_by_address;

        [NativeTypeName("int32_t (*)(struct IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void *, struct CSliceRef_u8, const struct ArchitectureIdent *, struct ModuleInfo *)")]
        public delegate* unmanaged[Cdecl]<IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void*, CSliceRef_u8, ArchitectureIdent*, ModuleInfo*, int> module_by_name_arch;

        [NativeTypeName("int32_t (*)(struct IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void *, struct CSliceRef_u8, struct ModuleInfo *)")]
        public delegate* unmanaged[Cdecl]<IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void*, CSliceRef_u8, ModuleInfo*, int> module_by_name;

        [NativeTypeName("int32_t (*)(struct IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void *, Address *)")]
        public delegate* unmanaged[Cdecl]<IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void*, ulong*, int> primary_module_address;

        [NativeTypeName("int32_t (*)(struct IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void *, struct ModuleInfo *)")]
        public delegate* unmanaged[Cdecl]<IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void*, ModuleInfo*, int> primary_module;

        [NativeTypeName("int32_t (*)(struct IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void *, const struct ModuleInfo *, ImportCallback)")]
        public delegate* unmanaged[Cdecl]<IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void*, ModuleInfo*, Callback_c_void__ImportInfo, int> module_import_list_callback;

        [NativeTypeName("int32_t (*)(struct IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void *, const struct ModuleInfo *, ExportCallback)")]
        public delegate* unmanaged[Cdecl]<IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void*, ModuleInfo*, Callback_c_void__ExportInfo, int> module_export_list_callback;

        [NativeTypeName("int32_t (*)(struct IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void *, const struct ModuleInfo *, SectionCallback)")]
        public delegate* unmanaged[Cdecl]<IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void*, ModuleInfo*, Callback_c_void__SectionInfo, int> module_section_list_callback;

        [NativeTypeName("int32_t (*)(struct IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void *, const struct ModuleInfo *, struct CSliceRef_u8, struct ImportInfo *)")]
        public delegate* unmanaged[Cdecl]<IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void*, ModuleInfo*, CSliceRef_u8, ImportInfo*, int> module_import_by_name;

        [NativeTypeName("int32_t (*)(struct IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void *, const struct ModuleInfo *, struct CSliceRef_u8, struct ExportInfo *)")]
        public delegate* unmanaged[Cdecl]<IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void*, ModuleInfo*, CSliceRef_u8, ExportInfo*, int> module_export_by_name;

        [NativeTypeName("int32_t (*)(struct IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void *, const struct ModuleInfo *, struct CSliceRef_u8, struct SectionInfo *)")]
        public delegate* unmanaged[Cdecl]<IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void*, ModuleInfo*, CSliceRef_u8, SectionInfo*, int> module_section_by_name;

        [NativeTypeName("const struct ProcessInfo *(*)(const struct IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void *)")]
        public delegate* unmanaged[Cdecl]<IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void*, ProcessInfo*> info;

        [NativeTypeName("void (*)(struct IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void *, imem, Address, Address, MemoryRangeCallback)")]
        public delegate* unmanaged[Cdecl]<IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void*, long, ulong, ulong, Callback_c_void__MemoryRange, void> mapped_mem_range;

        [NativeTypeName("void (*)(struct IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void *, imem, MemoryRangeCallback)")]
        public delegate* unmanaged[Cdecl]<IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void*, long, Callback_c_void__MemoryRange, void> mapped_mem;
    }

    /// <summary>
    /// CGlue vtable for trait VirtualTranslate.
    /// This virtual function table contains ABI-safe interface for the given trait.
    /// </summary>
    public unsafe partial struct VirtualTranslateVtbl_IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void
    {
        [NativeTypeName("void (*)(struct IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void *, struct CSliceRef_VtopRange, VirtualTranslationCallback, VirtualTranslationFailCallback)")]
        public delegate* unmanaged[Cdecl]<IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void*, CSliceRef_VtopRange, Callback_c_void__VirtualTranslation, Callback_c_void__VirtualTranslationFail, void> virt_to_phys_list;

        [NativeTypeName("void (*)(struct IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void *, Address, Address, VirtualTranslationCallback)")]
        public delegate* unmanaged[Cdecl]<IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void*, ulong, ulong, Callback_c_void__VirtualTranslation, void> virt_to_phys_range;

        [NativeTypeName("void (*)(struct IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void *, Address, Address, VirtualTranslationCallback)")]
        public delegate* unmanaged[Cdecl]<IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void*, ulong, ulong, Callback_c_void__VirtualTranslation, void> virt_translation_map_range;

        [NativeTypeName("void (*)(struct IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void *, imem, Address, Address, MemoryRangeCallback)")]
        public delegate* unmanaged[Cdecl]<IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void*, long, ulong, ulong, Callback_c_void__MemoryRange, void> virt_page_map_range;

        [NativeTypeName("int32_t (*)(struct IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void *, Address, struct PhysicalAddress *)")]
        public delegate* unmanaged[Cdecl]<IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void*, ulong, PhysicalAddress*, int> virt_to_phys;

        [NativeTypeName("int32_t (*)(struct IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void *, Address, struct Page *)")]
        public delegate* unmanaged[Cdecl]<IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void*, ulong, Page*, int> virt_page_info;

        [NativeTypeName("void (*)(struct IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void *, VirtualTranslationCallback)")]
        public delegate* unmanaged[Cdecl]<IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void*, Callback_c_void__VirtualTranslation, void> virt_translation_map;

        [NativeTypeName("struct COption_Address (*)(struct IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void *, Address)")]
        public delegate* unmanaged[Cdecl]<IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void*, ulong, COption_Address> phys_to_virt;

        [NativeTypeName("void (*)(struct IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void *, imem, MemoryRangeCallback)")]
        public delegate* unmanaged[Cdecl]<IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void*, long, Callback_c_void__MemoryRange, void> virt_page_map;
    }

    /// <summary>
    /// Trait group potentially implementing `:: cglue :: ext :: core :: clone :: Clone <> + MemoryView <> + Process <> + VirtualTranslate <>` traits.
    /// Optional traits are not implemented here, however. There are numerous conversionfunctions available for safely retrieving a concrete collection of traits.
    /// `check_impl_` functions allow to check if the object implements the wanted traits.
    /// `into_impl_` functions consume the object and produce a new final structure thatkeeps only the required information.
    /// `cast_impl_` functions merely check and transform the object into a type that canbe transformed back into `IntoProcessInstance` without losing data.
    /// `as_ref_`, and `as_mut_` functions obtain references to safe objects, but do notperform any memory transformations either. They are the safest to use, becausethere is no risk of accidentally consuming the whole object.
    /// </summary>
    public unsafe partial struct IntoProcessInstance_CBox_c_void_____CArc_c_void
    {
        [NativeTypeName("const struct CloneVtbl_IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void *")]
        public CloneVtbl_IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void* vtbl_clone;

        [NativeTypeName("const struct MemoryViewVtbl_IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void *")]
        public MemoryViewVtbl_IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void* vtbl_memoryview;

        [NativeTypeName("const struct ProcessVtbl_IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void *")]
        public ProcessVtbl_IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void* vtbl_process;

        [NativeTypeName("const struct VirtualTranslateVtbl_IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void *")]
        public VirtualTranslateVtbl_IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void* vtbl_virtualtranslate;

        [NativeTypeName("struct IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void")]
        public IntoProcessInstanceContainer_CBox_c_void_____CArc_c_void container;
    }

    /// <summary>
    /// Simple CGlue trait object container.
    /// This is the simplest form of container, represented by an instance, clone context, andtemporary return context.
    /// `instance` value usually is either a reference, or a mutable reference, or a `CBox`, whichcontains static reference to the instance, and a dedicated drop function for freeing resources.
    /// `context` is either `PhantomData` representing nothing, or typically a `CArc` that can becloned at will, reference counting some resource, like a `Library` for automatic unloading.
    /// `ret_tmp` is usually `PhantomData` representing nothing, unless the trait has functions thatreturn references to associated types, in which case space is reserved for wrapping structures.
    /// </summary>
    public partial struct CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void
    {
        [NativeTypeName("struct CBox_c_void")]
        public CBox_c_void instance;

        [NativeTypeName("struct CArc_c_void")]
        public CArc_c_void context;
    }

    /// <summary>
    /// CGlue vtable for trait MemoryView.
    /// This virtual function table contains ABI-safe interface for the given trait.
    /// </summary>
    public unsafe partial struct MemoryViewVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void
    {
        [NativeTypeName("int32_t (*)(struct CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void *, ReadRawMemOps)")]
        public delegate* unmanaged[Cdecl]<CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void*, MemOps_ReadDataRaw__ReadData, int> read_raw_iter;

        [NativeTypeName("int32_t (*)(struct CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void *, WriteRawMemOps)")]
        public delegate* unmanaged[Cdecl]<CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void*, MemOps_WriteDataRaw__WriteData, int> write_raw_iter;

        [NativeTypeName("struct MemoryViewMetadata (*)(const struct CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void *)")]
        public delegate* unmanaged[Cdecl]<CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void*, MemoryViewMetadata> metadata;

        [NativeTypeName("int32_t (*)(struct CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void *, struct CIterator_ReadData, ReadCallback *, ReadCallback *)")]
        public delegate* unmanaged[Cdecl]<CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void*, CIterator_ReadData, Callback_c_void__ReadData*, Callback_c_void__ReadData*, int> read_iter;

        [NativeTypeName("int32_t (*)(struct CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void *, struct CSliceMut_ReadData)")]
        public delegate* unmanaged[Cdecl]<CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void*, CSliceMut_ReadData, int> read_raw_list;

        [NativeTypeName("int32_t (*)(struct CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void *, Address, struct CSliceMut_u8)")]
        public delegate* unmanaged[Cdecl]<CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void*, ulong, CSliceMut_u8, int> read_raw_into;

        [NativeTypeName("int32_t (*)(struct CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void *, struct CIterator_WriteData, WriteCallback *, WriteCallback *)")]
        public delegate* unmanaged[Cdecl]<CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void*, CIterator_WriteData, Callback_c_void__WriteData*, Callback_c_void__WriteData*, int> write_iter;

        [NativeTypeName("int32_t (*)(struct CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void *, struct CSliceRef_WriteData)")]
        public delegate* unmanaged[Cdecl]<CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void*, CSliceRef_WriteData, int> write_raw_list;

        [NativeTypeName("int32_t (*)(struct CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void *, Address, struct CSliceRef_u8)")]
        public delegate* unmanaged[Cdecl]<CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void*, ulong, CSliceRef_u8, int> write_raw;
    }

    /// <summary>
    /// Simple CGlue trait object.
    /// This is the simplest form of CGlue object, represented by a container and vtable for a singletrait.
    /// Container merely is a this pointer with some optional temporary return reference context.
    /// </summary>
    public unsafe partial struct CGlueTraitObj_CBox_c_void_____MemoryViewVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void______________CArc_c_void_____MemoryViewRetTmp_CArc_c_void
    {
        [NativeTypeName("const struct MemoryViewVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void *")]
        public MemoryViewVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void* vtbl;

        [NativeTypeName("struct CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void")]
        public CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void container;
    }

    /// <summary>
    /// CGlue vtable for trait PhysicalMemory.
    /// This virtual function table contains ABI-safe interface for the given trait.
    /// </summary>
    public unsafe partial struct PhysicalMemoryVtbl_ConnectorInstanceContainer_CBox_c_void_____CArc_c_void
    {
        [NativeTypeName("int32_t (*)(struct ConnectorInstanceContainer_CBox_c_void_____CArc_c_void *, PhysicalReadMemOps)")]
        public delegate* unmanaged[Cdecl]<ConnectorInstanceContainer_CBox_c_void_____CArc_c_void*, MemOps_PhysicalReadData__ReadData, int> phys_read_raw_iter;

        [NativeTypeName("int32_t (*)(struct ConnectorInstanceContainer_CBox_c_void_____CArc_c_void *, PhysicalWriteMemOps)")]
        public delegate* unmanaged[Cdecl]<ConnectorInstanceContainer_CBox_c_void_____CArc_c_void*, MemOps_PhysicalWriteData__WriteData, int> phys_write_raw_iter;

        [NativeTypeName("struct PhysicalMemoryMetadata (*)(const struct ConnectorInstanceContainer_CBox_c_void_____CArc_c_void *)")]
        public delegate* unmanaged[Cdecl]<ConnectorInstanceContainer_CBox_c_void_____CArc_c_void*, PhysicalMemoryMetadata> metadata;

        [NativeTypeName("void (*)(struct ConnectorInstanceContainer_CBox_c_void_____CArc_c_void *, struct CSliceRef_PhysicalMemoryMapping)")]
        public delegate* unmanaged[Cdecl]<ConnectorInstanceContainer_CBox_c_void_____CArc_c_void*, CSliceRef_PhysicalMemoryMapping, void> set_mem_map;

        [NativeTypeName("MemoryViewBase_CBox_c_void_____CArc_c_void (*)(struct ConnectorInstanceContainer_CBox_c_void_____CArc_c_void)")]
        public delegate* unmanaged[Cdecl]<ConnectorInstanceContainer_CBox_c_void_____CArc_c_void, CGlueTraitObj_CBox_c_void_____MemoryViewVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void______________CArc_c_void_____MemoryViewRetTmp_CArc_c_void> into_phys_view;

        [NativeTypeName("MemoryViewBase_CBox_c_void_____CArc_c_void (*)(struct ConnectorInstanceContainer_CBox_c_void_____CArc_c_void *)")]
        public delegate* unmanaged[Cdecl]<ConnectorInstanceContainer_CBox_c_void_____CArc_c_void*, CGlueTraitObj_CBox_c_void_____MemoryViewVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void______________CArc_c_void_____MemoryViewRetTmp_CArc_c_void> phys_view;
    }

    public unsafe partial struct CollectBase
    {
        [NativeTypeName("char *")]
        public sbyte* buf;

        [NativeTypeName("size_t")]
        public nuint capacity;

        [NativeTypeName("size_t")]
        public nuint size;
    }

    public unsafe partial struct BufferIterator
    {
        [NativeTypeName("const char *")]
        public sbyte* buf;

        [NativeTypeName("size_t")]
        public nuint size;

        [NativeTypeName("size_t")]
        public nuint i;

        [NativeTypeName("size_t")]
        public nuint sz_elem;
    }

    public static unsafe partial class Methods
    {
        /// <summary>
        /// Initialize logging with selected logging level.
        /// </summary>
        [DllImport("libmemflow_ffi", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void log_init(LevelFilter level_filter);

        /// <summary>
        /// Logs a error message via log::error!
        /// # Safety
        /// The provided string must be a valid null-terminated char array.
        /// </summary>
        [DllImport("libmemflow_ffi", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void log_error([NativeTypeName("const char *")] sbyte* s);

        /// <summary>
        /// Logs a warning message via log::warn!
        /// # Safety
        /// The provided string must be a valid null-terminated char array.
        /// </summary>
        [DllImport("libmemflow_ffi", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void log_warn([NativeTypeName("const char *")] sbyte* s);

        /// <summary>
        /// Logs a info message via log::info!
        /// # Safety
        /// The provided string must be a valid null-terminated char array.
        /// </summary>
        [DllImport("libmemflow_ffi", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void log_info([NativeTypeName("const char *")] sbyte* s);

        /// <summary>
        /// Logs a debug message via log::debug!
        /// # Safety
        /// The provided string must be a valid null-terminated char array.
        /// </summary>
        [DllImport("libmemflow_ffi", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void log_debug([NativeTypeName("const char *")] sbyte* s);

        /// <summary>
        /// Logs a trace message via log::trace!
        /// # Safety
        /// The provided string must be a valid null-terminated char array.
        /// </summary>
        [DllImport("libmemflow_ffi", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void log_trace([NativeTypeName("const char *")] sbyte* s);

        /// <summary>
        /// Logs an error code with custom log level.
        /// </summary>
        [DllImport("libmemflow_ffi", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void log_errorcode(Level level, [NativeTypeName("int32_t")] int error);

        /// <summary>
        /// Logs an error with debug log level.
        /// </summary>
        [DllImport("libmemflow_ffi", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void log_debug_errorcode([NativeTypeName("int32_t")] int error);

        /// <summary>
        /// Sets new maximum log level.
        /// If `inventory` is supplied, the log level is also updated within all plugin instances. However,if it is not supplied, plugins will not have their log levels updated, potentially leading tolower performance, or less logging than expected.
        /// </summary>
        [DllImport("libmemflow_ffi", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void log_set_max_level(LevelFilter level_filter, [NativeTypeName("const struct Inventory *")] Inventory* inventory);

        /// <summary>
        /// Helper to convert `Address` to a `PhysicalAddress`
        /// This will create a `PhysicalAddress` with `UNKNOWN` PageType.
        /// </summary>
        [DllImport("libmemflow_ffi", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("struct PhysicalAddress")]
        public static extern PhysicalAddress addr_to_paddr([NativeTypeName("Address")] ulong address);

        /// <summary>
        /// Create a new connector inventory
        /// This function will try to find connectors using PATH environment variable
        /// Note that all functions go through each directories, and look for a `memflow` directory,and search for libraries in those.
        /// # Safety
        /// Inventory is inherently unsafe, because it loads shared libraries which can not beguaranteed to be safe.
        /// </summary>
        [DllImport("libmemflow_ffi", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("struct Inventory *")]
        public static extern Inventory* inventory_scan();

        /// <summary>
        /// Create a new inventory with custom path string
        /// # Safety
        /// `path` must be a valid null terminated string
        /// </summary>
        [DllImport("libmemflow_ffi", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("struct Inventory *")]
        public static extern Inventory* inventory_scan_path([NativeTypeName("const char *")] sbyte* path);

        /// <summary>
        /// Add a directory to an existing inventory
        /// # Safety
        /// `dir` must be a valid null terminated string
        /// </summary>
        [DllImport("libmemflow_ffi", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("int32_t")]
        public static extern int inventory_add_dir([NativeTypeName("struct Inventory *")] Inventory* inv, [NativeTypeName("const char *")] sbyte* dir);

        /// <summary>
        /// Create a connector with given arguments
        /// This creates an instance of `ConnectorInstance`.
        /// This instance needs to be dropped using `connector_drop`.
        /// # Arguments
        /// * `name` - name of the connector to use* `args` - arguments to be passed to the connector upon its creation
        /// # Safety
        /// Both `name`, and `args` must be valid null terminated strings.
        /// Any error strings returned by the connector must not be outputed after the connector getsfreed, because that operation could cause the underlying shared library to get unloaded.
        /// </summary>
        [DllImport("libmemflow_ffi", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("int32_t")]
        public static extern int inventory_create_connector([NativeTypeName("struct Inventory *")] Inventory* inv, [NativeTypeName("const char *")] sbyte* name, [NativeTypeName("const char *")] sbyte* args, [NativeTypeName("MuConnectorInstanceArcBox *")] ConnectorInstance_CBox_c_void_____CArc_c_void* @out);

        /// <summary>
        /// Create a OS instance with given arguments
        /// This creates an instance of `KernelInstance`.
        /// This instance needs to be freed using `os_drop`.
        /// # Arguments
        /// * `name` - name of the OS to use* `args` - arguments to be passed to the connector upon its creation* `mem` - a previously initialized connector instance* `out` - a valid memory location that will contain the resulting os-instance
        /// # Remarks
        /// The `mem` connector instance is being _moved_ into the os layer.This means upon calling `os_drop` it is not unnecessary to call `connector_drop` anymore.
        /// # Safety
        /// Both `name`, and `args` must be valid null terminated strings.
        /// Any error strings returned by the connector must not be outputed after the connector getsfreed, because that operation could cause the underlying shared library to get unloaded.
        /// </summary>
        [DllImport("libmemflow_ffi", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("int32_t")]
        public static extern int inventory_create_os([NativeTypeName("struct Inventory *")] Inventory* inv, [NativeTypeName("const char *")] sbyte* name, [NativeTypeName("const char *")] sbyte* args, [NativeTypeName("ConnectorInstanceArcBox *")] ConnectorInstance_CBox_c_void_____CArc_c_void* mem, [NativeTypeName("MuOsInstanceArcBox *")] OsInstance_CBox_c_void_____CArc_c_void* @out);

        /// <summary>
        /// Free a os plugin
        /// # Safety
        /// `os` must point to a valid `OsInstance` that was created using one of the providedfunctions.
        /// </summary>
        [DllImport("libmemflow_ffi", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void os_drop([NativeTypeName("OsInstanceArcBox *")] OsInstance_CBox_c_void_____CArc_c_void* os);

        /// <summary>
        /// Clone a connector
        /// This method is useful when needing to perform multithreaded operations, as a connector is notguaranteed to be thread safe. Every single cloned instance also needs to be dropped using`connector_drop`.
        /// # Safety
        /// `conn` has to point to a a valid `CloneablePhysicalMemory` created by one of the providedfunctions.
        /// </summary>
        [DllImport("libmemflow_ffi", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void connector_clone([NativeTypeName("const ConnectorInstanceArcBox *")] ConnectorInstance_CBox_c_void_____CArc_c_void* conn, [NativeTypeName("MuConnectorInstanceArcBox *")] ConnectorInstance_CBox_c_void_____CArc_c_void* @out);

        /// <summary>
        /// Free a connector instance
        /// # Safety
        /// `conn` has to point to a valid [`ConnectorInstance`] created by one of the providedfunctions.
        /// There has to be no instance of `PhysicalMemory` created from the input `conn`, because theywill become invalid.
        /// </summary>
        [DllImport("libmemflow_ffi", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void connector_drop([NativeTypeName("ConnectorInstanceArcBox *")] ConnectorInstance_CBox_c_void_____CArc_c_void* conn);

        /// <summary>
        /// Free a connector inventory
        /// # Safety
        /// `inv` must point to a valid `Inventory` that was created using one of the providedfunctions.
        /// </summary>
        [DllImport("libmemflow_ffi", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void inventory_free([NativeTypeName("struct Inventory *")] Inventory* inv);

        [DllImport("libmemflow_ffi", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint8_t")]
        public static extern byte arch_bits([NativeTypeName("const struct ArchitectureObj *")] ArchitectureObj* arch);

        [DllImport("libmemflow_ffi", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Endianess arch_endianess([NativeTypeName("const struct ArchitectureObj *")] ArchitectureObj* arch);

        [DllImport("libmemflow_ffi", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uintptr_t")]
        public static extern nuint arch_page_size([NativeTypeName("const struct ArchitectureObj *")] ArchitectureObj* arch);

        [DllImport("libmemflow_ffi", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uintptr_t")]
        public static extern nuint arch_size_addr([NativeTypeName("const struct ArchitectureObj *")] ArchitectureObj* arch);

        [DllImport("libmemflow_ffi", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint8_t")]
        public static extern byte arch_address_space_bits([NativeTypeName("const struct ArchitectureObj *")] ArchitectureObj* arch);

        /// <summary>
        /// Free an architecture reference
        /// # Safety
        /// `arch` must be a valid heap allocated reference created by one of the API's functions.
        /// </summary>
        [DllImport("libmemflow_ffi", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void arch_free([NativeTypeName("struct ArchitectureObj *")] ArchitectureObj* arch);

        [DllImport("libmemflow_ffi", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte is_x86_arch([NativeTypeName("const struct ArchitectureObj *")] ArchitectureObj* arch);

        public static CArc_c_void ctx_arc_clone(CArc_c_void* self)
        {
            CArc_c_void ret = *self;

            ret.instance = self->clone_fn(self->instance);
            return ret;
        }

        public static void ctx_arc_drop(CArc_c_void* self)
        {
            if ((self->drop_fn) != null && (self->instance) != null)
            {
                self->drop_fn(self->instance);
            }
        }

        public static void cont_box_drop(CBox_c_void* self)
        {
            if ((self->drop_fn) != null && (self->instance) != null)
            {
                self->drop_fn(self->instance);
            }
        }

        public static void mf_pause(void* self)
        {
            ((CGlueTraitObj_CBox_c_void_____CpuStateVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____CpuStateRetTmp_CArc_c_void______________CArc_c_void_____CpuStateRetTmp_CArc_c_void*)self)->vtbl->pause(&((CGlueTraitObj_CBox_c_void_____CpuStateVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____CpuStateRetTmp_CArc_c_void______________CArc_c_void_____CpuStateRetTmp_CArc_c_void*)self)->container);
        }

        public static void mf_resume(void* self)
        {
            ((CGlueTraitObj_CBox_c_void_____CpuStateVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____CpuStateRetTmp_CArc_c_void______________CArc_c_void_____CpuStateRetTmp_CArc_c_void*)self)->vtbl->resume(&((CGlueTraitObj_CBox_c_void_____CpuStateVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____CpuStateRetTmp_CArc_c_void______________CArc_c_void_____CpuStateRetTmp_CArc_c_void*)self)->container);
        }

        public static void mf_cpustate_drop([NativeTypeName("struct CGlueTraitObj_CBox_c_void_____CpuStateVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____CpuStateRetTmp_CArc_c_void______________CArc_c_void_____CpuStateRetTmp_CArc_c_void")] CGlueTraitObj_CBox_c_void_____CpuStateVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____CpuStateRetTmp_CArc_c_void______________CArc_c_void_____CpuStateRetTmp_CArc_c_void self)
        {
            cont_box_drop(&self.container.instance);
            ctx_arc_drop(&self.container.context);
        }

        public static bool mf_keyboardstate_is_down([NativeTypeName("const void *")] void* self, [NativeTypeName("int32_t")] int vk)
        {
            bool __ret = ((CGlueTraitObj_CBox_c_void_____KeyboardStateVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____KeyboardStateRetTmp_CArc_c_void______________CArc_c_void_____KeyboardStateRetTmp_CArc_c_void*)self)->vtbl->is_down(&((CGlueTraitObj_CBox_c_void_____KeyboardStateVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____KeyboardStateRetTmp_CArc_c_void______________CArc_c_void_____KeyboardStateRetTmp_CArc_c_void*)self)->container, vk) != 0;

            return __ret;
        }

        public static void mf_keyboardstate_drop([NativeTypeName("struct CGlueTraitObj_CBox_c_void_____KeyboardStateVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____KeyboardStateRetTmp_CArc_c_void______________CArc_c_void_____KeyboardStateRetTmp_CArc_c_void")] CGlueTraitObj_CBox_c_void_____KeyboardStateVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____KeyboardStateRetTmp_CArc_c_void______________CArc_c_void_____KeyboardStateRetTmp_CArc_c_void self)
        {
            cont_box_drop(&self.container.instance);
            ctx_arc_drop(&self.container.context);
        }

        public static bool mf_keyboard_is_down(void* self, [NativeTypeName("int32_t")] int vk)
        {
            bool __ret = ((CGlueTraitObj_CBox_c_void_____KeyboardVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____KeyboardRetTmp_CArc_c_void______________CArc_c_void_____KeyboardRetTmp_CArc_c_void*)self)->vtbl->is_down(&((CGlueTraitObj_CBox_c_void_____KeyboardVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____KeyboardRetTmp_CArc_c_void______________CArc_c_void_____KeyboardRetTmp_CArc_c_void*)self)->container, vk) != 0;

            return __ret;
        }

        public static void mf_set_down(void* self, [NativeTypeName("int32_t")] int vk, bool down)
        {
            ((CGlueTraitObj_CBox_c_void_____KeyboardVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____KeyboardRetTmp_CArc_c_void______________CArc_c_void_____KeyboardRetTmp_CArc_c_void*)self)->vtbl->set_down(&((CGlueTraitObj_CBox_c_void_____KeyboardVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____KeyboardRetTmp_CArc_c_void______________CArc_c_void_____KeyboardRetTmp_CArc_c_void*)self)->container, vk, down ? (byte)0x1 : (byte)0x0);
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_state(void* self, [NativeTypeName("KeyboardStateBase_CBox_c_void_____CArc_c_void *")] CGlueTraitObj_CBox_c_void_____KeyboardStateVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____KeyboardStateRetTmp_CArc_c_void______________CArc_c_void_____KeyboardStateRetTmp_CArc_c_void* ok_out)
        {
            int __ret = ((CGlueTraitObj_CBox_c_void_____KeyboardVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____KeyboardRetTmp_CArc_c_void______________CArc_c_void_____KeyboardRetTmp_CArc_c_void*)self)->vtbl->state(&((CGlueTraitObj_CBox_c_void_____KeyboardVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____KeyboardRetTmp_CArc_c_void______________CArc_c_void_____KeyboardRetTmp_CArc_c_void*)self)->container, ok_out);

            return __ret;
        }

        public static void mf_keyboard_drop([NativeTypeName("struct CGlueTraitObj_CBox_c_void_____KeyboardVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____KeyboardRetTmp_CArc_c_void______________CArc_c_void_____KeyboardRetTmp_CArc_c_void")] CGlueTraitObj_CBox_c_void_____KeyboardVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____KeyboardRetTmp_CArc_c_void______________CArc_c_void_____KeyboardRetTmp_CArc_c_void self)
        {
            cont_box_drop(&self.container.instance);
            ctx_arc_drop(&self.container.context);
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_read_raw_iter(void* self, [NativeTypeName("ReadRawMemOps")] MemOps_ReadDataRaw__ReadData data)
        {
            int __ret = ((CGlueTraitObj_CBox_c_void_____MemoryViewVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void______________CArc_c_void_____MemoryViewRetTmp_CArc_c_void*)self)->vtbl->read_raw_iter(&((CGlueTraitObj_CBox_c_void_____MemoryViewVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void______________CArc_c_void_____MemoryViewRetTmp_CArc_c_void*)self)->container, data);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_write_raw_iter(void* self, [NativeTypeName("WriteRawMemOps")] MemOps_WriteDataRaw__WriteData data)
        {
            int __ret = ((CGlueTraitObj_CBox_c_void_____MemoryViewVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void______________CArc_c_void_____MemoryViewRetTmp_CArc_c_void*)self)->vtbl->write_raw_iter(&((CGlueTraitObj_CBox_c_void_____MemoryViewVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void______________CArc_c_void_____MemoryViewRetTmp_CArc_c_void*)self)->container, data);

            return __ret;
        }

        [return: NativeTypeName("struct MemoryViewMetadata")]
        public static MemoryViewMetadata mf_metadata([NativeTypeName("const void *")] void* self)
        {
            MemoryViewMetadata __ret = ((CGlueTraitObj_CBox_c_void_____MemoryViewVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void______________CArc_c_void_____MemoryViewRetTmp_CArc_c_void*)self)->vtbl->metadata(&((CGlueTraitObj_CBox_c_void_____MemoryViewVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void______________CArc_c_void_____MemoryViewRetTmp_CArc_c_void*)self)->container);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_read_iter(void* self, [NativeTypeName("struct CIterator_ReadData")] CIterator_ReadData inp, [NativeTypeName("ReadCallback *")] Callback_c_void__ReadData* @out, [NativeTypeName("ReadCallback *")] Callback_c_void__ReadData* out_fail)
        {
            int __ret = ((CGlueTraitObj_CBox_c_void_____MemoryViewVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void______________CArc_c_void_____MemoryViewRetTmp_CArc_c_void*)self)->vtbl->read_iter(&((CGlueTraitObj_CBox_c_void_____MemoryViewVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void______________CArc_c_void_____MemoryViewRetTmp_CArc_c_void*)self)->container, inp, @out, out_fail);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_read_raw_list(void* self, [NativeTypeName("struct CSliceMut_ReadData")] CSliceMut_ReadData data)
        {
            int __ret = ((CGlueTraitObj_CBox_c_void_____MemoryViewVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void______________CArc_c_void_____MemoryViewRetTmp_CArc_c_void*)self)->vtbl->read_raw_list(&((CGlueTraitObj_CBox_c_void_____MemoryViewVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void______________CArc_c_void_____MemoryViewRetTmp_CArc_c_void*)self)->container, data);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_read_raw_into(void* self, [NativeTypeName("Address")] ulong addr, [NativeTypeName("struct CSliceMut_u8")] CSliceMut_u8 @out)
        {
            int __ret = ((CGlueTraitObj_CBox_c_void_____MemoryViewVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void______________CArc_c_void_____MemoryViewRetTmp_CArc_c_void*)self)->vtbl->read_raw_into(&((CGlueTraitObj_CBox_c_void_____MemoryViewVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void______________CArc_c_void_____MemoryViewRetTmp_CArc_c_void*)self)->container, addr, @out);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_write_iter(void* self, [NativeTypeName("struct CIterator_WriteData")] CIterator_WriteData inp, [NativeTypeName("WriteCallback *")] Callback_c_void__WriteData* @out, [NativeTypeName("WriteCallback *")] Callback_c_void__WriteData* out_fail)
        {
            int __ret = ((CGlueTraitObj_CBox_c_void_____MemoryViewVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void______________CArc_c_void_____MemoryViewRetTmp_CArc_c_void*)self)->vtbl->write_iter(&((CGlueTraitObj_CBox_c_void_____MemoryViewVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void______________CArc_c_void_____MemoryViewRetTmp_CArc_c_void*)self)->container, inp, @out, out_fail);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_write_raw_list(void* self, [NativeTypeName("struct CSliceRef_WriteData")] CSliceRef_WriteData data)
        {
            int __ret = ((CGlueTraitObj_CBox_c_void_____MemoryViewVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void______________CArc_c_void_____MemoryViewRetTmp_CArc_c_void*)self)->vtbl->write_raw_list(&((CGlueTraitObj_CBox_c_void_____MemoryViewVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void______________CArc_c_void_____MemoryViewRetTmp_CArc_c_void*)self)->container, data);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_write_raw(void* self, [NativeTypeName("Address")] ulong addr, [NativeTypeName("struct CSliceRef_u8")] CSliceRef_u8 data)
        {
            int __ret = ((CGlueTraitObj_CBox_c_void_____MemoryViewVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void______________CArc_c_void_____MemoryViewRetTmp_CArc_c_void*)self)->vtbl->write_raw(&((CGlueTraitObj_CBox_c_void_____MemoryViewVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void______________CArc_c_void_____MemoryViewRetTmp_CArc_c_void*)self)->container, addr, data);

            return __ret;
        }

        public static void mf_memoryview_drop([NativeTypeName("struct CGlueTraitObj_CBox_c_void_____MemoryViewVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void______________CArc_c_void_____MemoryViewRetTmp_CArc_c_void")] CGlueTraitObj_CBox_c_void_____MemoryViewVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void______________CArc_c_void_____MemoryViewRetTmp_CArc_c_void self)
        {
            cont_box_drop(&self.container.instance);
            ctx_arc_drop(&self.container.context);
        }

        [return: NativeTypeName("struct ConnectorInstance_CBox_c_void_____CArc_c_void")]
        public static ConnectorInstance_CBox_c_void_____CArc_c_void mf_connectorinstance_clone([NativeTypeName("const void *")] void* self)
        {
            ConnectorInstance_CBox_c_void_____CArc_c_void __ret = new ConnectorInstance_CBox_c_void_____CArc_c_void();

            __ret.container=(((ConnectorInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_clone->clone(&((ConnectorInstance_CBox_c_void_____CArc_c_void*)self)->container));
            return __ret;
        }

        public static void mf_connectorinstance_drop([NativeTypeName("struct ConnectorInstance_CBox_c_void_____CArc_c_void")] ConnectorInstance_CBox_c_void_____CArc_c_void self)
        {
            cont_box_drop(&self.container.instance);
            ctx_arc_drop(&self.container.context);
        }

        [return: NativeTypeName("struct IntoCpuState_CBox_c_void_____CArc_c_void")]
        public static IntoCpuState_CBox_c_void_____CArc_c_void mf_intocpustate_clone([NativeTypeName("const void *")] void* self)
        {
            IntoCpuState_CBox_c_void_____CArc_c_void __ret = new IntoCpuState_CBox_c_void_____CArc_c_void();

            __ret.container=(((IntoCpuState_CBox_c_void_____CArc_c_void*)self)->vtbl_clone->clone(&((IntoCpuState_CBox_c_void_____CArc_c_void*)self)->container));
            return __ret;
        }

        public static void mf_intocpustate_drop([NativeTypeName("struct IntoCpuState_CBox_c_void_____CArc_c_void")] IntoCpuState_CBox_c_void_____CArc_c_void self)
        {
            cont_box_drop(&self.container.instance);
            ctx_arc_drop(&self.container.context);
        }

        public static void mf_intocpustate_pause(void* self)
        {
            ((IntoCpuState_CBox_c_void_____CArc_c_void*)self)->vtbl_cpustate->pause(&((IntoCpuState_CBox_c_void_____CArc_c_void*)self)->container);
        }

        public static void mf_intocpustate_resume(void* self)
        {
            ((IntoCpuState_CBox_c_void_____CArc_c_void*)self)->vtbl_cpustate->resume(&((IntoCpuState_CBox_c_void_____CArc_c_void*)self)->container);
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_connectorinstance_cpu_state(void* self, [NativeTypeName("CpuStateBase_CBox_c_void_____CArc_c_void *")] CGlueTraitObj_CBox_c_void_____CpuStateVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____CpuStateRetTmp_CArc_c_void______________CArc_c_void_____CpuStateRetTmp_CArc_c_void* ok_out)
        {
            int __ret = ((ConnectorInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_connectorcpustateinner->cpu_state(&((ConnectorInstance_CBox_c_void_____CArc_c_void*)self)->container, ok_out);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_connectorinstance_into_cpu_state([NativeTypeName("struct ConnectorInstance_CBox_c_void_____CArc_c_void")] ConnectorInstance_CBox_c_void_____CArc_c_void self, [NativeTypeName("struct IntoCpuState_CBox_c_void_____CArc_c_void *")] IntoCpuState_CBox_c_void_____CArc_c_void* ok_out)
        {
            CArc_c_void ___ctx = ctx_arc_clone(&self.container.context);
            int __ret = (self.vtbl_connectorcpustateinner)->into_cpu_state(self.container, ok_out);

            ctx_arc_drop(&___ctx);
            return __ret;
        }

        [return: NativeTypeName("struct OsInstance_CBox_c_void_____CArc_c_void")]
        public static OsInstance_CBox_c_void_____CArc_c_void mf_osinstance_clone([NativeTypeName("const void *")] void* self)
        {
            OsInstance_CBox_c_void_____CArc_c_void __ret = new OsInstance_CBox_c_void_____CArc_c_void();

            __ret.container=(((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_clone->clone(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container));
            return __ret;
        }

        public static void mf_osinstance_drop([NativeTypeName("struct OsInstance_CBox_c_void_____CArc_c_void")] OsInstance_CBox_c_void_____CArc_c_void self)
        {
            cont_box_drop(&self.container.instance);
            ctx_arc_drop(&self.container.context);
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_osinstance_process_address_list_callback(void* self, [NativeTypeName("AddressCallback")] Callback_c_void__Address callback)
        {
            int __ret = ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_osinner->process_address_list_callback(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container, callback);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_osinstance_process_info_list_callback(void* self, [NativeTypeName("ProcessInfoCallback")] Callback_c_void__ProcessInfo callback)
        {
            int __ret = ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_osinner->process_info_list_callback(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container, callback);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_osinstance_process_info_by_address(void* self, [NativeTypeName("Address")] ulong address, [NativeTypeName("struct ProcessInfo *")] ProcessInfo* ok_out)
        {
            int __ret = ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_osinner->process_info_by_address(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container, address, ok_out);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_osinstance_process_info_by_name(void* self, [NativeTypeName("struct CSliceRef_u8")] CSliceRef_u8 name, [NativeTypeName("struct ProcessInfo *")] ProcessInfo* ok_out)
        {
            int __ret = ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_osinner->process_info_by_name(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container, name, ok_out);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_osinstance_process_info_by_pid(void* self, [NativeTypeName("Pid")] uint pid, [NativeTypeName("struct ProcessInfo *")] ProcessInfo* ok_out)
        {
            int __ret = ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_osinner->process_info_by_pid(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container, pid, ok_out);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_osinstance_process_by_info(void* self, [NativeTypeName("struct ProcessInfo")] ProcessInfo info, [NativeTypeName("struct ProcessInstance_CBox_c_void_____CArc_c_void *")] ProcessInstance_CBox_c_void_____CArc_c_void* ok_out)
        {
            int __ret = ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_osinner->process_by_info(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container, info, ok_out);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_osinstance_into_process_by_info([NativeTypeName("struct OsInstance_CBox_c_void_____CArc_c_void")] OsInstance_CBox_c_void_____CArc_c_void self, [NativeTypeName("struct ProcessInfo")] ProcessInfo info, [NativeTypeName("struct IntoProcessInstance_CBox_c_void_____CArc_c_void *")] IntoProcessInstance_CBox_c_void_____CArc_c_void* ok_out)
        {
            CArc_c_void ___ctx = ctx_arc_clone(&self.container.context);
            int __ret = (self.vtbl_osinner)->into_process_by_info(self.container, info, ok_out);

            ctx_arc_drop(&___ctx);
            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_osinstance_process_by_address(void* self, [NativeTypeName("Address")] ulong addr, [NativeTypeName("struct ProcessInstance_CBox_c_void_____CArc_c_void *")] ProcessInstance_CBox_c_void_____CArc_c_void* ok_out)
        {
            int __ret = ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_osinner->process_by_address(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container, addr, ok_out);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_osinstance_process_by_name(void* self, [NativeTypeName("struct CSliceRef_u8")] CSliceRef_u8 name, [NativeTypeName("struct ProcessInstance_CBox_c_void_____CArc_c_void *")] ProcessInstance_CBox_c_void_____CArc_c_void* ok_out)
        {
            int __ret = ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_osinner->process_by_name(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container, name, ok_out);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_osinstance_process_by_pid(void* self, [NativeTypeName("Pid")] uint pid, [NativeTypeName("struct ProcessInstance_CBox_c_void_____CArc_c_void *")] ProcessInstance_CBox_c_void_____CArc_c_void* ok_out)
        {
            int __ret = ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_osinner->process_by_pid(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container, pid, ok_out);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_osinstance_into_process_by_address([NativeTypeName("struct OsInstance_CBox_c_void_____CArc_c_void")] OsInstance_CBox_c_void_____CArc_c_void self, [NativeTypeName("Address")] ulong addr, [NativeTypeName("struct IntoProcessInstance_CBox_c_void_____CArc_c_void *")] IntoProcessInstance_CBox_c_void_____CArc_c_void* ok_out)
        {
            CArc_c_void ___ctx = ctx_arc_clone(&self.container.context);
            int __ret = (self.vtbl_osinner)->into_process_by_address(self.container, addr, ok_out);

            ctx_arc_drop(&___ctx);
            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_osinstance_into_process_by_name([NativeTypeName("struct OsInstance_CBox_c_void_____CArc_c_void")] OsInstance_CBox_c_void_____CArc_c_void self, [NativeTypeName("struct CSliceRef_u8")] CSliceRef_u8 name, [NativeTypeName("struct IntoProcessInstance_CBox_c_void_____CArc_c_void *")] IntoProcessInstance_CBox_c_void_____CArc_c_void* ok_out)
        {
            CArc_c_void ___ctx = ctx_arc_clone(&self.container.context);
            int __ret = (self.vtbl_osinner)->into_process_by_name(self.container, name, ok_out);

            ctx_arc_drop(&___ctx);
            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_osinstance_into_process_by_pid([NativeTypeName("struct OsInstance_CBox_c_void_____CArc_c_void")] OsInstance_CBox_c_void_____CArc_c_void self, [NativeTypeName("Pid")] uint pid, [NativeTypeName("struct IntoProcessInstance_CBox_c_void_____CArc_c_void *")] IntoProcessInstance_CBox_c_void_____CArc_c_void* ok_out)
        {
            CArc_c_void ___ctx = ctx_arc_clone(&self.container.context);
            int __ret = (self.vtbl_osinner)->into_process_by_pid(self.container, pid, ok_out);

            ctx_arc_drop(&___ctx);
            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_osinstance_module_address_list_callback(void* self, [NativeTypeName("AddressCallback")] Callback_c_void__Address callback)
        {
            int __ret = ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_osinner->module_address_list_callback(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container, callback);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_osinstance_module_list_callback(void* self, [NativeTypeName("ModuleInfoCallback")] Callback_c_void__ModuleInfo callback)
        {
            int __ret = ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_osinner->module_list_callback(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container, callback);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_osinstance_module_by_address(void* self, [NativeTypeName("Address")] ulong address, [NativeTypeName("struct ModuleInfo *")] ModuleInfo* ok_out)
        {
            int __ret = ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_osinner->module_by_address(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container, address, ok_out);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_osinstance_module_by_name(void* self, [NativeTypeName("struct CSliceRef_u8")] CSliceRef_u8 name, [NativeTypeName("struct ModuleInfo *")] ModuleInfo* ok_out)
        {
            int __ret = ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_osinner->module_by_name(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container, name, ok_out);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_osinstance_primary_module_address(void* self, [NativeTypeName("Address *")] ulong* ok_out)
        {
            int __ret = ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_osinner->primary_module_address(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container, ok_out);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_osinstance_primary_module(void* self, [NativeTypeName("struct ModuleInfo *")] ModuleInfo* ok_out)
        {
            int __ret = ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_osinner->primary_module(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container, ok_out);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_osinstance_module_import_list_callback(void* self, [NativeTypeName("const struct ModuleInfo *")] ModuleInfo* info, [NativeTypeName("ImportCallback")] Callback_c_void__ImportInfo callback)
        {
            int __ret = ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_osinner->module_import_list_callback(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container, info, callback);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_osinstance_module_export_list_callback(void* self, [NativeTypeName("const struct ModuleInfo *")] ModuleInfo* info, [NativeTypeName("ExportCallback")] Callback_c_void__ExportInfo callback)
        {
            int __ret = ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_osinner->module_export_list_callback(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container, info, callback);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_osinstance_module_section_list_callback(void* self, [NativeTypeName("const struct ModuleInfo *")] ModuleInfo* info, [NativeTypeName("SectionCallback")] Callback_c_void__SectionInfo callback)
        {
            int __ret = ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_osinner->module_section_list_callback(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container, info, callback);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_osinstance_module_import_by_name(void* self, [NativeTypeName("const struct ModuleInfo *")] ModuleInfo* info, [NativeTypeName("struct CSliceRef_u8")] CSliceRef_u8 name, [NativeTypeName("struct ImportInfo *")] ImportInfo* ok_out)
        {
            int __ret = ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_osinner->module_import_by_name(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container, info, name, ok_out);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_osinstance_module_export_by_name(void* self, [NativeTypeName("const struct ModuleInfo *")] ModuleInfo* info, [NativeTypeName("struct CSliceRef_u8")] CSliceRef_u8 name, [NativeTypeName("struct ExportInfo *")] ExportInfo* ok_out)
        {
            int __ret = ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_osinner->module_export_by_name(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container, info, name, ok_out);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_osinstance_module_section_by_name(void* self, [NativeTypeName("const struct ModuleInfo *")] ModuleInfo* info, [NativeTypeName("struct CSliceRef_u8")] CSliceRef_u8 name, [NativeTypeName("struct SectionInfo *")] SectionInfo* ok_out)
        {
            int __ret = ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_osinner->module_section_by_name(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container, info, name, ok_out);

            return __ret;
        }

        [return: NativeTypeName("const struct OsInfo *")]
        public static OsInfo* mf_osinstance_info([NativeTypeName("const void *")] void* self)
        {
            OsInfo* __ret = ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_osinner->info(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_osinstance_read_raw_iter(void* self, [NativeTypeName("ReadRawMemOps")] MemOps_ReadDataRaw__ReadData data)
        {
            int __ret = ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_memoryview->read_raw_iter(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container, data);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_osinstance_write_raw_iter(void* self, [NativeTypeName("WriteRawMemOps")] MemOps_WriteDataRaw__WriteData data)
        {
            int __ret = ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_memoryview->write_raw_iter(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container, data);

            return __ret;
        }

        [return: NativeTypeName("struct MemoryViewMetadata")]
        public static MemoryViewMetadata mf_osinstance_metadata([NativeTypeName("const void *")] void* self)
        {
            MemoryViewMetadata __ret = ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_memoryview->metadata(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_osinstance_read_iter(void* self, [NativeTypeName("struct CIterator_ReadData")] CIterator_ReadData inp, [NativeTypeName("ReadCallback *")] Callback_c_void__ReadData* @out, [NativeTypeName("ReadCallback *")] Callback_c_void__ReadData* out_fail)
        {
            int __ret = ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_memoryview->read_iter(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container, inp, @out, out_fail);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_osinstance_read_raw_list(void* self, [NativeTypeName("struct CSliceMut_ReadData")] CSliceMut_ReadData data)
        {
            int __ret = ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_memoryview->read_raw_list(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container, data);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_osinstance_read_raw_into(void* self, [NativeTypeName("Address")] ulong addr, [NativeTypeName("struct CSliceMut_u8")] CSliceMut_u8 @out)
        {
            int __ret = ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_memoryview->read_raw_into(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container, addr, @out);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_osinstance_write_iter(void* self, [NativeTypeName("struct CIterator_WriteData")] CIterator_WriteData inp, [NativeTypeName("WriteCallback *")] Callback_c_void__WriteData* @out, [NativeTypeName("WriteCallback *")] Callback_c_void__WriteData* out_fail)
        {
            int __ret = ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_memoryview->write_iter(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container, inp, @out, out_fail);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_osinstance_write_raw_list(void* self, [NativeTypeName("struct CSliceRef_WriteData")] CSliceRef_WriteData data)
        {
            int __ret = ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_memoryview->write_raw_list(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container, data);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_osinstance_write_raw(void* self, [NativeTypeName("Address")] ulong addr, [NativeTypeName("struct CSliceRef_u8")] CSliceRef_u8 data)
        {
            int __ret = ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_memoryview->write_raw(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container, addr, data);

            return __ret;
        }

        [return: NativeTypeName("struct IntoKeyboard_CBox_c_void_____CArc_c_void")]
        public static IntoKeyboard_CBox_c_void_____CArc_c_void mf_intokeyboard_clone([NativeTypeName("const void *")] void* self)
        {
            IntoKeyboard_CBox_c_void_____CArc_c_void __ret = new IntoKeyboard_CBox_c_void_____CArc_c_void();

            __ret.container=(((IntoKeyboard_CBox_c_void_____CArc_c_void*)self)->vtbl_clone->clone(&((IntoKeyboard_CBox_c_void_____CArc_c_void*)self)->container));
            return __ret;
        }

        public static void mf_intokeyboard_drop([NativeTypeName("struct IntoKeyboard_CBox_c_void_____CArc_c_void")] IntoKeyboard_CBox_c_void_____CArc_c_void self)
        {
            cont_box_drop(&self.container.instance);
            ctx_arc_drop(&self.container.context);
        }

        public static bool mf_intokeyboard_is_down(void* self, [NativeTypeName("int32_t")] int vk)
        {
            bool __ret = ((IntoKeyboard_CBox_c_void_____CArc_c_void*)self)->vtbl_keyboard->is_down(&((IntoKeyboard_CBox_c_void_____CArc_c_void*)self)->container, vk) != 0;

            return __ret;
        }

        public static void mf_intokeyboard_set_down(void* self, [NativeTypeName("int32_t")] int vk, bool down)
        {
            ((IntoKeyboard_CBox_c_void_____CArc_c_void*)self)->vtbl_keyboard->set_down(&((IntoKeyboard_CBox_c_void_____CArc_c_void*)self)->container, vk, down ? (byte)0x1 : (byte)0x0);
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_intokeyboard_state(void* self, [NativeTypeName("KeyboardStateBase_CBox_c_void_____CArc_c_void *")] CGlueTraitObj_CBox_c_void_____KeyboardStateVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____KeyboardStateRetTmp_CArc_c_void______________CArc_c_void_____KeyboardStateRetTmp_CArc_c_void* ok_out)
        {
            int __ret = ((IntoKeyboard_CBox_c_void_____CArc_c_void*)self)->vtbl_keyboard->state(&((IntoKeyboard_CBox_c_void_____CArc_c_void*)self)->container, ok_out);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_osinstance_keyboard(void* self, [NativeTypeName("KeyboardBase_CBox_c_void_____CArc_c_void *")] CGlueTraitObj_CBox_c_void_____KeyboardVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____KeyboardRetTmp_CArc_c_void______________CArc_c_void_____KeyboardRetTmp_CArc_c_void* ok_out)
        {
            int __ret = ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_oskeyboardinner->keyboard(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container, ok_out);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_osinstance_into_keyboard([NativeTypeName("struct OsInstance_CBox_c_void_____CArc_c_void")] OsInstance_CBox_c_void_____CArc_c_void self, [NativeTypeName("struct IntoKeyboard_CBox_c_void_____CArc_c_void *")] IntoKeyboard_CBox_c_void_____CArc_c_void* ok_out)
        {
            CArc_c_void ___ctx = ctx_arc_clone(&self.container.context);
            int __ret = (self.vtbl_oskeyboardinner)->into_keyboard(self.container, ok_out);

            ctx_arc_drop(&___ctx);
            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_osinstance_phys_read_raw_iter(void* self, [NativeTypeName("PhysicalReadMemOps")] MemOps_PhysicalReadData__ReadData data)
        {
            int __ret = ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_physicalmemory->phys_read_raw_iter(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container, data);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_osinstance_phys_write_raw_iter(void* self, [NativeTypeName("PhysicalWriteMemOps")] MemOps_PhysicalWriteData__WriteData data)
        {
            int __ret = ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_physicalmemory->phys_write_raw_iter(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container, data);

            return __ret;
        }

        public static void mf_osinstance_set_mem_map(void* self, [NativeTypeName("struct CSliceRef_PhysicalMemoryMapping")] CSliceRef_PhysicalMemoryMapping _mem_map)
        {
            ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_physicalmemory->set_mem_map(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container, _mem_map);
        }

        [return: NativeTypeName("MemoryViewBase_CBox_c_void_____CArc_c_void")]
        public static CGlueTraitObj_CBox_c_void_____MemoryViewVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void______________CArc_c_void_____MemoryViewRetTmp_CArc_c_void mf_osinstance_into_phys_view([NativeTypeName("struct OsInstance_CBox_c_void_____CArc_c_void")] OsInstance_CBox_c_void_____CArc_c_void self)
        {
            CArc_c_void ___ctx = ctx_arc_clone(&self.container.context);
            CGlueTraitObj_CBox_c_void_____MemoryViewVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void______________CArc_c_void_____MemoryViewRetTmp_CArc_c_void __ret = (self.vtbl_physicalmemory)->into_phys_view(self.container);

            ctx_arc_drop(&___ctx);
            return __ret;
        }

        [return: NativeTypeName("MemoryViewBase_CBox_c_void_____CArc_c_void")]
        public static CGlueTraitObj_CBox_c_void_____MemoryViewVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void______________CArc_c_void_____MemoryViewRetTmp_CArc_c_void mf_osinstance_phys_view(void* self)
        {
            CGlueTraitObj_CBox_c_void_____MemoryViewVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void______________CArc_c_void_____MemoryViewRetTmp_CArc_c_void __ret = ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_physicalmemory->phys_view(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container);

            return __ret;
        }

        public static void mf_osinstance_virt_to_phys_list(void* self, [NativeTypeName("struct CSliceRef_VtopRange")] CSliceRef_VtopRange addrs, [NativeTypeName("VirtualTranslationCallback")] Callback_c_void__VirtualTranslation @out, [NativeTypeName("VirtualTranslationFailCallback")] Callback_c_void__VirtualTranslationFail out_fail)
        {
            ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_virtualtranslate->virt_to_phys_list(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container, addrs, @out, out_fail);
        }

        public static void mf_osinstance_virt_to_phys_range(void* self, [NativeTypeName("Address")] ulong start, [NativeTypeName("Address")] ulong end, [NativeTypeName("VirtualTranslationCallback")] Callback_c_void__VirtualTranslation @out)
        {
            ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_virtualtranslate->virt_to_phys_range(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container, start, end, @out);
        }

        public static void mf_osinstance_virt_translation_map_range(void* self, [NativeTypeName("Address")] ulong start, [NativeTypeName("Address")] ulong end, [NativeTypeName("VirtualTranslationCallback")] Callback_c_void__VirtualTranslation @out)
        {
            ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_virtualtranslate->virt_translation_map_range(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container, start, end, @out);
        }

        public static void mf_osinstance_virt_page_map_range(void* self, [NativeTypeName("imem")] long gap_size, [NativeTypeName("Address")] ulong start, [NativeTypeName("Address")] ulong end, [NativeTypeName("MemoryRangeCallback")] Callback_c_void__MemoryRange @out)
        {
            ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_virtualtranslate->virt_page_map_range(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container, gap_size, start, end, @out);
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_osinstance_virt_to_phys(void* self, [NativeTypeName("Address")] ulong address, [NativeTypeName("struct PhysicalAddress *")] PhysicalAddress* ok_out)
        {
            int __ret = ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_virtualtranslate->virt_to_phys(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container, address, ok_out);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_osinstance_virt_page_info(void* self, [NativeTypeName("Address")] ulong addr, [NativeTypeName("struct Page *")] Page* ok_out)
        {
            int __ret = ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_virtualtranslate->virt_page_info(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container, addr, ok_out);

            return __ret;
        }

        public static void mf_osinstance_virt_translation_map(void* self, [NativeTypeName("VirtualTranslationCallback")] Callback_c_void__VirtualTranslation @out)
        {
            ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_virtualtranslate->virt_translation_map(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container, @out);
        }

        [return: NativeTypeName("struct COption_Address")]
        public static COption_Address mf_osinstance_phys_to_virt(void* self, [NativeTypeName("Address")] ulong phys)
        {
            COption_Address __ret = ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_virtualtranslate->phys_to_virt(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container, phys);

            return __ret;
        }

        public static void mf_osinstance_virt_page_map(void* self, [NativeTypeName("imem")] long gap_size, [NativeTypeName("MemoryRangeCallback")] Callback_c_void__MemoryRange @out)
        {
            ((OsInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_virtualtranslate->virt_page_map(&((OsInstance_CBox_c_void_____CArc_c_void*)self)->container, gap_size, @out);
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_processinstance_read_raw_iter(void* self, [NativeTypeName("ReadRawMemOps")] MemOps_ReadDataRaw__ReadData data)
        {
            int __ret = ((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_memoryview->read_raw_iter(&((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, data);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_processinstance_write_raw_iter(void* self, [NativeTypeName("WriteRawMemOps")] MemOps_WriteDataRaw__WriteData data)
        {
            int __ret = ((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_memoryview->write_raw_iter(&((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, data);

            return __ret;
        }

        [return: NativeTypeName("struct MemoryViewMetadata")]
        public static MemoryViewMetadata mf_processinstance_metadata([NativeTypeName("const void *")] void* self)
        {
            MemoryViewMetadata __ret = ((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_memoryview->metadata(&((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->container);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_processinstance_read_iter(void* self, [NativeTypeName("struct CIterator_ReadData")] CIterator_ReadData inp, [NativeTypeName("ReadCallback *")] Callback_c_void__ReadData* @out, [NativeTypeName("ReadCallback *")] Callback_c_void__ReadData* out_fail)
        {
            int __ret = ((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_memoryview->read_iter(&((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, inp, @out, out_fail);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_processinstance_read_raw_list(void* self, [NativeTypeName("struct CSliceMut_ReadData")] CSliceMut_ReadData data)
        {
            int __ret = ((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_memoryview->read_raw_list(&((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, data);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_processinstance_read_raw_into(void* self, [NativeTypeName("Address")] ulong addr, [NativeTypeName("struct CSliceMut_u8")] CSliceMut_u8 @out)
        {
            int __ret = ((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_memoryview->read_raw_into(&((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, addr, @out);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_processinstance_write_iter(void* self, [NativeTypeName("struct CIterator_WriteData")] CIterator_WriteData inp, [NativeTypeName("WriteCallback *")] Callback_c_void__WriteData* @out, [NativeTypeName("WriteCallback *")] Callback_c_void__WriteData* out_fail)
        {
            int __ret = ((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_memoryview->write_iter(&((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, inp, @out, out_fail);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_processinstance_write_raw_list(void* self, [NativeTypeName("struct CSliceRef_WriteData")] CSliceRef_WriteData data)
        {
            int __ret = ((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_memoryview->write_raw_list(&((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, data);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_processinstance_write_raw(void* self, [NativeTypeName("Address")] ulong addr, [NativeTypeName("struct CSliceRef_u8")] CSliceRef_u8 data)
        {
            int __ret = ((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_memoryview->write_raw(&((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, addr, data);

            return __ret;
        }

        public static void mf_processinstance_drop([NativeTypeName("struct ProcessInstance_CBox_c_void_____CArc_c_void")] ProcessInstance_CBox_c_void_____CArc_c_void self)
        {
            cont_box_drop(&self.container.instance);
            ctx_arc_drop(&self.container.context);
        }

        [return: NativeTypeName("struct ProcessState")]
        public static ProcessState mf_processinstance_state(void* self)
        {
            ProcessState __ret = ((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_process->state(&((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->container);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_processinstance_module_address_list_callback(void* self, [NativeTypeName("const struct ArchitectureIdent *")] ArchitectureIdent* target_arch, [NativeTypeName("ModuleAddressCallback")] Callback_c_void__ModuleAddressInfo callback)
        {
            int __ret = ((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_process->module_address_list_callback(&((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, target_arch, callback);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_processinstance_module_list_callback(void* self, [NativeTypeName("const struct ArchitectureIdent *")] ArchitectureIdent* target_arch, [NativeTypeName("ModuleInfoCallback")] Callback_c_void__ModuleInfo callback)
        {
            int __ret = ((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_process->module_list_callback(&((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, target_arch, callback);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_processinstance_module_by_address(void* self, [NativeTypeName("Address")] ulong address, [NativeTypeName("struct ArchitectureIdent")] ArchitectureIdent architecture, [NativeTypeName("struct ModuleInfo *")] ModuleInfo* ok_out)
        {
            int __ret = ((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_process->module_by_address(&((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, address, architecture, ok_out);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_processinstance_module_by_name_arch(void* self, [NativeTypeName("struct CSliceRef_u8")] CSliceRef_u8 name, [NativeTypeName("const struct ArchitectureIdent *")] ArchitectureIdent* architecture, [NativeTypeName("struct ModuleInfo *")] ModuleInfo* ok_out)
        {
            int __ret = ((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_process->module_by_name_arch(&((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, name, architecture, ok_out);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_processinstance_module_by_name(void* self, [NativeTypeName("struct CSliceRef_u8")] CSliceRef_u8 name, [NativeTypeName("struct ModuleInfo *")] ModuleInfo* ok_out)
        {
            int __ret = ((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_process->module_by_name(&((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, name, ok_out);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_processinstance_primary_module_address(void* self, [NativeTypeName("Address *")] ulong* ok_out)
        {
            int __ret = ((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_process->primary_module_address(&((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, ok_out);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_processinstance_primary_module(void* self, [NativeTypeName("struct ModuleInfo *")] ModuleInfo* ok_out)
        {
            int __ret = ((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_process->primary_module(&((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, ok_out);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_processinstance_module_import_list_callback(void* self, [NativeTypeName("const struct ModuleInfo *")] ModuleInfo* info, [NativeTypeName("ImportCallback")] Callback_c_void__ImportInfo callback)
        {
            int __ret = ((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_process->module_import_list_callback(&((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, info, callback);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_processinstance_module_export_list_callback(void* self, [NativeTypeName("const struct ModuleInfo *")] ModuleInfo* info, [NativeTypeName("ExportCallback")] Callback_c_void__ExportInfo callback)
        {
            int __ret = ((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_process->module_export_list_callback(&((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, info, callback);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_processinstance_module_section_list_callback(void* self, [NativeTypeName("const struct ModuleInfo *")] ModuleInfo* info, [NativeTypeName("SectionCallback")] Callback_c_void__SectionInfo callback)
        {
            int __ret = ((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_process->module_section_list_callback(&((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, info, callback);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_processinstance_module_import_by_name(void* self, [NativeTypeName("const struct ModuleInfo *")] ModuleInfo* info, [NativeTypeName("struct CSliceRef_u8")] CSliceRef_u8 name, [NativeTypeName("struct ImportInfo *")] ImportInfo* ok_out)
        {
            int __ret = ((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_process->module_import_by_name(&((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, info, name, ok_out);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_processinstance_module_export_by_name(void* self, [NativeTypeName("const struct ModuleInfo *")] ModuleInfo* info, [NativeTypeName("struct CSliceRef_u8")] CSliceRef_u8 name, [NativeTypeName("struct ExportInfo *")] ExportInfo* ok_out)
        {
            int __ret = ((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_process->module_export_by_name(&((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, info, name, ok_out);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_processinstance_module_section_by_name(void* self, [NativeTypeName("const struct ModuleInfo *")] ModuleInfo* info, [NativeTypeName("struct CSliceRef_u8")] CSliceRef_u8 name, [NativeTypeName("struct SectionInfo *")] SectionInfo* ok_out)
        {
            int __ret = ((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_process->module_section_by_name(&((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, info, name, ok_out);

            return __ret;
        }

        [return: NativeTypeName("const struct ProcessInfo *")]
        public static ProcessInfo* mf_processinstance_info([NativeTypeName("const void *")] void* self)
        {
            ProcessInfo* __ret = ((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_process->info(&((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->container);

            return __ret;
        }

        public static void mf_processinstance_mapped_mem_range(void* self, [NativeTypeName("imem")] long gap_size, [NativeTypeName("Address")] ulong start, [NativeTypeName("Address")] ulong end, [NativeTypeName("MemoryRangeCallback")] Callback_c_void__MemoryRange @out)
        {
            ((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_process->mapped_mem_range(&((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, gap_size, start, end, @out);
        }

        public static void mf_processinstance_mapped_mem(void* self, [NativeTypeName("imem")] long gap_size, [NativeTypeName("MemoryRangeCallback")] Callback_c_void__MemoryRange @out)
        {
            ((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_process->mapped_mem(&((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, gap_size, @out);
        }

        public static void mf_processinstance_virt_to_phys_list(void* self, [NativeTypeName("struct CSliceRef_VtopRange")] CSliceRef_VtopRange addrs, [NativeTypeName("VirtualTranslationCallback")] Callback_c_void__VirtualTranslation @out, [NativeTypeName("VirtualTranslationFailCallback")] Callback_c_void__VirtualTranslationFail out_fail)
        {
            ((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_virtualtranslate->virt_to_phys_list(&((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, addrs, @out, out_fail);
        }

        public static void mf_processinstance_virt_to_phys_range(void* self, [NativeTypeName("Address")] ulong start, [NativeTypeName("Address")] ulong end, [NativeTypeName("VirtualTranslationCallback")] Callback_c_void__VirtualTranslation @out)
        {
            ((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_virtualtranslate->virt_to_phys_range(&((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, start, end, @out);
        }

        public static void mf_processinstance_virt_translation_map_range(void* self, [NativeTypeName("Address")] ulong start, [NativeTypeName("Address")] ulong end, [NativeTypeName("VirtualTranslationCallback")] Callback_c_void__VirtualTranslation @out)
        {
            ((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_virtualtranslate->virt_translation_map_range(&((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, start, end, @out);
        }

        public static void mf_processinstance_virt_page_map_range(void* self, [NativeTypeName("imem")] long gap_size, [NativeTypeName("Address")] ulong start, [NativeTypeName("Address")] ulong end, [NativeTypeName("MemoryRangeCallback")] Callback_c_void__MemoryRange @out)
        {
            ((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_virtualtranslate->virt_page_map_range(&((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, gap_size, start, end, @out);
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_processinstance_virt_to_phys(void* self, [NativeTypeName("Address")] ulong address, [NativeTypeName("struct PhysicalAddress *")] PhysicalAddress* ok_out)
        {
            int __ret = ((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_virtualtranslate->virt_to_phys(&((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, address, ok_out);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_processinstance_virt_page_info(void* self, [NativeTypeName("Address")] ulong addr, [NativeTypeName("struct Page *")] Page* ok_out)
        {
            int __ret = ((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_virtualtranslate->virt_page_info(&((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, addr, ok_out);

            return __ret;
        }

        public static void mf_processinstance_virt_translation_map(void* self, [NativeTypeName("VirtualTranslationCallback")] Callback_c_void__VirtualTranslation @out)
        {
            ((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_virtualtranslate->virt_translation_map(&((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, @out);
        }

        [return: NativeTypeName("struct COption_Address")]
        public static COption_Address mf_processinstance_phys_to_virt(void* self, [NativeTypeName("Address")] ulong phys)
        {
            COption_Address __ret = ((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_virtualtranslate->phys_to_virt(&((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, phys);

            return __ret;
        }

        public static void mf_processinstance_virt_page_map(void* self, [NativeTypeName("imem")] long gap_size, [NativeTypeName("MemoryRangeCallback")] Callback_c_void__MemoryRange @out)
        {
            ((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_virtualtranslate->virt_page_map(&((ProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, gap_size, @out);
        }

        [return: NativeTypeName("struct IntoProcessInstance_CBox_c_void_____CArc_c_void")]
        public static IntoProcessInstance_CBox_c_void_____CArc_c_void mf_intoprocessinstance_clone([NativeTypeName("const void *")] void* self)
        {
            IntoProcessInstance_CBox_c_void_____CArc_c_void __ret = new IntoProcessInstance_CBox_c_void_____CArc_c_void();

            __ret.container=(((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_clone->clone(&((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->container));
            return __ret;
        }

        public static void mf_intoprocessinstance_drop([NativeTypeName("struct IntoProcessInstance_CBox_c_void_____CArc_c_void")] IntoProcessInstance_CBox_c_void_____CArc_c_void self)
        {
            cont_box_drop(&self.container.instance);
            ctx_arc_drop(&self.container.context);
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_intoprocessinstance_read_raw_iter(void* self, [NativeTypeName("ReadRawMemOps")] MemOps_ReadDataRaw__ReadData data)
        {
            int __ret = ((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_memoryview->read_raw_iter(&((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, data);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_intoprocessinstance_write_raw_iter(void* self, [NativeTypeName("WriteRawMemOps")] MemOps_WriteDataRaw__WriteData data)
        {
            int __ret = ((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_memoryview->write_raw_iter(&((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, data);

            return __ret;
        }

        [return: NativeTypeName("struct MemoryViewMetadata")]
        public static MemoryViewMetadata mf_intoprocessinstance_metadata([NativeTypeName("const void *")] void* self)
        {
            MemoryViewMetadata __ret = ((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_memoryview->metadata(&((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->container);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_intoprocessinstance_read_iter(void* self, [NativeTypeName("struct CIterator_ReadData")] CIterator_ReadData inp, [NativeTypeName("ReadCallback *")] Callback_c_void__ReadData* @out, [NativeTypeName("ReadCallback *")] Callback_c_void__ReadData* out_fail)
        {
            int __ret = ((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_memoryview->read_iter(&((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, inp, @out, out_fail);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_intoprocessinstance_read_raw_list(void* self, [NativeTypeName("struct CSliceMut_ReadData")] CSliceMut_ReadData data)
        {
            int __ret = ((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_memoryview->read_raw_list(&((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, data);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_intoprocessinstance_read_raw_into(void* self, [NativeTypeName("Address")] ulong addr, [NativeTypeName("struct CSliceMut_u8")] CSliceMut_u8 @out)
        {
            int __ret = ((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_memoryview->read_raw_into(&((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, addr, @out);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_intoprocessinstance_write_iter(void* self, [NativeTypeName("struct CIterator_WriteData")] CIterator_WriteData inp, [NativeTypeName("WriteCallback *")] Callback_c_void__WriteData* @out, [NativeTypeName("WriteCallback *")] Callback_c_void__WriteData* out_fail)
        {
            int __ret = ((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_memoryview->write_iter(&((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, inp, @out, out_fail);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_intoprocessinstance_write_raw_list(void* self, [NativeTypeName("struct CSliceRef_WriteData")] CSliceRef_WriteData data)
        {
            int __ret = ((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_memoryview->write_raw_list(&((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, data);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_intoprocessinstance_write_raw(void* self, [NativeTypeName("Address")] ulong addr, [NativeTypeName("struct CSliceRef_u8")] CSliceRef_u8 data)
        {
            int __ret = ((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_memoryview->write_raw(&((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, addr, data);

            return __ret;
        }

        [return: NativeTypeName("struct ProcessState")]
        public static ProcessState mf_intoprocessinstance_state(void* self)
        {
            ProcessState __ret = ((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_process->state(&((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->container);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_intoprocessinstance_module_address_list_callback(void* self, [NativeTypeName("const struct ArchitectureIdent *")] ArchitectureIdent* target_arch, [NativeTypeName("ModuleAddressCallback")] Callback_c_void__ModuleAddressInfo callback)
        {
            int __ret = ((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_process->module_address_list_callback(&((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, target_arch, callback);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_intoprocessinstance_module_list_callback(void* self, [NativeTypeName("const struct ArchitectureIdent *")] ArchitectureIdent* target_arch, [NativeTypeName("ModuleInfoCallback")] Callback_c_void__ModuleInfo callback)
        {
            int __ret = ((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_process->module_list_callback(&((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, target_arch, callback);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_intoprocessinstance_module_by_address(void* self, [NativeTypeName("Address")] ulong address, [NativeTypeName("struct ArchitectureIdent")] ArchitectureIdent architecture, [NativeTypeName("struct ModuleInfo *")] ModuleInfo* ok_out)
        {
            int __ret = ((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_process->module_by_address(&((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, address, architecture, ok_out);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_intoprocessinstance_module_by_name_arch(void* self, [NativeTypeName("struct CSliceRef_u8")] CSliceRef_u8 name, [NativeTypeName("const struct ArchitectureIdent *")] ArchitectureIdent* architecture, [NativeTypeName("struct ModuleInfo *")] ModuleInfo* ok_out)
        {
            int __ret = ((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_process->module_by_name_arch(&((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, name, architecture, ok_out);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_intoprocessinstance_module_by_name(void* self, [NativeTypeName("struct CSliceRef_u8")] CSliceRef_u8 name, [NativeTypeName("struct ModuleInfo *")] ModuleInfo* ok_out)
        {
            int __ret = ((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_process->module_by_name(&((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, name, ok_out);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_intoprocessinstance_primary_module_address(void* self, [NativeTypeName("Address *")] ulong* ok_out)
        {
            int __ret = ((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_process->primary_module_address(&((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, ok_out);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_intoprocessinstance_primary_module(void* self, [NativeTypeName("struct ModuleInfo *")] ModuleInfo* ok_out)
        {
            int __ret = ((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_process->primary_module(&((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, ok_out);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_intoprocessinstance_module_import_list_callback(void* self, [NativeTypeName("const struct ModuleInfo *")] ModuleInfo* info, [NativeTypeName("ImportCallback")] Callback_c_void__ImportInfo callback)
        {
            int __ret = ((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_process->module_import_list_callback(&((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, info, callback);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_intoprocessinstance_module_export_list_callback(void* self, [NativeTypeName("const struct ModuleInfo *")] ModuleInfo* info, [NativeTypeName("ExportCallback")] Callback_c_void__ExportInfo callback)
        {
            int __ret = ((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_process->module_export_list_callback(&((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, info, callback);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_intoprocessinstance_module_section_list_callback(void* self, [NativeTypeName("const struct ModuleInfo *")] ModuleInfo* info, [NativeTypeName("SectionCallback")] Callback_c_void__SectionInfo callback)
        {
            int __ret = ((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_process->module_section_list_callback(&((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, info, callback);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_intoprocessinstance_module_import_by_name(void* self, [NativeTypeName("const struct ModuleInfo *")] ModuleInfo* info, [NativeTypeName("struct CSliceRef_u8")] CSliceRef_u8 name, [NativeTypeName("struct ImportInfo *")] ImportInfo* ok_out)
        {
            int __ret = ((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_process->module_import_by_name(&((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, info, name, ok_out);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_intoprocessinstance_module_export_by_name(void* self, [NativeTypeName("const struct ModuleInfo *")] ModuleInfo* info, [NativeTypeName("struct CSliceRef_u8")] CSliceRef_u8 name, [NativeTypeName("struct ExportInfo *")] ExportInfo* ok_out)
        {
            int __ret = ((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_process->module_export_by_name(&((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, info, name, ok_out);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_intoprocessinstance_module_section_by_name(void* self, [NativeTypeName("const struct ModuleInfo *")] ModuleInfo* info, [NativeTypeName("struct CSliceRef_u8")] CSliceRef_u8 name, [NativeTypeName("struct SectionInfo *")] SectionInfo* ok_out)
        {
            int __ret = ((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_process->module_section_by_name(&((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, info, name, ok_out);

            return __ret;
        }

        [return: NativeTypeName("const struct ProcessInfo *")]
        public static ProcessInfo* mf_intoprocessinstance_info([NativeTypeName("const void *")] void* self)
        {
            ProcessInfo* __ret = ((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_process->info(&((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->container);

            return __ret;
        }

        public static void mf_intoprocessinstance_mapped_mem_range(void* self, [NativeTypeName("imem")] long gap_size, [NativeTypeName("Address")] ulong start, [NativeTypeName("Address")] ulong end, [NativeTypeName("MemoryRangeCallback")] Callback_c_void__MemoryRange @out)
        {
            ((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_process->mapped_mem_range(&((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, gap_size, start, end, @out);
        }

        public static void mf_intoprocessinstance_mapped_mem(void* self, [NativeTypeName("imem")] long gap_size, [NativeTypeName("MemoryRangeCallback")] Callback_c_void__MemoryRange @out)
        {
            ((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_process->mapped_mem(&((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, gap_size, @out);
        }

        public static void mf_intoprocessinstance_virt_to_phys_list(void* self, [NativeTypeName("struct CSliceRef_VtopRange")] CSliceRef_VtopRange addrs, [NativeTypeName("VirtualTranslationCallback")] Callback_c_void__VirtualTranslation @out, [NativeTypeName("VirtualTranslationFailCallback")] Callback_c_void__VirtualTranslationFail out_fail)
        {
            ((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_virtualtranslate->virt_to_phys_list(&((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, addrs, @out, out_fail);
        }

        public static void mf_intoprocessinstance_virt_to_phys_range(void* self, [NativeTypeName("Address")] ulong start, [NativeTypeName("Address")] ulong end, [NativeTypeName("VirtualTranslationCallback")] Callback_c_void__VirtualTranslation @out)
        {
            ((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_virtualtranslate->virt_to_phys_range(&((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, start, end, @out);
        }

        public static void mf_intoprocessinstance_virt_translation_map_range(void* self, [NativeTypeName("Address")] ulong start, [NativeTypeName("Address")] ulong end, [NativeTypeName("VirtualTranslationCallback")] Callback_c_void__VirtualTranslation @out)
        {
            ((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_virtualtranslate->virt_translation_map_range(&((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, start, end, @out);
        }

        public static void mf_intoprocessinstance_virt_page_map_range(void* self, [NativeTypeName("imem")] long gap_size, [NativeTypeName("Address")] ulong start, [NativeTypeName("Address")] ulong end, [NativeTypeName("MemoryRangeCallback")] Callback_c_void__MemoryRange @out)
        {
            ((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_virtualtranslate->virt_page_map_range(&((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, gap_size, start, end, @out);
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_intoprocessinstance_virt_to_phys(void* self, [NativeTypeName("Address")] ulong address, [NativeTypeName("struct PhysicalAddress *")] PhysicalAddress* ok_out)
        {
            int __ret = ((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_virtualtranslate->virt_to_phys(&((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, address, ok_out);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_intoprocessinstance_virt_page_info(void* self, [NativeTypeName("Address")] ulong addr, [NativeTypeName("struct Page *")] Page* ok_out)
        {
            int __ret = ((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_virtualtranslate->virt_page_info(&((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, addr, ok_out);

            return __ret;
        }

        public static void mf_intoprocessinstance_virt_translation_map(void* self, [NativeTypeName("VirtualTranslationCallback")] Callback_c_void__VirtualTranslation @out)
        {
            ((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_virtualtranslate->virt_translation_map(&((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, @out);
        }

        [return: NativeTypeName("struct COption_Address")]
        public static COption_Address mf_intoprocessinstance_phys_to_virt(void* self, [NativeTypeName("Address")] ulong phys)
        {
            COption_Address __ret = ((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_virtualtranslate->phys_to_virt(&((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, phys);

            return __ret;
        }

        public static void mf_intoprocessinstance_virt_page_map(void* self, [NativeTypeName("imem")] long gap_size, [NativeTypeName("MemoryRangeCallback")] Callback_c_void__MemoryRange @out)
        {
            ((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_virtualtranslate->virt_page_map(&((IntoProcessInstance_CBox_c_void_____CArc_c_void*)self)->container, gap_size, @out);
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_connectorinstance_phys_read_raw_iter(void* self, [NativeTypeName("PhysicalReadMemOps")] MemOps_PhysicalReadData__ReadData data)
        {
            int __ret = ((ConnectorInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_physicalmemory->phys_read_raw_iter(&((ConnectorInstance_CBox_c_void_____CArc_c_void*)self)->container, data);

            return __ret;
        }

        [return: NativeTypeName("int32_t")]
        public static int mf_connectorinstance_phys_write_raw_iter(void* self, [NativeTypeName("PhysicalWriteMemOps")] MemOps_PhysicalWriteData__WriteData data)
        {
            int __ret = ((ConnectorInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_physicalmemory->phys_write_raw_iter(&((ConnectorInstance_CBox_c_void_____CArc_c_void*)self)->container, data);

            return __ret;
        }

        [return: NativeTypeName("struct PhysicalMemoryMetadata")]
        public static PhysicalMemoryMetadata mf_connectorinstance_metadata([NativeTypeName("const void *")] void* self)
        {
            PhysicalMemoryMetadata __ret = ((ConnectorInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_physicalmemory->metadata(&((ConnectorInstance_CBox_c_void_____CArc_c_void*)self)->container);

            return __ret;
        }

        public static void mf_connectorinstance_set_mem_map(void* self, [NativeTypeName("struct CSliceRef_PhysicalMemoryMapping")] CSliceRef_PhysicalMemoryMapping _mem_map)
        {
            ((ConnectorInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_physicalmemory->set_mem_map(&((ConnectorInstance_CBox_c_void_____CArc_c_void*)self)->container, _mem_map);
        }

        [return: NativeTypeName("MemoryViewBase_CBox_c_void_____CArc_c_void")]
        public static CGlueTraitObj_CBox_c_void_____MemoryViewVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void______________CArc_c_void_____MemoryViewRetTmp_CArc_c_void mf_connectorinstance_into_phys_view([NativeTypeName("struct ConnectorInstance_CBox_c_void_____CArc_c_void")] ConnectorInstance_CBox_c_void_____CArc_c_void self)
        {
            CArc_c_void ___ctx = ctx_arc_clone(&self.container.context);
            CGlueTraitObj_CBox_c_void_____MemoryViewVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void______________CArc_c_void_____MemoryViewRetTmp_CArc_c_void __ret = (self.vtbl_physicalmemory)->into_phys_view(self.container);

            ctx_arc_drop(&___ctx);
            return __ret;
        }

        [return: NativeTypeName("MemoryViewBase_CBox_c_void_____CArc_c_void")]
        public static CGlueTraitObj_CBox_c_void_____MemoryViewVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void______________CArc_c_void_____MemoryViewRetTmp_CArc_c_void mf_connectorinstance_phys_view(void* self)
        {
            CGlueTraitObj_CBox_c_void_____MemoryViewVtbl_CGlueObjContainer_CBox_c_void_____CArc_c_void_____MemoryViewRetTmp_CArc_c_void______________CArc_c_void_____MemoryViewRetTmp_CArc_c_void __ret = ((ConnectorInstance_CBox_c_void_____CArc_c_void*)self)->vtbl_physicalmemory->phys_view(&((ConnectorInstance_CBox_c_void_____CArc_c_void*)self)->container);

            return __ret;
        }

        public static bool cb_collect_static_base([NativeTypeName("struct CollectBase *")] CollectBase* ctx, [NativeTypeName("size_t")] nuint elem_size, void* info)
        {
            if (ctx->size < ctx->capacity)
            {
                Unsafe.CopyBlockUnaligned(ctx->buf + elem_size * ctx->size++, info, (uint)elem_size);
            }

            return ctx->size < ctx->capacity;
        }

        public static bool cb_collect_dynamic_base([NativeTypeName("struct CollectBase *")] CollectBase* ctx, [NativeTypeName("size_t")] nuint elem_size, void* info)
        {
            if (ctx->buf == null || ctx->size >= ctx->capacity)
            {
                nuint new_capacity = (ctx->buf) != null ? ctx->capacity * 2 : 64;
                sbyte* buf = (sbyte*)(NativeMemory.Realloc(ctx->buf, elem_size * new_capacity));

                if ((unchecked(buf)) != null)
                {
                    ctx->buf = buf;
                    ctx->capacity = new_capacity;
                }
            }

            if (ctx->buf == null || ctx->size >= ctx->capacity)
            {
                return false;
            }

            Unsafe.CopyBlockUnaligned(ctx->buf + elem_size * ctx->size++, info, (uint)elem_size);
            return true;
        }

        public static bool buf_iter_next([NativeTypeName("struct BufferIterator *")] BufferIterator* iter, void* @out)
        {
            if (iter->i >= iter->size)
            {
                return (1) != 0;
            }

            Unsafe.CopyBlockUnaligned(@out, iter->buf + iter->i++ * iter->sz_elem, (uint)iter->sz_elem);
            return (0) != 0;
        }

        public static bool cb_collect_static_ReadData([NativeTypeName("struct CollectBase *")] CollectBase* ctx, [NativeTypeName("ReadData")] CTup2_Address__CSliceMut_u8 info)
        {
            return cb_collect_static_base(ctx, (uint)sizeof(CTup2_Address__CSliceMut_u8), &info);
        }

        public static bool cb_collect_dynamic_ReadData([NativeTypeName("struct CollectBase *")] CollectBase* ctx, [NativeTypeName("ReadData")] CTup2_Address__CSliceMut_u8 info)
        {
            return cb_collect_dynamic_base(ctx, (uint)sizeof(CTup2_Address__CSliceMut_u8), &info);
        }

        public static bool cb_count_ReadData([NativeTypeName("size_t *")] nuint* cnt, [NativeTypeName("ReadData")] CTup2_Address__CSliceMut_u8 info)
        {
            return (++(*cnt)) != 0;
        }

        public static bool cb_collect_static_WriteData([NativeTypeName("struct CollectBase *")] CollectBase* ctx, [NativeTypeName("WriteData")] CTup2_Address__CSliceRef_u8 info)
        {
            return cb_collect_static_base(ctx, (uint)sizeof(CTup2_Address__CSliceRef_u8), &info);
        }

        public static bool cb_collect_dynamic_WriteData([NativeTypeName("struct CollectBase *")] CollectBase* ctx, [NativeTypeName("WriteData")] CTup2_Address__CSliceRef_u8 info)
        {
            return cb_collect_dynamic_base(ctx, (uint)sizeof(CTup2_Address__CSliceRef_u8), &info);
        }

        public static bool cb_count_WriteData([NativeTypeName("size_t *")] nuint* cnt, [NativeTypeName("WriteData")] CTup2_Address__CSliceRef_u8 info)
        {
            return (++(*cnt)) != 0;
        }

        public static bool cb_collect_static_Address([NativeTypeName("struct CollectBase *")] CollectBase* ctx, [NativeTypeName("Address")] ulong info)
        {
            return cb_collect_static_base(ctx, sizeof(ulong), &info);
        }

        public static bool cb_collect_dynamic_Address([NativeTypeName("struct CollectBase *")] CollectBase* ctx, [NativeTypeName("Address")] ulong info)
        {
            return cb_collect_dynamic_base(ctx, sizeof(ulong), &info);
        }

        public static bool cb_count_Address([NativeTypeName("size_t *")] nuint* cnt, [NativeTypeName("Address")] ulong info)
        {
            return (++(*cnt)) != 0;
        }

        public static bool cb_collect_static_ProcessInfo([NativeTypeName("struct CollectBase *")] CollectBase* ctx, ProcessInfo info)
        {
            return cb_collect_static_base(ctx, (uint)sizeof(ProcessInfo), &info);
        }

        public static bool cb_collect_dynamic_ProcessInfo([NativeTypeName("struct CollectBase *")] CollectBase* ctx, ProcessInfo info)
        {
            return cb_collect_dynamic_base(ctx, (uint)sizeof(ProcessInfo), &info);
        }

        public static bool cb_count_ProcessInfo([NativeTypeName("size_t *")] nuint* cnt, ProcessInfo info)
        {
            return (++(*cnt)) != 0;
        }

        public static bool cb_collect_static_ModuleAddressInfo([NativeTypeName("struct CollectBase *")] CollectBase* ctx, ModuleAddressInfo info)
        {
            return cb_collect_static_base(ctx, (uint)sizeof(ModuleAddressInfo), &info);
        }

        public static bool cb_collect_dynamic_ModuleAddressInfo([NativeTypeName("struct CollectBase *")] CollectBase* ctx, ModuleAddressInfo info)
        {
            return cb_collect_dynamic_base(ctx, (uint)sizeof(ModuleAddressInfo), &info);
        }

        public static bool cb_count_ModuleAddressInfo([NativeTypeName("size_t *")] nuint* cnt, ModuleAddressInfo info)
        {
            return (++(*cnt)) != 0;
        }

        public static bool cb_collect_static_ModuleInfo([NativeTypeName("struct CollectBase *")] CollectBase* ctx, ModuleInfo info)
        {
            return cb_collect_static_base(ctx, (uint)sizeof(ModuleInfo), &info);
        }

        public static bool cb_collect_dynamic_ModuleInfo([NativeTypeName("struct CollectBase *")] CollectBase* ctx, ModuleInfo info)
        {
            return cb_collect_dynamic_base(ctx, (uint)sizeof(ModuleInfo), &info);
        }

        public static bool cb_count_ModuleInfo([NativeTypeName("size_t *")] nuint* cnt, ModuleInfo info)
        {
            return (++(*cnt)) != 0;
        }

        public static bool cb_collect_static_ImportInfo([NativeTypeName("struct CollectBase *")] CollectBase* ctx, ImportInfo info)
        {
            return cb_collect_static_base(ctx, (uint)sizeof(ImportInfo), &info);
        }

        public static bool cb_collect_dynamic_ImportInfo([NativeTypeName("struct CollectBase *")] CollectBase* ctx, ImportInfo info)
        {
            return cb_collect_dynamic_base(ctx, (uint)sizeof(ImportInfo), &info);
        }

        public static bool cb_count_ImportInfo([NativeTypeName("size_t *")] nuint* cnt, ImportInfo info)
        {
            return (++(*cnt)) != 0;
        }

        public static bool cb_collect_static_ExportInfo([NativeTypeName("struct CollectBase *")] CollectBase* ctx, ExportInfo info)
        {
            return cb_collect_static_base(ctx, (uint)sizeof(ExportInfo), &info);
        }

        public static bool cb_collect_dynamic_ExportInfo([NativeTypeName("struct CollectBase *")] CollectBase* ctx, ExportInfo info)
        {
            return cb_collect_dynamic_base(ctx, (uint)sizeof(ExportInfo), &info);
        }

        public static bool cb_count_ExportInfo([NativeTypeName("size_t *")] nuint* cnt, ExportInfo info)
        {
            return (++(*cnt)) != 0;
        }

        public static bool cb_collect_static_SectionInfo([NativeTypeName("struct CollectBase *")] CollectBase* ctx, SectionInfo info)
        {
            return cb_collect_static_base(ctx, (uint)sizeof(SectionInfo), &info);
        }

        public static bool cb_collect_dynamic_SectionInfo([NativeTypeName("struct CollectBase *")] CollectBase* ctx, SectionInfo info)
        {
            return cb_collect_dynamic_base(ctx, (uint)sizeof(SectionInfo), &info);
        }

        public static bool cb_count_SectionInfo([NativeTypeName("size_t *")] nuint* cnt, SectionInfo info)
        {
            return (++(*cnt)) != 0;
        }

        public static bool cb_collect_static_MemoryRange([NativeTypeName("struct CollectBase *")] CollectBase* ctx, [NativeTypeName("MemoryRange")] CTup3_Address__umem__PageType info)
        {
            return cb_collect_static_base(ctx, (uint)sizeof(CTup3_Address__umem__PageType), &info);
        }

        public static bool cb_collect_dynamic_MemoryRange([NativeTypeName("struct CollectBase *")] CollectBase* ctx, [NativeTypeName("MemoryRange")] CTup3_Address__umem__PageType info)
        {
            return cb_collect_dynamic_base(ctx, (uint)sizeof(CTup3_Address__umem__PageType), &info);
        }

        public static bool cb_count_MemoryRange([NativeTypeName("size_t *")] nuint* cnt, [NativeTypeName("MemoryRange")] CTup3_Address__umem__PageType info)
        {
            return (++(*cnt)) != 0;
        }

        public static bool cb_collect_static_VirtualTranslation([NativeTypeName("struct CollectBase *")] CollectBase* ctx, VirtualTranslation info)
        {
            return cb_collect_static_base(ctx, (uint)sizeof(VirtualTranslation), &info);
        }

        public static bool cb_collect_dynamic_VirtualTranslation([NativeTypeName("struct CollectBase *")] CollectBase* ctx, VirtualTranslation info)
        {
            return cb_collect_dynamic_base(ctx, (uint)sizeof(VirtualTranslation), &info);
        }

        public static bool cb_count_VirtualTranslation([NativeTypeName("size_t *")] nuint* cnt, VirtualTranslation info)
        {
            return (++(*cnt)) != 0;
        }

        public static bool cb_collect_static_VirtualTranslationFail([NativeTypeName("struct CollectBase *")] CollectBase* ctx, VirtualTranslationFail info)
        {
            return cb_collect_static_base(ctx, (uint)sizeof(VirtualTranslationFail), &info);
        }

        public static bool cb_collect_dynamic_VirtualTranslationFail([NativeTypeName("struct CollectBase *")] CollectBase* ctx, VirtualTranslationFail info)
        {
            return cb_collect_dynamic_base(ctx, (uint)sizeof(VirtualTranslationFail), &info);
        }

        public static bool cb_count_VirtualTranslationFail([NativeTypeName("size_t *")] nuint* cnt, VirtualTranslationFail info)
        {
            return (++(*cnt)) != 0;
        }

        [NativeTypeName("#define PageType_NONE 0")]
        public const int PageType_NONE = 0;

        [NativeTypeName("#define PageType_UNKNOWN 1")]
        public const int PageType_UNKNOWN = 1;

        [NativeTypeName("#define PageType_PAGE_TABLE 2")]
        public const int PageType_PAGE_TABLE = 2;

        [NativeTypeName("#define PageType_WRITEABLE 4")]
        public const int PageType_WRITEABLE = 4;

        [NativeTypeName("#define PageType_READ_ONLY 8")]
        public const int PageType_READ_ONLY = 8;

        [NativeTypeName("#define PageType_NOEXEC 16")]
        public const int PageType_NOEXEC = 16;
    }
}