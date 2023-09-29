REM echo ######## Set variables ########
set "publisher="Ascensio System SIA""
set "nuget="%cd%\thirdparty\SimpleRestServices\src\.nuget\NuGet.exe""
set "environment=production"

REM echo ######## Extracting and preparing files to build ########
md build\install\win\OpenResty\tools
md build\install\win\Files\tools
md build\install\win\Files\Logs
md build\install\win\Files\Data
md build\install\win\Files\sbin
md build\install\win\Files\products\ASC.Files\server\temp
md build\install\win\Files\products\ASC.People\server\temp
md build\install\win\Files\services\ASC.Data.Backup\service\temp
md build\install\win\Files\services\ASC.Files.Service\service\temp
md build\install\win\Files\services\ASC.Notify\service\temp
md build\install\win\Files\services\ASC.Studio.Notify\service\temp
md build\install\win\Files\services\ASC.Data.Backup.BackgroundTasks\service\temp
md build\install\win\Files\services\ASC.ClearEvents\service\temp
md build\install\win\Files\services\ASC.Web.Api\service\temp
md build\install\win\Files\services\ASC.Web.Studio\service\temp
md build\install\win\Files\services\ASC.Web.HealthChecks.UI\service\temp
copy build\install\win\WinSW.NET4.exe "build\install\win\OpenResty\tools\OpenResty.exe" /y
copy build\install\win\tools\OpenResty.xml "build\install\win\OpenResty\tools\OpenResty.xml" /y
copy build\install\win\WinSW3.0.0.exe "build\install\win\Files\tools\Socket.IO.exe" /y
copy build\install\win\tools\Socket.IO.xml "build\install\win\Files\tools\Socket.IO.xml" /y
copy build\install\win\WinSW3.0.0.exe "build\install\win\Files\tools\SsoAuth.exe" /y
copy build\install\win\tools\SsoAuth.xml "build\install\win\Files\tools\SsoAuth.xml" /y
copy build\install\win\WinSW3.0.0.exe "build\install\win\Files\tools\DocEditor.exe" /y
copy build\install\win\tools\DocEditor.xml "build\install\win\Files\tools\DocEditor.xml" /y
copy build\install\win\WinSW3.0.0.exe "build\install\win\Files\tools\Login.exe" /y
copy build\install\win\tools\Login.xml "build\install\win\Files\tools\Login.xml" /y
copy "build\install\win\nginx.conf" "build\install\win\Files\nginx\conf\nginx.conf" /y
copy "build\install\docker\config\nginx\onlyoffice-proxy.conf" "build\install\win\Files\nginx\conf\onlyoffice-proxy.conf" /y
copy "build\install\docker\config\nginx\onlyoffice-proxy-ssl.conf" "build\install\win\Files\nginx\conf\onlyoffice-proxy-ssl.conf.tmpl" /y
copy "build\install\docker\config\nginx\letsencrypt.conf" "build\install\win\Files\nginx\conf\includes\letsencrypt.conf" /y
copy "build\install\win\sbin\docspace-ssl-setup.ps1" "build\install\win\Files\sbin\docspace-ssl-setup.ps1" /y
rmdir build\install\win\publish /s /q

REM echo ######## SSL configs ########
%sed% -i "s/the_host/host/g" build\install\win\Files\nginx\conf\onlyoffice-proxy.conf build\install\win\Files\nginx\conf\onlyoffice-proxy-ssl.conf.tmpl
%sed% -i "s/the_scheme/scheme/g" build\install\win\Files\nginx\conf\onlyoffice-proxy.conf build\install\win\Files\nginx\conf\onlyoffice-proxy-ssl.conf.tmpl
%sed% -i "s/ssl_dhparam \/etc\/ssl\/certs\/dhparam.pem;/#ssl_dhparam \/etc\/ssl\/certs\/dhparam.pem;/" build\install\win\Files\nginx\conf\onlyoffice-proxy-ssl.conf.tmpl
%sed% -i "s_\(.*root\).*;_\1 \"{APPDIR}letsencrypt\";_g" -i build\install\win\Files\nginx\conf\includes\letsencrypt.conf

REM echo ######## Delete test and dev configs ########
del /f /q build\install\win\Files\config\*.test.json
del /f /q build\install\win\Files\config\*.dev.json

::default logging to warning
%sed% "s_\(\"Default\":\).*,_\1 \"Warning\",_g" -i build\install\win\Files\config\appsettings.json
%sed% "s_\(\"logLevel\":\).*_\1 \"warning\"_g" -i build\install\win\Files\config\appsettings.services.json
%sed% "/\"debug-info\": {/,/}/ s/\(\"enabled\": \)\".*\"/\1\"false\"/" -i build\install\win\Files\config\appsettings.json

%sed% "s_\(\"samesite\":\).*,_\1 \"None\",_g" -i build\install\win\Files\config\appsettings.json

::redirectUrl value replacement
%sed% "s/teamlab.info/onlyoffice.com/g" -i build\install\win\Files\config/autofac.consumers.json
%sed% "s_\(\"wrongPortalNameUrl\":\).*,_\1 \"\",_g" -i build\install\win\Files\public\scripts\config.json

REM echo ######## Remove AWSTarget from nlog.config ########
%sed% -i "/<target type=\"AWSTarget\" name=\"aws\"/,/<\/target>/d; /<target type=\"AWSTarget\" name=\"aws_sql\"/,/<\/target>/d" build\install\win\Files\config\nlog.config
del /q build\install\win\Files\config\sed*

::edit environment
%sed% -i "s/\(\W\)PRODUCT.ENVIRONMENT.SUB\(\W\)/\1%environment%\2/g" build\install\win\DocSpace.aip

::delete nginx configs
del /f /q build\install\win\Files\nginx\conf\onlyoffice-login.conf
del /f /q build\install\win\Files\nginx\conf\onlyoffice-story.conf


REM echo ######## Build Utils ########
%nuget% install %cd%\build\install\win\CustomActions\C#\Utils\packages.config -OutputDirectory %cd%\build\install\win\CustomActions\C#\Utils\packages
%msbuild% build\install\win\CustomActions\C#\Utils\Utils.csproj
copy build\install\win\CustomActions\C#\Utils\bin\Debug\Utils.CA.dll build\install\win\Utils.CA.dll /y
rmdir build\install\win\CustomActions\C#\Utils\bin /s /q
rmdir build\install\win\CustomActions\C#\Utils\obj /s /q

REM echo ######## Delete temp files ########
del /f /q build\install\win\*.back.*

REM echo ######## Build MySQL Server Installer ########
iscc /Qp /S"byparam="signtool" sign /a /n "%publisher%" /t http://timestamp.digicert.com $f" "build\install\win\MySQL Server Installer Runner.iss"

REM echo ######## Build OpenResty ########
IF "%SignBuild%"=="true" (
%AdvancedInstaller% /edit build\install\win\OpenResty.aip /SetSig
%AdvancedInstaller% /edit build\install\win\OpenResty.aip /SetDigitalCertificateFile -file %onlyoffice_codesign_path% -password "%onlyoffice_codesign_password%"
)
%AdvancedInstaller% /rebuild build\install\win\OpenResty.aip

REM echo ######## Build DocSpace package ########
%AdvancedInstaller% /edit build\install\win\DocSpace.aip /SetVersion %BUILD_VERSION%.%BUILD_NUMBER%

IF "%SignBuild%"=="true" (
%AdvancedInstaller% /edit build\install\win\DocSpace.aip /SetSig
%AdvancedInstaller% /edit build\install\win\DocSpace.aip /SetDigitalCertificateFile -file %onlyoffice_codesign_path% -password "%onlyoffice_codesign_password%"
)

%AdvancedInstaller% /rebuild build\install\win\DocSpace.aip

REM echo ######## Build DocSpace Enterprise package ########
%AdvancedInstaller% /edit build\install\win\DocSpace.Enterprise.aip /SetVersion %BUILD_VERSION%.%BUILD_NUMBER%

IF "%SignBuild%"=="true" (
%AdvancedInstaller% /edit build\install\win\DocSpace.Enterprise.aip /SetSig
%AdvancedInstaller% /edit build\install\win\DocSpace.Enterprise.aip /SetDigitalCertificateFile -file %onlyoffice_codesign_path% -password "%onlyoffice_codesign_password%"
)

%AdvancedInstaller% /rebuild build\install\win\DocSpace.Enterprise.aip