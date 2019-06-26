PUSHD %~dp0
echo "ASC.Web.Components"
cd ../web/ASC.Web.Components
call npm install

echo "ASC.Web.sln"
cd ../../
call dotnet build ASC.Web.sln  /fl1 /flp1:LogFile=build/ASC.Web.log;Verbosity=Normal

echo "ASC.People"
call dotnet publish products/ASC.People --self-contained -r win10-x64 -o build/deploy/products/people  /fl1 /flp1:LogFile=build/ASC.People.log;Verbosity=Normal

echo "ASC.Web.Api"
call dotnet publish web/ASC.Web.Api --self-contained -r win10-x64 -o build/deploy/www/api  /fl1 /flp1:LogFile=build/ASC.Web.Api.log;Verbosity=Normal

xcopy config\*.* build\deploy\config\ /E /R /Y

pause