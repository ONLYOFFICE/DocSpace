PUSHD %~dp0..\..
setlocal EnableDelayedExpansion

if %errorlevel% == 0 (
	for /R "build\run\" %%f in (*.bat) do (
		call build\run\%%~nxf service
		echo !servicepath!
		call sc create "Onlyoffice %%~nf"  binPath="!servicepath!"
	)
)

pause