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

rem backend services (dotnet) in directory 'products'
dotnet publish "%PathToRepository%\products\ASC.Files\server\ASC.Files.csproj" -c Release --self-contained false -o "%PathToAppFolder%\products\ASC.Files\server"
dotnet publish "%PathToRepository%\products\ASC.People\server\ASC.People.csproj" -c Release --self-contained false -o "%PathToAppFolder%\products\ASC.People\server"

rem backend services (dotnet) in directory 'services'
dotnet publish "%PathToRepository%\common\services\ASC.ApiSystem\ASC.ApiSystem.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.ApiSystem\service"
dotnet publish "%PathToRepository%\common\services\ASC.Data.Backup\ASC.Data.Backup.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.Data.Backup\service"
dotnet publish "%PathToRepository%\products\ASC.Files\service\ASC.Files.Service.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.Files.Service\service"
dotnet publish "%PathToRepository%\common\services\ASC.Notify\ASC.Notify.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.Notify\service"
dotnet publish "%PathToRepository%\common\services\ASC.Studio.Notify\ASC.Studio.Notify.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.Studio.Notify\service"
dotnet publish "%PathToRepository%\common\services\ASC.TelegramService\ASC.TelegramService.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.TelegramService\service"
dotnet publish "%PathToRepository%\common\services\ASC.Data.Backup.BackgroundTasks\ASC.Data.Backup.BackgroundTasks.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.Data.Backup.BackgroundTasks\service"
dotnet publish "%PathToRepository%\common\services\ASC.ClearEvents\ASC.ClearEvents.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.ClearEvents\service"
dotnet publish "%PathToRepository%\common\ASC.Migration\ASC.Migration.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.Migration\service"
dotnet publish "%PathToRepository%\common\services\ASC.Webhooks.Service\ASC.Webhooks.Service.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.Webhooks.Service\service"
dotnet publish "%PathToRepository%\web\ASC.Web.Api\ASC.Web.Api.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.Web.Api\service"
dotnet publish "%PathToRepository%\web\ASC.Web.Studio\ASC.Web.Studio.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.Web.Studio\service"

rem backend services (Nodejs) in directory 'services'
mkdir "%PathToAppFolder%\services\ASC.UrlShortener\service"
xcopy "%PathToRepository%\common\ASC.UrlShortener" "%PathToAppFolder%\services\ASC.UrlShortener\service" /s /y /b /i

mkdir "%PathToAppFolder%\services\ASC.Socket.IO\service"
xcopy "%PathToRepository%\common\ASC.Socket.IO" "%PathToAppFolder%\services\ASC.Socket.IO\service" /s /y /b /i

mkdir "%PathToAppFolder%\services\ASC.SsoAuth\service"
xcopy "%PathToRepository%\common\ASC.SsoAuth" "%PathToAppFolder%\services\ASC.SsoAuth\service" /s /y /b /i

rem backend services (Nodejs) in directory 'products'
mkdir "%PathToAppFolder%\products\ASC.Login\login"
xcopy "%PathToRepository%\build\deploy\login" "%PathToAppFolder%\products\ASC.Login\login" /s /y /b /i

mkdir "%PathToAppFolder%\products\ASC.Files\editor"
xcopy "%PathToRepository%\build\deploy\editor" "%PathToAppFolder%\products\ASC.Files\editor" /s /y /b /i
