PUSHD %~dp0
echo "ASC.Web.Components"
cd ../web/ASC.Web.Components
call npm install

echo "ASC.Web.Components Storybook"
cd ../ASC.Web.Components/example
call npm install

echo "ASC.Web.sln"
cd ../../../
call dotnet build ASC.Web.sln  /fl1 /flp1:LogFile=build/ASC.Web.log;Verbosity=Normal

rem echo "ASC.People"
rem call dotnet build products/ASC.People --self-contained -r win10-x64 -o build/deploy/products/people  /fl1 /flp1:LogFile=build/ASC.People.log;Verbosity=Normal

rem echo "ASC.Web.Api"
rem call dotnet publish web/ASC.Web.Api --self-contained -r win10-x64 -o build/deploy/www/api  /fl1 /flp1:LogFile=build/ASC.Web.Api.log;Verbosity=Normal

rem xcopy config\*.* build\deploy\config\ /E /R /Y

pause