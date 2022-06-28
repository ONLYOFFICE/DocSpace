@echo off

echo "##########################################################"
echo "#########  Start build and deploy Personal  ##############"
echo "##########################################################"

echo.

PUSHD %~dp0
call runasadmin.bat "%~dpnx0"

if %errorlevel% == 0 (

call start\stop.bat nopause

PUSHD %~dp0..

echo "FRONT-END static"
call build\build.static.bat nopause personal

echo "BACK-END"
call build\build.backend.bat nopause

PUSHD %~dp0

call start\start.bat nopause

echo.

pause
)