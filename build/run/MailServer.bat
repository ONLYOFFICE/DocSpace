echo "RUN ASC.Mail"
call dotnet run --project ..\..\products\ASC.Mail\Server\ASC.Mail.csproj --no-build --$STORAGE_ROOT=..\..\..\Data --log__dir=..\..\..\Logs --log__name=mail