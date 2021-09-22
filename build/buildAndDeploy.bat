@echo off
PUSHD %~dp0
setlocal EnableDelayedExpansion

call runasadmin.bat "%~dpnx0"

if %errorlevel% == 0 (

rem call start\stop.bat

PUSHD %~dp0..

echo "FRONT-END static"
call build\build.static.bat

echo "BACK-END"
call build\build.backend.bat

if %errorlevel% == 0 (
	for /R "build\run\" %%f in (*.bat) do (
		call build\run\%%~nxf publish
		call dotnet -d publish -c Debug --no-restore --no-build --self-contained -o !servicepath! !servicesource!!servicename!.csproj
	)
)

rem start /b call build\start\start.bat

pause
)