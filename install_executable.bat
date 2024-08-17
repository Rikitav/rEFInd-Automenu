@echo off
set ProjectBuildingDirectory=%cd%\bin\console
set ExecutableInstallingDirectory=%winDir%
set ExecutableInstallingName=refindwin.exe

echo Prcoessor architecture : %PROCESSOR_ARCHITECTURE%
echo Executable building directory : %ProjectBuildingDirectory%
echo Executable installing path : %ExecutableInstallingDirectory%\%ExecutableInstallingName%

if %PROCESSOR_ARCHITECTURE% EQU AMD64 set ExePath=%ProjectBuildingDirectory%\win-x64-s\refind.exe
if %PROCESSOR_ARCHITECTURE% EQU X86 set ExePath=%ProjectBuildingDirectory%\win-x86-s\refind.exe
if %PROCESSOR_ARCHITECTURE% EQU ARM64 set ExePath=%ProjectBuildingDirectory%\win-arm64-s\refind.exe

echo Executable path : %ExePath%

if not exist %ExePath% (
	call build.bat
)

echo Installing...
copy /y %ExePath% %ExecutableInstallingDirectory%\%ExecutableInstallingName%