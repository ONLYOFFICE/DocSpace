@echo off

PUSHD %~dp0..
call runasadmin.bat "%~dpnx0"

POPD

if %errorlevel% == 0 (
	pwsh  %~dp0/command.ps1 "restart"
	
	for /R "%~dp0\..\..\common\services\winswConfigs\" %%f in (*.xml) do (
		call %~dp0\..\..\thirdparty\winsw.exe restart %%f
	)
)

echo.

if "%1"=="nopause" goto start
pause
:start