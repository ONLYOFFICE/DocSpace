PUSHD %~dp0
call runasadmin.bat "%~dpnx0"
if %errorlevel% == 0 (

call start\stop.bat

PUSHD %~dp0..

echo "FRONT-END (for start run command 'yarn start' inside the root folder)"
call build\build.frontend.bat

echo "BACK-END"
call build\build.backend.bat

start /b call build\start\start.bat

pause
)