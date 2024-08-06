@echo off

set projName=rEFInd-Automenu.ConsoleApplication
set profilesDir=%projName%\Properties\PublishProfiles\

echo Building x64_Standalone...
dotnet publish %projName%\%projName%.csproj /p:PublishProfile=%profilesDir%\FolderProfile_x64_Standalone.pubxml > nul
echo Building x86_Standalone...
dotnet publish %projName%\%projName%.csproj /p:PublishProfile=%profilesDir%\FolderProfile_x86_Standalone.pubxml > nul
echo Building ARM64_Standalone...
dotnet publish %projName%\%projName%.csproj /p:PublishProfile=%profilesDir%\FolderProfile_ARM64_Standalone.pubxml > nul

echo Archiving...
7z a -tzip -mx3 %cd%\bin\console\win-arm64-standalone.zip %cd%\bin\console\win-arm64-s\refind.exe > nul
7z a -tzip -mx3 %cd%\bin\console\win-x64-standalone.zip %cd%\bin\console\win-x64-s\refind.exe > nul
7z a -tzip -mx3 %cd%\bin\console\win-x86-standalone.zip %cd%\bin\console\win-x86-s\refind.exe > nul