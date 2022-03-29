@echo off

echo Start build backend...
echo.

cd /D "%~dp0"
call runasadmin.bat "%~dpnx0"

if %errorlevel% == 0 (
call start\stop.bat nopause
dotnet build ..\asc.web.slnf  /fl1 /flp1:logfile=asc.web.log;verbosity=normal
echo.

echo install nodejs projects dependencies...
echo.


for /R "scripts\" %%f in (*.bat) do (
 echo Run script %%~nxf...
 echo.
 call scripts\%%~nxf
)

echo.

call start\start.bat nopause

echo.

if "%1"=="nopause" goto end
pause

)

:end