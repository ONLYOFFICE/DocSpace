PUSHD %~dp0..
call runasadmin.bat "%~dpnx0"

if %errorlevel% == 0 (
	call stop.bat
	call stop.bat
    call iisreset
)
