@echo off
cd /d "%~dp0"
dotnet restore CineStreamCR.sln
dotnet run --project CineStreamCR\CineStreamCR.csproj
pause
