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

dotnet publish "%PathToRepository%\products\ASC.Calendar\server\ASC.Calendar.csproj" -c Release --self-contained false -o "%PathToAppFolder%\products\ASC.Calendar\server"
dotnet publish "%PathToRepository%\products\ASC.CRM\server\ASC.CRM.csproj" -c Release --self-contained false -o "%PathToAppFolder%\products\ASC.CRM\server"
dotnet publish "%PathToRepository%\products\ASC.Files\Core\ASC.Files.Core.csproj" -c Release --self-contained false -o "%PathToAppFolder%\products\ASC.Files.Core\server"
dotnet publish "%PathToRepository%\products\ASC.Files\server\ASC.Files.csproj" -c Release --self-contained false -o "%PathToAppFolder%\products\ASC.Files\server"
dotnet publish "%PathToRepository%\products\ASC.Mail\server\ASC.Mail.csproj" -c Release --self-contained false -o "%PathToAppFolder%\products\ASC.Mail\server"
dotnet publish "%PathToRepository%\products\ASC.People\server\ASC.People.csproj" -c Release --self-contained false -o "%PathToAppFolder%\products\ASC.People\server"
dotnet publish "%PathToRepository%\products\ASC.Projects\server\ASC.Projects.csproj" -c Release --self-contained false -o "%PathToAppFolder%\products\ASC.Projects\server"
dotnet publish "%PathToRepository%\products\ASC.Files\service\ASC.Files.service.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.Files.service\service"
dotnet publish "%PathToRepository%\products\ASC.Files\Tests\ASC.Files.Tests.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.Files.Tests\service"
dotnet publish "%PathToRepository%\common\services\ASC.ApiSystem\ASC.ApiSystem.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.ApiSystem\service"
dotnet publish "%PathToRepository%\common\services\ASC.AuditTrail\ASC.AuditTrail.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.AuditTrail\service"
dotnet publish "%PathToRepository%\common\services\ASC.Data.Backup\ASC.Data.Backup.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.Data.Backup\service"
dotnet publish "%PathToRepository%\common\services\ASC.Data.Storage.Encryption\ASC.Data.Storage.Encryption.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.Data.Storage.Encryption\service"
dotnet publish "%PathToRepository%\common\services\ASC.Data.Storage.Migration\ASC.Data.Storage.Migration.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.Data.Storage.Migration\service"
dotnet publish "%PathToRepository%\common\services\ASC.ElasticSearch\ASC.ElasticSearch.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.ElasticSearch\service"
dotnet publish "%PathToRepository%\common\services\ASC.Feed.Aggregator\ASC.Feed.Aggregator.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.Feed.Aggregator\service"
dotnet publish "%PathToRepository%\common\services\ASC.Notify\ASC.Notify.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.Notify\service"
dotnet publish "%PathToRepository%\common\services\ASC.Socket.IO.Svc\ASC.Socket.IO.Svc.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.Socket.IO.Svc\service"
dotnet publish "%PathToRepository%\common\services\ASC.Studio.Notify\ASC.Studio.Notify.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.Studio.Notify\service"
dotnet publish "%PathToRepository%\common\services\ASC.Telegramservice\ASC.Telegramservice.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.Telegramservice\service"
dotnet publish "%PathToRepository%\common\services\ASC.Thumbnails.Svc\ASC.Thumbnails.Svc.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.Thumbnails.Svc\service"
dotnet publish "%PathToRepository%\common\services\ASC.UrlShortener.Svc\ASC.UrlShortener.Svc.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.UrlShortener.Svc\service"
dotnet publish "%PathToRepository%\web\ASC.Web.Api\ASC.Web.Api.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.Web.Api\service"
dotnet publish "%PathToRepository%\web\ASC.Web.Studio\ASC.Web.Studio.csproj" -c Release --self-contained false -o "%PathToAppFolder%\services\ASC.Web.Studio\service"
mkdir "%PathToAppFolder%\services\ASC.Thumbnails\service"
xcopy "%PathToRepository%\common\ASC.Thumbnails" "%PathToAppFolder%\services\ASC.Thumbnails\service" /S /Y /B /I
mkdir "%PathToAppFolder%\services\ASC.UrlShortener\service"
xcopy "%PathToRepository%\common\ASC.UrlShortener" "%PathToAppFolder%\services\ASC.UrlShortener\service" /S /Y /B /I
mkdir "%PathToAppFolder%\services\ASC.Socket.IO\service"
xcopy "%PathToRepository%\common\ASC.Socket.IO" "%PathToAppFolder%\services\ASC.Socket.IO\service" /S /Y /B /I