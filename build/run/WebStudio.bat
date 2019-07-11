echo "RUN ASC.Web.Studio"
PUSHD %~dp0..\..\web\ASC.Web.Studio\
call dotnet watch run --project ASC.Web.Studio.csproj