echo "RUN ASC.Web.Api"
call dotnet run --project ..\..\web\ASC.Web.Api\ASC.Web.Api.csproj --no-build --$STORAGE_ROOT=..\..\Data --log__dir=..\..\Logs --log__name=api