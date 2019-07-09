PUSHD %~dp0
echo "ASC.Web.Components"
cd ../web/ASC.Web.Components
call npm ci

echo "ASC.Web.Storybook"
cd ../ASC.Web.Storybook
call npm ci

echo "ASC.Web.Client"
cd ../ASC.Web.Client
call npm ci

echo "ASC.Web.sln"
cd ../../
call dotnet build ASC.Web.sln  /fl1 /flp1:LogFile=build/ASC.Web.log;Verbosity=Normal

echo "ASC.People"
cd products/ASC.People/ClientApp
call npm ci
call npm run build

pause