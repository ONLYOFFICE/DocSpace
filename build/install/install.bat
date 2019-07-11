PUSHD %~dp0
for /R "..\run\" %%f in (*.bat) do (
	start nssm install Onlyoffice%%~nf "%%~f"
)

pause