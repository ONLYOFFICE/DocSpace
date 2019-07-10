PUSHD %~dp0..
echo "ASC.Web.Components"
call npm ci --prefix web/ASC.Web.Components

echo "ASC.Web.Storybook"
call npm ci --prefix web/ASC.Web.Storybook

echo "ASC.Web.Client"
call npm ci --prefix web/ASC.Web.Client

echo "ASC.Web.People.Client"
call npm ci --prefix products/ASC.People/Client
call npm run build --prefix products/ASC.People/Client

echo "ASC.Web.sln"
call dotnet build ASC.Web.sln  /fl1 /flp1:LogFile=build/ASC.Web.log;Verbosity=Normal

pause