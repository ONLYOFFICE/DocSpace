echo "RUN ASC.Projects"
call dotnet run --project ..\..\products\ASC.Projects\Server\ASC.Projects.csproj --no-build --$STORAGE_ROOT=..\..\..\Data --log__dir=..\..\..\Logs --log__name=projects