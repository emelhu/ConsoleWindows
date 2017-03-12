@echo off
echo --- clean output ---

cd ConsoleWindows
dotnet clean
cd ..

cd ConsoleWindowsDemo
dotnet clean
cd ..


cd CoreVirtualConsole
dotnet clean
cd ..

echo --- OK ---

pause