echo "RUN ASC.CRM.Server"
call dotnet run --project ..\..\products\ASC.CRM\Server\ASC.CRM.csproj --no-build --$STORAGE_ROOT=..\..\..\Data --log__dir=..\..\..\Logs --log__name=crm