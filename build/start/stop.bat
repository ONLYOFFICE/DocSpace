PUSHD %~dp0
for /R "..\run\" %%f in (*.bat) do (
	start nssm stop Onlyoffice%%~nf
)

pause