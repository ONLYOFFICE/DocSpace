echo "RUN ASC.ClearEvents"
call dotnet run --project ..\..\common\services\ASC.ClearEvents\ASC.ClearEvents.csproj --no-build --$STORAGE_ROOT=..\..\Data  --log__dir=..\..\Logs --log__name=clearEvents