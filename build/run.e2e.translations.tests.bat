@echo off

PUSHD %~dp0
call runasadmin.bat "%~dpnx0"

if %errorlevel% == 0 (
PUSHD %~dp0..


echo "mode="


REM call yarn wipe
call yarn install

REM call yarn build
call yarn build:test.translation

REM call yarn wipe
call yarn deploy


REM copy nginx configurations to deploy folder
xcopy config\nginx\onlyoffice.conf build\deploy\nginx\ /E /R /Y
powershell -Command "(gc build\deploy\nginx\onlyoffice.conf) -replace '#', '' | Out-File -encoding ASCII build\deploy\nginx\onlyoffice.conf"

xcopy config\nginx\sites-enabled\* build\deploy\nginx\sites-enabled\ /E /R /Y

REM fix paths
powershell -Command "(gc build\deploy\nginx\sites-enabled\onlyoffice-calendar.conf) -replace 'ROOTPATH', '%~dp0deploy\products\ASC.Calendar\client' -replace '\\', '/' | Out-File -encoding ASCII build\deploy\nginx\sites-enabled\onlyoffice-calendar.conf"
powershell -Command "(gc build\deploy\nginx\sites-enabled\onlyoffice-crm.conf) -replace 'ROOTPATH', '%~dp0deploy\products\ASC.CRM\client' -replace '\\', '/' | Out-File -encoding ASCII build\deploy\nginx\sites-enabled\onlyoffice-crm.conf"
powershell -Command "(gc build\deploy\nginx\sites-enabled\onlyoffice-editor.conf) -replace 'ROOTPATH', '%~dp0deploy\products\ASC.Files\editor' -replace '\\', '/' | Out-File -encoding ASCII build\deploy\nginx\sites-enabled\onlyoffice-editor.conf"
powershell -Command "(gc build\deploy\nginx\sites-enabled\onlyoffice-files.conf) -replace 'ROOTPATH', '%~dp0deploy\products\ASC.Files\client' -replace '\\', '/' | Out-File -encoding ASCII build\deploy\nginx\sites-enabled\onlyoffice-files.conf"
powershell -Command "(gc build\deploy\nginx\sites-enabled\onlyoffice-login.conf) -replace 'ROOTPATH', '%~dp0deploy\studio\login' -replace '\\', '/' | Out-File -encoding ASCII build\deploy\nginx\sites-enabled\onlyoffice-login.conf"
powershell -Command "(gc build\deploy\nginx\sites-enabled\onlyoffice-mail.conf) -replace 'ROOTPATH', '%~dp0deploy\products\ASC.Mail\client' -replace '\\', '/' | Out-File -encoding ASCII build\deploy\nginx\sites-enabled\onlyoffice-mail.conf"
powershell -Command "(gc build\deploy\nginx\sites-enabled\onlyoffice-people.conf) -replace 'ROOTPATH', '%~dp0deploy\products\ASC.People\client' -replace '\\', '/' | Out-File -encoding ASCII build\deploy\nginx\sites-enabled\onlyoffice-people.conf"
powershell -Command "(gc build\deploy\nginx\sites-enabled\onlyoffice-projects.conf) -replace 'ROOTPATH', '%~dp0deploy\products\ASC.Projects\client' -replace '\\', '/' | Out-File -encoding ASCII build\deploy\nginx\sites-enabled\onlyoffice-projects.conf"
powershell -Command "(gc build\deploy\nginx\sites-enabled\onlyoffice-studio.conf) -replace 'ROOTPATH', '%~dp0deploy\studio\client' -replace '\\', '/' | Out-File -encoding ASCII build\deploy\nginx\sites-enabled\onlyoffice-studio.conf"

REM restart nginx
echo service nginx stop
call sc stop nginx > nul

REM sleep 5 seconds
call ping 127.0.0.1 -n 6 > nul

echo service nginx start
call sc start nginx > nul

REM sleep 5 seconds
call ping 127.0.0.1 -n 6 > nul

call yarn e2e.test:translation

exit



if NOT %errorlevel% == 0 (
	echo Couldn't restarte Onlyoffice%%~nf service			
)

)

echo.

POPD

if "%1"=="nopause" goto start
pause
:start