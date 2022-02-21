@echo off

PUSHD %~dp0..\..
setlocal EnableDelayedExpansion

if %errorlevel% == 0 (
	for /R "build\run\" %%f in (*.bat) do (
		call build\run\%%~nxf
		echo service create "Onlyoffice%%~nf"
		call sc create "Onlyoffice%%~nf" displayname= "ONLYOFFICE %%~nf" binPath= "!servicepath!"
	)
	for /R "common\services\winswConfigs\" %%f in (*.xml) do (
		call thirdparty\winsw.exe install %%f
	)
)

echo.
pause