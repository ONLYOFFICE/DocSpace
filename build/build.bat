PUSHD %~dp0
echo "ASC.Web.Components"
cd ../web/ASC.Web.Components
call npm install

echo "ASC.Web.sln"
cd ../../
call dotnet build ASC.Web.sln

echo "ASC.People"
call dotnet publish products/ASC.People --self-contained -r win10-x64 -o build/deploy

echo "ASC.Web.Api"
call dotnet publish web/ASC.Web.Api --self-contained -r win10-x64 -o build/deploy