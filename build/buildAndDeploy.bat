@echo off
PUSHD %~dp0
setlocal EnableDelayedExpansion

call runasadmin.bat "%~dpnx0"

if %errorlevel% == 0 (

rem call start\stop.bat

PUSHD %~dp0..

echo "FRONT-END static"
call build\build.static.bat

echo "BACK-END"
call build\buildAndDeploy.backend.bat

rem start /b call build\start\start.bat

pause
)