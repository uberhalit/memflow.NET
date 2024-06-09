# memflow.NET
A C# (.NET 8) wrapper for the [memflow-ffi](https://github.com/memflow/memflow/tree/main/memflow-ffi) crate >= 0.2.0.

## Installation
Compile the `libmemflow_ffi` library (see [Compiling the interopt library](#Compiling-the-interopt-library)) for your distro/OS and copy into the [memflow.NET/memflow.NET](https://github.com/uberhalit/memflow.NET/tree/main/memflow.NET/memflow.NET) folder. Then copy the entire [memflow.NET/memflow.NET](https://github.com/uberhalit/memflow.NET/tree/main/memflow.NET/memflow.NET) folder with its 2 class files and the freshly built library to your project root. 

## Usage
Import the `memflowNET` class like you'd import an ordinary class. The package contains abstract methods for the most used functionality of memflow. The `memflow.Interop` namespace further exposes all raw memflow structs and methods through `Methods`.
```cs
using memflowNET;
using memflowNET.Interop;
```
See [Example.cs](https://github.com/uberhalit/memflow.NET/blob/main/memflow.NET/Example.cs) for a short demo or [memflowNET.cs](https://github.com/uberhalit/memflow.NET/blob/main/memflow.NET/memflow.NET/memflowNET.cs) for the raw interop.

## Compiling the interopt library
Build the correct version of [memflow-ffi](https://github.com/memflow/memflow/tree/main/memflow-ffi) library:
```
git clone https://github.com/memflow/memflow.git --depth 1 --branch 0.2.X
cd memflow
cargo build --release --all-features --workspace
```

Navigate to `/memflow/target/release/` and copy the freshly built `libmemflow_ffi.so (Linux)` or `memflow_ffi.dll (Windows)` to `memflow.NET/`. This is the library used to interact with memflow. On windows you have to rename the library to `libmemflow_ffi.dll`.

## Installing Connectors
Use the latest [memflowup](https://github.com/memflow/memflowup) utility to install your connector. 

Linux: 
```
curl --proto '=https' --tlsv1.2 -sSf https://sh.memflow.io | sh
memflowup install memflow-win32 memflow-kvm memflow-qemu --system --dev --from-source
modprobe memflow
echo memflow >> /etc/modules-load.d/modules.conf
```
Windows: 
```
git clone https://github.com/memflow/memflowup.git
cd memflowup
cargo build --release --all-features
cd \target\release\
.\memflowup.exe install memflow-win32 memflow-pcileech --system --dev --from-source
```

## Creating Bindings (Windows)
To re-generate the bindings we use [ClangSharp](https://github.com/microsoft/ClangSharp) **on Windows**. Switch to Windows and execute: 
```
git clone https://github.com/microsoft/ClangSharp.git
cd ClangSharp
dotnet build -c Release
cd \artifacts\bin\sources\ClangSharpPInvokeGenerator\Release\net8.0\
```

Get the matching [ffi header file](https://github.com/memflow/memflow/blob/main/memflow-ffi/memflow.h) version to your `libmemflow_ffi` library and place it in that folder. Execute following to generate the wrapper:

```
.\ClangSharpPInvokeGenerator.exe -f memflow.h -n memflowNET.Interop -o memflowInterop.cs -l libmemflow_ffi -c preview-codegen unix-types generate-aggressive-inlining generate-macro-bindings
```
This file isn't read to run yet, replace the old [memflowInterop.cs](https://github.com/uberhalit/memflow.NET/blob/main/memflow.NET/memflow.NET/memflowInterop.cs) of this repo with the new one and run the included [CodeGenFix](https://github.com/uberhalit/memflow.NET/blob/main/CodeGenFix/Program.cs) tool. It will automatically fix all CodeGen issues and copy over comments.

If there are any remaining errors you can try to fix them manually or adjust the CodeGenFix utility:
* Copy over the definitions of `NativeTypeNameAttribute` and `NativeInheritanceAttribute` of this repo
* Fix some `bool` <-> `byte` conversations
* Remove `.operator=` and replace with regular `=`
* Replace `realloc()` with `NativeMemory.Realloc()`
* Change `nuint` to `uint` whenever the former causes a problem
* Add explicit casts to `uint` whenever neccessary
* Fix `void*` casts on helper methods
* Fix empty definitions for `PhysicalAddress_INVALID` and `Page_INVALID`
* Some structs require manual layouting through `[StructLayout(LayoutKind.Explicit)]`:
    * ProcessInfo

## Building Connectors
In case you want to compile and install the connector manually:

**kvm**
```
git clone https://github.com/memflow/memflow-kvm.git
cd memflow-kvm
./install.sh
cp target/release/libmemflow_kvm.so /usr/lib/memflow/libmemflow_kvm.so

wget https://github.com/memflow/memflow-kvm/releases/download/v0.2.0/memflow-dkms_0.1.7_all.deb
dpkg -i memflow-dkms_0.1.7_all.deb
modprobe memflow
echo memflow >> /etc/modules-load.d/modules.conf
```

Afterwards as non-root execute:
```
mkdir -p ~/.local/lib/memflow
cp memflow-kvm/target/release/libmemflow_kvm.so ~/.local/lib/memflow/libmemflow_kvm.so
```

**pcileech**
```
git clone https://github.com/memflow/memflow-pcileech.git
cd memflow-pcileech
git submodule update --init
cargo build --release --all-features
```

Copy `FTD3XX.dll` and `leechcore.dll` to `C:\Program Files\memflow\`.

Build and install the [memflow-win32](https://github.com/memflow/memflow-win32) library:
```
git clone https://github.com/memflow/memflow-win32.git
cd memflow-win32
cargo build --release --all-features --workspace
cp target/release/libmemflow_win32.so /usr/lib/memflow/libmemflow_win32.so
```

## Updating
To update any branches to latest changes run:
```
memflowup update --dev --system
git fetch
git pull
git submodule update --init --recursive
cargo update
```

## Performance
The helper class `memflowNET.cs` has been optimized to be close to native Rust performance. On an AMD 5900X through the kvm connector one can expect single-core performance between 12,000,000 and 15,000,000 `int` per second reading and similar in writing. This however depends heavily on the distro, VM setup and general hardware performance.

## Debugging
For debugging with VS Code you need to either follow the guide in [launch.json](https://github.com/uberhalit/memflow.NET/blob/main/.vscode/launch.json) to setup a local SSH and use `Debug SSH (root-less)` debug configuration or run VS Code as root directly and use `Debug (root only)` configuration (not recommended): 
```
code --user-data-dir="/home/$(whoami)/.vscode-root" --no-sandbox
```