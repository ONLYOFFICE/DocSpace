@echo off

cd /D "%~dp0"
call runasadmin.bat "%~dpnx0"

if %errorlevel% == 0 (
PUSHD %~dp0..

IF "%2"=="personal" (
   echo "mode=%2"
) ELSE (
   echo "mode="
)

REM call yarn wipe
call yarn install

REM call yarn build
IF "%2"=="personal" (
    call yarn build:personal
) ELSE (
    call yarn build
)

REM call yarn wipe
IF "%2"=="personal" (
    call yarn deploy:personal
) ELSE (
    call yarn deploy
)

REM copy nginx configurations to deploy folder
xcopy config\nginx\onlyoffice.conf build\deploy\nginx\ /E /R /Y
powershell -Command "(gc build\deploy\nginx\onlyoffice.conf) -replace '#', '' | Out-File -encoding ASCII build\deploy\nginx\onlyoffice.conf"

xcopy config\nginx\sites-enabled\* build\deploy\nginx\sites-enabled\ /E /R /Y

REM fix paths
powershell -Command "(gc build\deploy\nginx\sites-enabled\onlyoffice-client.conf) -replace 'ROOTPATH', '%~dp0deploy\client' -replace '\\', '/' | Out-File -encoding ASCII build\deploy\nginx\sites-enabled\onlyoffice-client.conf"

REM restart nginx
echo service nginx stop
call sc stop nginx > nul

REM sleep 5 seconds
call ping 127.0.0.1 -n 6 > nul

echo service nginx start
call sc start nginx > nul

if NOT %errorlevel% == 0 (
	echo Couldn't restart Onlyoffice%%~nf service			
)

)

echo.

POPD

if "%1"=="nopause" goto start
pause
:start