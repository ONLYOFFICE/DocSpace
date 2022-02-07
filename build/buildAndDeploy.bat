@echo off

echo "##########################################################"
echo "#########  Start build and deploy  #######################"
echo "##########################################################"

echo.

cd /D "%~dp0"
call runasadmin.bat "%~dpnx0"

if %errorlevel% == 0 (


echo "FRONT-END static"
call build.static.bat nopause

echo "BACK-END"
call build.backend.bat nopause

echo.

pause

)
