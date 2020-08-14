PUSHD %~dp0..
call runasadmin.bat "%~dpnx0"

if %errorlevel% == 0 (
	for /R "run\" %%f in (*.bat) do (
		call nssm stop Onlyoffice%%~nf
	)

  for /R "run\" %%f in (*.bat) do (
		call nssm start Onlyoffice%%~nf
	)

  call iisreset
)
