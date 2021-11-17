@echo off

PUSHD %~dp0..
call runasadmin.bat "%~dpnx0"

if %errorlevel% == 0 (
	for /R "run\" %%f in (*.bat) do (
		call sc stop "Onlyoffice%%~nf" > nul
		call sc start "Onlyoffice%%~nf" > nul
		
		if %errorlevel% == 0 (
			echo Onlyoffice%%~nf service has been restarted		
		) else (
			echo Couldn't restarte Onlyoffice%%~nf service			
		)
	)      
)

POPD

echo.

if "%1"=="nopause" goto start
pause
:start