echo "RUN ASC.Notify"
call dotnet run --project ..\..\common\services\ASC.Notify\ASC.Notify.csproj --no-build --$STORAGE_ROOT=..\..\..\Data --log__dir=..\..\..\Logs --log__name=notify