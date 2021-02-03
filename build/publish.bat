echo "Delete publish folder"
rmdir /s /q publish

echo "Publish ASC.Notify.csproj project"
dotnet publish ..\common\services\ASC.Notify\ASC.Notify.csproj -c Release -o publish/ASC.Notify/
if not %errorlevel% == 0 goto end

echo "Publish ASC.People.csproj project"
dotnet publish ..\products\ASC.People\Server\ASC.People.csproj -c Release -o publish/ASC.People/
if not %errorlevel% == 0 goto end

echo "Publish ASC.Studio.Notify.csproj project"
dotnet publish ..\common\services\ASC.Studio.Notify\ASC.Studio.Notify.csproj -c Release -o publish/ASC.Studio.Notify/
if not %errorlevel% == 0 goto end

echo "Publish ASC.Web.Api.csproj project"
dotnet publish ..\web\ASC.Web.Api\ASC.Web.Api.csproj -c Release -o publish/ASC.Web.Studio
if not %errorlevel% == 0 goto end

echo "Publish ASC.Web.Studio.csproj project"
dotnet publish ..\web\ASC.Web.Studio\ASC.Web.Studio.csproj -c Release -o publish/ASC.Web.Studio
if not %errorlevel% == 0 goto end

echo "Publish ASC.People project"
xcopy ..\products\ASC.People\Client\*.* publish\ASC.People.Client\ /E /R /Y

echo "Publish ASC.Web.Client project"
xcopy ..\web\ASC.Web.Client\*.* publish\ASC.Web.Client\ /E /R /Y

echo "Publish ASC.UrlShortener.Svc.csproj project"
dotnet publish ..\common\services\ASC.UrlShortener.Svc\ASC.UrlShortener.Svc.csproj -c Release -o publish/ASC.UrlShortener.Svc/
xcopy ..\common\ASC.UrlShortener\*.* publish\ASC.UrlShortener\ /E /R /Y
if not %errorlevel% == 0 goto end

:end
pause