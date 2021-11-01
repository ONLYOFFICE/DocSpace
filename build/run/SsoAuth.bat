echo "RUN ASC.SsoAuth.Svc"
call dotnet run --project ..\..\common\services\ASC.SsoAuth.Svc\ASC.SsoAuth.Svc.csproj --no-build --$STORAGE_ROOT=..\..\Data  --log__dir=..\..\Logs --log__name=ssoauth