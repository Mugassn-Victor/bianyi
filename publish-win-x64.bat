@echo off
setlocal
cd /d "%~dp0"

echo [1/3] Restoring packages...
dotnet restore
if errorlevel 1 goto :error

echo [2/3] Publishing Windows x64 EXE...
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o "%~dp0publish-temp"
if errorlevel 1 goto :error

echo [3/3] Copying EXE beside this BAT file...
copy /y "%~dp0publish-temp\H2LinuxMultiTerminal.exe" "%~dp0H2LinuxMultiTerminal.exe" >nul
if errorlevel 1 goto :error

rmdir /s /q "%~dp0publish-temp" 2>nul

echo.
echo Build complete:
echo %~dp0H2LinuxMultiTerminal.exe
pause
exit /b 0

:error
echo.
echo Build failed. Make sure .NET 8 SDK is installed.
pause
exit /b 1
