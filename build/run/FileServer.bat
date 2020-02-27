echo "RUN ASC.Files"
call dotnet run --project ..\..\products\ASC.Files\Server\ASC.Files.csproj --no-build --$STORAGE_ROOT=..\..\..\Data --log__dir=..\..\..\Logs --log__name=files