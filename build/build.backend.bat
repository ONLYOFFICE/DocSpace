call build/createMigrations.bat
PUSHD %~dp0..
dotnet build ASC.Web.sln  /fl1 /flp1:LogFile=build/ASC.Web.log;Verbosity=Normal

@echo off

if %errorlevel% == 0 (
	for /R "build\scripts\" %%f in (*.sh) do (
		echo "%%~nxf"
		call build\scripts\%%~nxf
	)
)

pause