PUSHD %~dp0..
dotnet build ASC.Web.slnf  /fl1 /flp1:LogFile=build/ASC.Web.log;Verbosity=Normal
@echo off
echo.
echo Install nodejs projects dependencies...
echo.

if %errorlevel% == 0 (
	for /R "build\scripts\" %%f in (*.bat) do (
		echo Run script %%~nxf...
		echo.
		call build\scripts\%%~nxf
	)
)

echo.

if "%1"=="nopause" goto start
pause
:start