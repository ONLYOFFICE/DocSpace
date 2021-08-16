PUSHD %~dp0..
call runasadmin.bat "%~dpnx0"

if %errorlevel% == 0 (  
    for /f %%f in ('dir /b run ^| findstr /v /i "\Client.bat$"') do (
		call nssm start Onlyoffice%%~nf
	)
)