@echo "MIGRATIONS"
@echo off

cd /D "%~dp0"
call start\stop.bat nopause
dotnet build ..\asc.web.sln 
dotnet build ..\ASC.Migrations.sln
PUSHD %~dp0..\common\Tools\ASC.Migration.Runner\bin\Debug\net6.0
dotnet ASC.Migration.Runner.dll
pause