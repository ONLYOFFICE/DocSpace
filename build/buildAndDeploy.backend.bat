@echo off
PUSHD %~dp0..
setlocal EnableDelayedExpansion

call build\build.backend.bat

if %errorlevel% == 0 (
	for /R "build\run\" %%f in (*.bat) do (
		call build\run\%%~nxf publish
		call dotnet -d publish -c Debug --no-restore --no-build --self-contained -o !servicepath! !servicesource!!servicename!.csproj
	)
)
