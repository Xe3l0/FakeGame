@echo off
title FakeExe Installer
color 0A

echo.
echo  ================================
echo    FakeExe - Installing...
echo  ================================
echo.

:: Check if .NET is installed
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo  [ERROR] .NET SDK not found!
    echo.
    echo  Please install it from:
    echo  https://aka.ms/dotnet/download
    echo.
    pause
    start https://aka.ms/dotnet/download
    exit
)

echo  [1/2] Building FakeExe...
dotnet publish -c Release >nul 2>&1

if %errorlevel% neq 0 (
    echo  [ERROR] Build failed!
    pause
    exit
)

echo  [2/2] Copying to Desktop...

:: Get desktop path and copy exe
set "EXE=bin\Release\net6.0-windows\win-x64\publish\FakeExe.exe"
set "DESKTOP=%USERPROFILE%\Desktop\FakeExe.exe"

copy "%EXE%" "%DESKTOP%" >nul 2>&1

echo.
echo  ================================
echo    Done! FakeExe is on Desktop
echo  ================================
echo.

start "" "%DESKTOP%"
exit
