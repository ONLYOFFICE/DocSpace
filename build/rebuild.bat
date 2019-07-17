PUSHD %~dp0
call start\stop.bat

PUSHD %~dp0..
echo "REMOVE npm-local"
del /s /q web\npm-local

PUSHD %~dp0..
echo "ASC.Web.Components"
cd web/ASC.Web.Components
call npm i

PUSHD %~dp0..
echo "ASC.Web.Storybook"
cd web\ASC.Web.Storybook
call npm i

PUSHD %~dp0..
echo "ASC.Web.Client"
cd web\ASC.Web.Client
call npm i

PUSHD %~dp0..
echo "ASC.Web.People.Client"
cd products/ASC.People/Client 
call npm i

PUSHD %~dp0..
xcopy build\cra\*.* products\ASC.People\Client\node_modules\ /E /R /Y

echo "ASC.Web.sln"
call dotnet build ASC.Web.sln  /fl1 /flp1:LogFile=build/ASC.Web.log;Verbosity=Normal

PUSHD %~dp0
start /b call start\start.bat

pause