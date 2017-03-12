@echo off
echo --- build output ---

cd ConsoleWindows
dotnet build
cd ..

cd ConsoleWindowsDemo
dotnet build
cd ..


cd CoreVirtualConsole
dotnet build
cd ..

echo --- OK ---

pause