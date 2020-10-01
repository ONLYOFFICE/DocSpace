echo "RUN ASC.Socket.IO.Svc"
call dotnet run --project ..\..\common\services\ASC.Socket.IO.Svc\ASC.Socket.IO.Svc.csproj --no-build --$STORAGE_ROOT=..\..\Data  --log__dir=..\..\Logs --log__name=socket