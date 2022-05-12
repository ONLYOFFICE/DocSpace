@echo off
echo 
echo #####################
echo #  publish backend  #
echo #####################

set FirstArg=%~s1

set SecondArg=%~s2

if defined SecondArg (
	set PathToRepository=%FirstArg%
	set PathToAppFolder=%SecondArg%
) else (
	set PathToRepository=%FirstArg%
	set PathToAppFolder=%FirstArg%\publish
)

rem publish in directory 'products'
REM dotnet publish "%PathToRepository%\products\ASC.Calendar\server\ASC.Calendar.csproj" -c Release --self-contained false -o "%PathToAppFolder%\products\ASC.Calendar\server"
REM dotnet publish "%PathToRepository%\products\ASC.CRM\server\ASC.CRM.csproj" -c Release --self-contained false -o "%PathToAppFolder%\products\ASC.CRM\server"
dotnet publish "%PathToRepository%\products\ASC.Files\server\ASC.Files.csproj" -c Release --self-contained false -o "%PathToAppFolder%\products\ASC.Files\server"
REM dotnet publish "%PathToRepository%\products\ASC.Mail\server\ASC.Mail.csproj" -c Release --self-contained false -o "%PathToAppFolder%\products\ASC.Mail\server"
dotnet publish "%PathToRepository%\products\ASC.People\server\ASC.People.csproj" -c Release --self-contained false -o "%PathToAppFolder%\products\ASC.People\server"
REM dotnet publish "%PathToRepository%\products\ASC.Projects\server\ASC.Projects.csproj" -c Release --self-contained false -o "%PathToAppFolder%\products\ASC.Projects\server"

rem publish in directory 'services'
dotnet publish "%PathToRepository%\common\services\ASC.ApiSystem\ASC.ApiSystem.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.ApiSystem\service"
dotnet publish "%PathToRepository%\common\services\ASC.Data.Backup\ASC.Data.Backup.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.Data.Backup\service"
dotnet publish "%PathToRepository%\common\services\ASC.Data.Storage.Encryption\ASC.Data.Storage.Encryption.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.Data.Storage.Encryption\service"
dotnet publish "%PathToRepository%\products\ASC.Files\service\ASC.Files.Service.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.Files.Service\service"
dotnet publish "%PathToRepository%\common\services\ASC.Data.Storage.Migration\ASC.Data.Storage.Migration.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.Data.Storage.Migration\service"
dotnet publish "%PathToRepository%\common\services\ASC.Notify\ASC.Notify.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.Notify\service"
dotnet publish "%PathToRepository%\common\services\ASC.Socket.IO.Svc\ASC.Socket.IO.Svc.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.Socket.IO.Svc\service"
dotnet publish "%PathToRepository%\common\services\ASC.Studio.Notify\ASC.Studio.Notify.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.Studio.Notify\service"
dotnet publish "%PathToRepository%\common\services\ASC.TelegramService\ASC.TelegramService.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.TelegramService\service"
dotnet publish "%PathToRepository%\common\services\ASC.Thumbnails.Svc\ASC.Thumbnails.Svc.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.Thumbnails.Svc\service"
dotnet publish "%PathToRepository%\common\services\ASC.UrlShortener.Svc\ASC.UrlShortener.Svc.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.UrlShortener.Svc\service"
dotnet publish "%PathToRepository%\web\ASC.Web.Api\ASC.Web.Api.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.Web.Api\service"
dotnet publish "%PathToRepository%\web\ASC.Web.Studio\ASC.Web.Studio.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.Web.Studio\service"
dotnet publish "%PathToRepository%\common\services\ASC.SsoAuth.Svc\ASC.SsoAuth.Svc.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.SsoAuth.Svc\service"

rem Publish backend services (Nodejs)
mkdir "%PathToAppFolder%\services\ASC.Thumbnails\service"
xcopy "%PathToRepository%\common\ASC.Thumbnails" "%PathToAppFolder%\services\ASC.Thumbnails\service" /s /y /b /i

mkdir "%PathToAppFolder%\services\ASC.UrlShortener\service"
xcopy "%PathToRepository%\common\ASC.UrlShortener" "%PathToAppFolder%\services\ASC.UrlShortener\service" /s /y /b /i

mkdir "%PathToAppFolder%\services\ASC.Socket.IO\service"
xcopy "%PathToRepository%\common\ASC.Socket.IO" "%PathToAppFolder%\services\ASC.Socket.IO\service" /s /y /b /i

mkdir "%PathToAppFolder%\services\ASC.SsoAuth\service"
xcopy "%PathToRepository%\common\ASC.SsoAuth" "%PathToAppFolder%\services\ASC.SsoAuth\service" /s /y /b /i
