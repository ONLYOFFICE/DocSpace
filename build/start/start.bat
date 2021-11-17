@echo off

PUSHD %~dp0..
call runasadmin.bat "%~dpnx0"

POPD
powershell  %~dp0/start.ps1

echo.

if "%1"=="nopause" goto start
pause
:start