@echo off
PUSHD %~dp0
setlocal EnableDelayedExpansion

call runasadmin.bat "%~dpnx0"

if %errorlevel% == 0 (

call start\stop.bat nopause

PUSHD %~dp0..

echo "FRONT-END static"
call build\build.static.bat nopause

echo "BACK-END"
call build\build.backend.bat nopause

call build\start\start.bat nopause

pause
)