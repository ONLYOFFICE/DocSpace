@echo off

echo "##########################################################"
echo "#########  Start build and deploy  #######################"
echo "##########################################################"

echo.

PUSHD %~dp0
setlocal EnableDelayedExpansion

call runasadmin.bat "%~dpnx0"

if %errorlevel% == 0 (

call start\stop.bat nopause

echo "FRONT-END static"
call build.static.bat nopause

echo "BACK-END"
call build.backend.bat nopause

call start\start.bat nopause

echo.

pause
)