@echo "MIGRATIONS"
@echo off

cd /D "%~dp0"
call start\stop.bat nopause
dotnet build ..\asc.web.slnf
dotnet build ..\ASC.Migrations.sln
PUSHD %~dp0..\common\Tools\ASC.Migration.Runner\bin\Debug\net7.0
dotnet ASC.Migration.Runner.dll standalone=true
pause