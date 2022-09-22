REM echo ######## Set variables ########
set "publisher="Ascensio System SIA""
set "nginx_version=1.21.1"
set "nuget="%cd%\thirdparty\SimpleRestServices\src\.nuget\NuGet.exe""

REM echo ######## Extracting and preparing files to build ########
%sevenzip% x build\install\win\nginx-%nginx_version%.zip -o"build\install\win\Files" -y
xcopy "build\install\win\Files\nginx-%nginx_version%" "build\install\win\Files\nginx" /s /y /b /i
rmdir build\install\win\Files\nginx-%nginx_version% /s /q
md build\install\win\Files\nginx\temp
md build\install\win\Files\nginx\logs
md build\install\win\Files\tools
md build\install\win\Files\Logs
md build\install\win\Files\service\
md build\install\win\Files\products\ASC.Files\server\temp
md build\install\win\Files\products\ASC.People\server\temp
md build\install\win\Files\services\ASC.Data.Backup\service\temp
md build\install\win\Files\services\ASC.Files.Service\service\temp
md build\install\win\Files\services\ASC.Notify\service\temp
md build\install\win\Files\services\ASC.Studio.Notify\service\temp
md build\install\win\Files\services\ASC.TelegramService\service\temp
md build\install\win\Files\services\ASC.Data.Backup.BackgroundTasks\service\temp
md build\install\win\Files\services\ASC.ClearEvents\service\temp
md build\install\win\Files\services\ASC.Migration\service\temp
md build\install\win\Files\services\ASC.Webhooks.Service\service\temp
md build\install\win\Files\services\ASC.Web.Api\service\temp
md build\install\win\Files\services\ASC.Web.Studio\service\temp
copy build\install\win\WinSW.NET4.exe "build\install\win\Files\tools\Proxy.exe" /y
copy build\install\win\tools\Proxy.xml "build\install\win\Files\tools\Proxy.xml" /y
copy build\install\win\WinSW3.0.0.exe "build\install\win\Files\tools\Socket.IO.exe" /y
copy build\install\win\tools\Socket.IO.xml "build\install\win\Files\tools\Socket.IO.xml" /y
copy build\install\win\WinSW3.0.0.exe "build\install\win\Files\tools\UrlShortener.exe" /y
copy build\install\win\tools\UrlShortener.xml "build\install\win\Files\tools\UrlShortener.xml" /y
copy build\install\win\WinSW3.0.0.exe "build\install\win\Files\tools\SsoAuth.exe" /y
copy build\install\win\tools\SsoAuth.xml "build\install\win\Files\tools\SsoAuth.xml" /y
copy build\install\win\WinSW3.0.0.exe "build\install\win\Files\tools\DocEditor.exe" /y
copy build\install\win\tools\DocEditor.xml "build\install\win\Files\tools\DocEditor.xml" /y
copy build\install\win\WinSW3.0.0.exe "build\install\win\Files\tools\Login.exe" /y
copy build\install\win\tools\Login.xml "build\install\win\Files\tools\Login.xml" /y
copy "build\install\win\nginx.conf" "build\install\win\Files\nginx\conf\nginx.conf" /y
rmdir build\install\win\publish /s /q
del /f /q build\install\win\Files\nginx\conf\onlyoffice-login.conf


REM echo ######## Build Utils ########
%nuget% install %cd%\build\install\win\CustomActions\C#\Utils\packages.config -OutputDirectory %cd%\build\install\win\CustomActions\C#\Utils\packages
%msbuild% build\install\win\CustomActions\C#\Utils\Utils.csproj
copy build\install\win\CustomActions\C#\Utils\bin\Debug\Utils.CA.dll build\install\win\Utils.CA.dll /y
rmdir build\install\win\CustomActions\C#\Utils\bin /s /q
rmdir build\install\win\CustomActions\C#\Utils\obj /s /q

REM echo ######## Delete temp files ########
del /f /q build\install\win\Files\config\sed*
del /f /q build\install\win\Files\nginx\conf\sed*
del /f /q build\install\win\Files\nginx\conf\includes\sed*
del /f /q build\install\win\Files\services\*\service\config\sed*
del /f /q build\install\win\*.back.*

REM echo ######## Build MySQL Server Installer ########
iscc /Qp /S"byparam="signtool" sign /a /n "%publisher%" /t http://timestamp.digicert.com $f" "build\install\win\MySQL Server Installer Runner.iss"

REM echo ######## Build DocSpace package ########
%AdvancedInstaller% /edit build\install\win\DocSpace.aip /SetVersion %BUILD_VERSION%.%BUILD_NUMBER%
%AdvancedInstaller% /rebuild build\install\win\DocSpace.aip
