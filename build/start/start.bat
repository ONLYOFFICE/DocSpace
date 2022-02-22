@echo off

PUSHD %~dp0..
call runasadmin.bat "%~dpnx0"


POPD

if %errorlevel% == 0 (
	pwsh  %~dp0/command.ps1 "start"
	
	for /R "%~dp0\..\..\common\services\winswConfigs\" %%f in (*.xml) do (
		call %~dp0\..\..\thirdparty\winsw.exe start %%f
	)
)

echo.

if "%1"=="nopause" goto start
pause
:start