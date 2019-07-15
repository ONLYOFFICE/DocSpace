PUSHD %~dp0
call start\stop.bat

PUSHD %~dp0..
echo "ASC.Web.Components"
call npm ci --prefix web/ASC.Web.Components

echo "ASC.Web.Storybook"
call npm ci --prefix web/ASC.Web.Storybook

echo "ASC.Web.Client"
call npm ci --prefix web/ASC.Web.Client

echo "ASC.Web.People.Client"
call npm ci --prefix products/ASC.People/Client

xcopy build\cra\*.* products\ASC.People\Client\node_modules\ /E /R /Y

echo "ASC.Web.sln"
call dotnet build ASC.Web.sln  /fl1 /flp1:LogFile=build/ASC.Web.log;Verbosity=Normal

start /b call build\start\start.bat

pause