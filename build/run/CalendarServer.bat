echo "RUN ASC.Calendar"
call dotnet run --project ..\..\products\ASC.Calendar\Server\ASC.Calendar.csproj --no-build --$STORAGE_ROOT=..\..\..\Data --log__dir=..\..\..\Logs --log__name=calendar