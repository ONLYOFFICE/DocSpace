PUSHD %~dp0
call start\stop.bat

PUSHD %~dp0..

del /s /q web\npm-local

echo "ASC.Web.Components"
call yarn install --cwd web/ASC.Web.Components > build\ASC.Web.Components.log

echo "ASC.Web.Storybook"
call yarn install --cwd web/ASC.Web.Storybook  > build\ASC.Web.Storybook.log

echo "ASC.Web.Client"
call yarn install --cwd web/ASC.Web.Client > build\ASC.Web.Client.log

echo "ASC.Web.People.Client"
call yarn install --cwd products/ASC.People/Client > build\ASC.Web.People.Client.log

xcopy build\cra\*.* products\ASC.People\Client\node_modules\ /E /R /Y

echo "ASC.Web.sln"
call dotnet build ASC.Web.sln  /fl1 /flp1:LogFile=build/ASC.Web.log;Verbosity=Normal

start /b call build\start\start.bat

pause