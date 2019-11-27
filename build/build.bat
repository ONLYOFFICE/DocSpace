PUSHD %~dp0
call runasadmin.bat "%~dpnx0"
if %errorlevel% == 0 (

call start\stop.bat

PUSHD %~dp0..

echo "ASC.Web.Components"
call yarn install --cwd web/ASC.Web.Components --frozen-lockfile > build\ASC.Web.Components.log
call yarn link --cwd packages/asc-web-components

echo "ASC.Web.Common"
call yarn link "asc-web-components" --cwd web/ASC.Web.Common
call yarn install --cwd web/ASC.Web.Common --frozen-lockfile > build\ASC.Web.Common.log
call yarn link --cwd packages/asc-web-common

echo "ASC.Web.Client"
call yarn link "asc-web-components" --cwd web/ASC.Web.Client
call yarn link "asc-web-common" --cwd web/ASC.Web.Client
call yarn install --cwd web/ASC.Web.Client --frozen-lockfile > build\ASC.Web.Client.log

echo "ASC.Web.People.Client"
call yarn link "asc-web-components" --cwd products/ASC.People/Client
call yarn link "asc-web-common" --cwd products/ASC.People/Client
call yarn install --cwd products/ASC.People/Client --frozen-lockfile > build\ASC.Web.People.Client.log

xcopy build\cra\*.* products\ASC.People\Client\node_modules\ /E /R /Y

echo "ASC.Web.sln"
call dotnet build ASC.Web.sln  /fl1 /flp1:LogFile=build/ASC.Web.log;Verbosity=Normal

start /b call build\start\start.bat

pause
)