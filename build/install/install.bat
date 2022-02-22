@echo off

PUSHD %~dp0..\..
setlocal EnableDelayedExpansion

if %errorlevel% == 0 (
	for /R "build\run\" %%f in (*.bat) do (
		call build\run\%%~nxf
		echo service create "Onlyoffice%%~nf"
		call sc create "Onlyoffice%%~nf" displayname= "ONLYOFFICE %%~nf" binPath= "!servicepath!"
	)
	for /R "build\run\" %%f in (*.xml) do (
		call build\install\win\WinSW3.0.0.exe install %%f
	)
)

echo.
pause