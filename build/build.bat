PUSHD %~dp0
call runasadmin.bat "%~dpnx0"
if %errorlevel% == 0 (

call start\stop.bat

PUSHD %~dp0..

echo "ASC.Web.Components"
call build\scripts\components.sh

echo "ASC.Web.Common"
call build\scripts\common.sh

echo "ASC.Web.Client"
call build\scripts\client.sh

echo "ASC.Web.People.Client"
call build\scripts\people.sh

xcopy build\cra\*.* products\ASC.People\Client\node_modules\ /E /R /Y

echo "ASC.Web.sln"
call dotnet build ASC.Web.sln  /fl1 /flp1:LogFile=build/ASC.Web.log;Verbosity=Normal

start /b call build\start\start.bat

pause
)