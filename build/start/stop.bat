@echo off

PUSHD %~dp0..
call runasadmin.bat "%~dpnx0"

if %errorlevel% == 0 (
	for /R "run\" %%f in (*.bat) do (
		call sc stop "Onlyoffice%%~nf" > nul

		if %errorlevel% == 0 (
			echo Onlyoffice%%~nf service has been stopped		
		) else (
			echo Couldn't stop Onlyoffice%%~nf service			
		)
	)
)

echo.

POPD

if "%1"=="nopause" goto start
pause
:start