# QuantumTunnel
A Xbox One/Series Flash Dumper for SystemOS created in C#.

### Prerequisites
.NET 10 is required, for instructions on installing .NET to your console over SSH follow https://xboxoneresearch.github.io/wiki/development/installing-compatible-software/.

You must also have an administrator account, with a shell **elevated to SYSTEM** on your console in either Retail or Dev Mode to access the flash driver. 

### Usage
If dotnet isn't in your PATH:
* <dotnet path>\dotnet.exe <path to QuantumTunnelEx>\QuantumTunnelEx.dll -t FileToDump

If dotnet is in your PATH:
* dotnet QuantumTunnelEx.dll FileToDump

* f.e. dotnet <path to QuantumTunnelEx>\QuantumTunnelEx.dll -t certkeys.bin

To obtain a raw flash image, usable in XBFS tools, use --rawdump (1GBish on Series S/X and 5GBish on Xbox One):
* <dotnet path>\dotnet.exe <path to QuantumTunnelEx>\QuantumTunnelEx.dll -r -o XBFS.bin

If using the published/self-contained build, use `QuantumTunnelEx.exe` instead of `dotnet QuantumTunnelEx.dll



#### More examples of command lines:
* <dotnet path>\dotnet.exe <path to QuantumTunnelEx>\QuantumTunnelEx.dll -d \\.\Xvux\FlashFs -t devkit.ini -wd DUMP_FLASH
* <dotnet path>\dotnet.exe <path to QuantumTunnelEx>\QuantumTunnelEx.dll -d \\.\Xvux\FlashFs -t devkit.ini -o customdevkit.ini
* <dotnet path>\dotnet.exe <path to QuantumTunnelEx>\QuantumTunnelEx.dll -r (will output a filename flash.bin)
* <dotnet path>\dotnet.exe <path to QuantumTunnelEx>\QuantumTunnelEx.dll -r -o XBFS.bin
* <dotnet path>\dotnet.exe <path to QuantumTunnelEx>\QuantumTunnelEx.dll -t certkeys.bin
* <dotnet path>\dotnet.exe <path to QuantumTunnelEx>\QuantumTunnelEx.dll -t certkeys.bin -o certkeys_dump.bin

### FAQ
Q: Why can't I dump X file? (host.xvd, system.xvd, etc)
A: Files currently mounted or otherwise in use can't be read.

Q: How do I dump my developer certificate?
A: Read certkeys.bin

Q: Where can I find a list of file names?
A: Check out XVDTool's [source code](https://github.com/emoose/xvdtool/blob/master/LibXboxOne/NAND/XbfsFile.cs#L13)

Q: I get an access denied error
A: Ensure your shell is elevated to **SYSTEM privileges**.
