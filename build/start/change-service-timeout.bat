@echo off

PUSHD %~dp0..
call runasadmin.bat "%~dpnx0"

POPD

if %errorlevel% == 0 (
	pwsh  %~dp0/command.ps1 "increase-service-timeout"
)

echo.

if "%1"=="nopause" goto start
pause
:start