echo "RUN ASC.Backup"
call dotnet run --project ..\..\common\services\ASC.Data.Backup\ASC.Data.Backup.csproj --no-build --$STORAGE_ROOT=..\..\..\Data --log__dir=..\..\..\Logs --log__name=backup