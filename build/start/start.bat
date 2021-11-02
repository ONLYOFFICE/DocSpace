@echo off

PUSHD %~dp0..
call runasadmin.bat "%~dpnx0"

if %errorlevel% == 0 (
	for /R "run\" %%f in (*.bat) do (
		call sc start "Onlyoffice%%~nf"
	)
)

echo.

if "%1"=="nopause" goto start
pause
:start