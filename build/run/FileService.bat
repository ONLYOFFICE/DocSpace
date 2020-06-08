echo "RUN ASC.Files"
call dotnet run --project ..\..\products\ASC.Files\Service\ASC.Files.Service.csproj --no-build --$STORAGE_ROOT=..\..\..\Data --log__dir=..\..\..\Logs --log__name=files