echo "RUN ASC.Web.Api"
PUSHD %~dp0..\..\web\ASC.Web.Api\
call dotnet watch run --project ASC.Web.Api.csproj