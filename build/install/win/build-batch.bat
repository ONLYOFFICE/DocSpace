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
copy "build\install\win\nginx.conf" "build\install\win\Files\nginx\conf\nginx.conf" /y
rmdir build\install\win\publish /s /q

REM echo ######## Build Utils ########
%nuget% install %cd%\build\install\win\CustomActions\C#\Utils\packages.config -OutputDirectory %cd%\build\install\win\CustomActions\C#\Utils\packages
%msbuild% build\install\win\CustomActions\C#\Utils\Utils.csproj
copy build\install\win\CustomActions\C#\Utils\bin\Debug\Utils.CA.dll build\install\win\Utils.CA.dll /y
rmdir build\install\win\CustomActions\C#\Utils\bin /s /q
rmdir build\install\win\CustomActions\C#\Utils\obj /s /q

REM echo ######## Edit nginx conf files ########
%sed% -i "s!#rewrite!rewrite!g" build/install/win/Files/nginx/conf/onlyoffice.conf
%sed% -i "s!/etc/nginx/includes!includes!g" build/install/win/Files/nginx/conf/onlyoffice.conf
%sed% -i "s!/var/www!..!g" build/install/win/Files/nginx/conf/onlyoffice-*.conf
%sed% -i "s!/var/www!..!g" build/install/win/Files/nginx/conf/includes/onlyoffice-*.conf

REM echo ######## Edit json files ########
%sed% -i "s!\(\"machinekey\":\).\".*\"!\1 \"1123askdasjklasbnd\"!g" build/install/win/Files/config/appsettings*.json
%sed% -i "s!\(\"folder\":\).\".*\"!\1 \"{APPDIRCONF}products\"!g" build/install/win/Files/config/appsettings*.json
%sed% -i "s!\(\"path\":\).\".*\"!\1 \"{APPDIRCONF}services\/ASC.Socket.IO\/service\"!g" build/install/win/Files/config/socket*.json
%sed% -i "s!\(\"path\":\).\".*\"!\1 \"{APPDIRCONF}services\/ASC.Thumbnails\/service\"!g" build/install/win/Files/config/thumb*.json
%sed% -i "s!\(\"path\":\).\".*\"!\1 \"{APPDIRCONF}services\/ASC.UrlShortener\/service\/index.js\"!g" build/install/win/Files/config/urlshortener*.json
%sed% -i "s!\(\"path\":\).\".*\"!\1 \"{APPDIRCONF}services\/ASC.SsoAuth\/service\"!g" build/install/win/Files/config/ssoauth*.json
%sed% -i "s!\(\"path\":\).\".*\"!\1 \"{APPDIRCONF}services\/ASC.UrlShortener\/service\/index.js\"!g" build/install/win/Files/config/appsettings.services.json
%sed% -i "s!\(\"log\":\).\".*\"!\1 \"{APPDIRCONF}Logs\/urlshortener.log\"!g" build/install/win/Files/config/appsettings.services.json
%sed% -i "s!\(\"appsettings\":\).\".*\"!\1 \"{APPDIRCONF}config\"!g" build/install/win/Files/services/ASC.UrlShortener/service/config/config.json
%sed% -i "s!\(\"appsettings\":\).\".*\"!\1 \"{APPDIRCONF}config\"!g" build/install/win/Files/services/ASC.Socket.IO/service/config/config.json
%sed% -i "s!\(\"appsettings\":\).\".*\"!\1 \"{APPDIRCONF}config\"!g" build/install/win/Files/services/ASC.SsoAuth/service/config/config.json

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
