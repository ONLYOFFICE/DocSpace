@echo off

PUSHD %~dp0..
call runasadmin.bat "%~dpnx0"

if %errorlevel% == 0 (
	for /R "..\common\services\winswConfigs\" %%f in (*.xml) do (
		call ..\thirdparty\winsw.exe stop %%f
	)
)

POPD

if %errorlevel% == 0 (
	pwsh  %~dp0/command.ps1 "stop"
)

echo.

if "%1"=="nopause" goto start
pause
:start