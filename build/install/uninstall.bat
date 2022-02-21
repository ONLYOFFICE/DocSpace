@echo off

PUSHD %~dp0..
call runasadmin.bat "%~dpnx0"

if %errorlevel% == 0 (
	for /R "run\" %%f in (*.bat) do (
                echo Running service stop and delete "Onlyoffice%%~nf" 
		call sc stop "Onlyoffice%%~nf"
		call sc delete "Onlyoffice%%~nf"
	)
	for /R "..\common\services\winswConfigs\" %%f in (*.xml) do (
		call ..\thirdparty\winsw.exe stop %%f
		call ..\thirdparty\winsw.exe uninstall %%f
	)
)

echo.
pause