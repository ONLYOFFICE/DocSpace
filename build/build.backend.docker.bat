@echo off

if %errorlevel% == 0 (
	pwsh  %~dp0/build.backend.docker.ps1 "start"
)

echo.

pause