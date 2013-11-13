REM run this script after running the 32bit 'Release' build of Rhino3dmIO.sln

REM check the paths.  The TargetDirs in the respective solutions might have changed
robocopy ..\..\dotnet\bin\x86\Release .\lib\net40 Rhino3dmIO.dll
robocopy ..\..\Release .\NativeBinaries\x86 rhino3dmio_native.dll

nuget pack Rhino3dmIO.dll.nuspec -NonInteractive

