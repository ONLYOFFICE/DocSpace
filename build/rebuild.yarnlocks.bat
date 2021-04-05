PUSHD %~dp0..

del /f /q yarn.lock

start /b call build\rebuild.frontend.bat

