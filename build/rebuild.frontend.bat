PUSHD %~dp0
call start\stop.bat

PUSHD %~dp0..

echo "BUILD FRONT-EDN"
call yarn install

start /b call build\start\start.bat

pause