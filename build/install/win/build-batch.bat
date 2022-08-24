REM echo ######## Set variables ########
set "publisher="Ascensio System SIA""
set "zookeeper_version=3.7.1"
set "kafka_version=2.8.0"
set "nginx_version=1.21.1"
set "scala_version=2.12"

REM echo ######## Extracting and preparing files to build ########
%sevenzip% x build\install\win\nginx-%nginx_version%.zip -o"build\install\win\Files" -y
xcopy "build\install\win\Files\nginx-%nginx_version%" "build\install\win\Files\nginx" /s /y /b /i
rmdir build\install\win\Files\nginx-%nginx_version% /s /q
rmdir build\install\win\kafka-zookeeper /s /q
md build\install\win\kafka-zookeeper
md build\install\win\Files\nginx\temp
md build\install\win\Files\nginx\logs
%tar% -xvf build\install\win\apache-zookeeper-%zookeeper_version%-bin.tar.gz -C build\install\win\kafka-zookeeper
%tar% -xvf build\install\win\kafka_%scala_version%-%kafka_version%.tgz -C build\install\win\kafka-zookeeper
ren build\install\win\kafka-zookeeper\apache-zookeeper-%zookeeper_version%-bin zookeeper
ren build\install\win\kafka-zookeeper\kafka_%scala_version%-%kafka_version% kafka
md build\install\win\kafka-zookeeper\kafka\tools
md build\install\win\Files\tools
copy build\install\win\WinSW.NET4new.exe "build\install\win\kafka-zookeeper\kafka\tools\kafka.exe" /y
copy build\install\win\WinSW.NET4new.exe "build\install\win\kafka-zookeeper\kafka\tools\zookeeper.exe" /y
copy build\install\win\tools\zookeeper.xml "build\install\win\kafka-zookeeper\kafka\tools\zookeeper.xml" /y
copy build\install\win\tools\kafka.xml "build\install\win\kafka-zookeeper\kafka\tools\kafka.xml" /y
del /f /q build\install\win\apache-zookeeper-%zookeeper_version%-bin.*
del /f /q build\install\win\kafka_%scala_version%-%kafka_version%.*
copy build\install\win\WinSW.NET4.exe "build\install\win\Files\tools\proxy.exe" /y
copy build\install\win\tools\proxy.xml "build\install\win\Files\tools\proxy.xml" /y
copy "build\install\win\nginx.conf" "build\install\win\Files\nginx\conf\nginx.conf" /y
copy "build\install\win\kafka-zookeeper\zookeeper\conf\zoo_sample.cfg" "build\install\win\kafka-zookeeper\zookeeper\conf\zoo.cfg" /y
del /f /q "build\install\win\kafka-zookeeper\zookeeper\conf\zoo_sample.cfg"
rmdir build\install\win\publish /s /q

REM echo ######## Build Utils ########
%msbuild% build\install\win\CustomActions\C#\Utils\Utils.csproj
copy build\install\win\CustomActions\C#\Utils\bin\Debug\Utils.CA.dll build\install\win\Utils.CA.dll /y
rmdir build\install\win\CustomActions\C#\Utils\bin /s /q
rmdir build\install\win\CustomActions\C#\Utils\obj /s /q

REM echo ######## Edit zookeeper/kafka cfg and proprties files ########
%sed% -i "s/\(dataDir\).*/\1=.\/..\/zookeeper\/Data/g" build/install/win/kafka-zookeeper/zookeeper/conf/zoo.cfg
%sed% -i "s/\(log.dirs\)=.*/\1=kafka-logs/g" build/install/win/kafka-zookeeper/kafka/config/server.properties
%sed% -i "s/\(zookeeper.connect\)=.*/\1=localhost:2181/g" build/install/win/kafka-zookeeper/kafka/config/server.properties
%sed% -i "s/\(clientPort\)=.*/\1=2181/g" build/install/win/kafka-zookeeper/kafka/config/zookeeper.properties
%sed% -i "s/\(dataDir\).*/\1=.\/..\/zookeeper\/Data/g" build/install/win/kafka-zookeeper/kafka/config/zookeeper.properties
%sed% -i "s/\(bootstrap.servers\)=.*/\1=localhost:9092/g" build/install/win/kafka-zookeeper/kafka/config/consumer.properties
%sed% -i "s/\(bootstrap.servers\)=.*/\1=localhost:9092/g" build/install/win/kafka-zookeeper/kafka/config/connect-standalone.properties
%sed% -i "s/\(offset.storage.file.filename\)=.*/\1=kafka-offsets/g" build/install/win/kafka-zookeeper/kafka/config/connect-standalone.properties
%sed% -i "s/\(logger.kafka.controller\)=.*,/\1=INFO,/g" build/install/win/kafka-zookeeper/kafka/config/log4j.properties
%sed% -i "s/\(logger.state.change.logger\)=.*,/\1=INFO,/g" build/install/win/kafka-zookeeper/kafka/config/log4j.properties
echo log4j.logger.kafka.producer.async.DefaultEventHandler=INFO, kafkaAppender >> build/install/win/kafka-zookeeper/kafka/config/log4j.properties
echo exit /b 1 >> build/install/win/kafka-zookeeper/kafka/bin/windows/zookeeper-server-start.bat
echo exit /b 1 >> build/install/win/kafka-zookeeper/kafka/bin/windows/kafka-server-start.bat

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

REM echo ######## Delete temp files ########
del /f /q build\install\win\Files\config\sed*
del /f /q build\install\win\Files\nginx\conf\sed*
del /f /q build\install\win\Files\nginx\conf\includes\sed*
del /f /q build\install\win\kafka-zookeeper\zookeeper\conf\sed*
del /f /q build\install\win\kafka-zookeeper\kafka\config\sed*
del /f /q build\install\win\*.back.*

REM echo ######## Build kafka/zookeeper ########
%AdvancedInstaller% /rebuild "build\install\win\Apache ZooKeeper.aip"
copy "build\install\win\publish\Apache ZooKeeper.msi" "build\install\win\Apache ZooKeeper.msi" /y
%AdvancedInstaller% /rebuild "build\install\win\Apache Kafka.aip"
copy "build\install\win\publish\Apache Kafka.msi" "build\install\win\Apache Kafka.msi" /y

REM echo ######## Build MySQL Server Installer ########
iscc /Qp /S"byparam="signtool" sign /a /n "%publisher%" /t http://timestamp.digicert.com $f" "build\install\win\MySQL Server Installer Runner.iss"

REM echo ######## Build DocSpace package ########
%AdvancedInstaller% /edit build\install\win\DocSpace.aip /SetVersion %BUILD_VERSION%.%BUILD_NUMBER%
%AdvancedInstaller% /rebuild build\install\win\DocSpace.aip
