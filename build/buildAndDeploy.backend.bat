@echo off
PUSHD %~dp0..
setlocal EnableDelayedExpansion

dotnet build ASC.Web.sln  /fl1 /flp1:LogFile=build/ASC.Web.log;Verbosity=Normal

if %errorlevel% == 0 (
	for /R "build\run\" %%f in (*.bat) do (
		call build\run\%%~nxf publish
		call dotnet -d publish -c Debug --no-restore --no-build --self-contained -o !servicepath! !servicesource!!servicename!.csproj
	)
)

if %errorlevel% == 0 (
	for /R "build\scripts\" %%f in (*.sh) do (
		echo "%%~nxf"
		call build\scripts\%%~nxf
	)
)