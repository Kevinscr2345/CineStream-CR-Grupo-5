@echo off
cd /d "%~dp0"
dotnet restore CineStreamCR.sln
dotnet build CineStreamCR.sln
pause
