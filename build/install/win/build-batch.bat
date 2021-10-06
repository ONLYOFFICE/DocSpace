REM echo ######## Extracting and preparing files to build ########
%sevenzip% x %cd%\build\install\win\nginx-1.21.1.zip -o"%cd%\build\install\win\Files" -y
xcopy "%cd%\build\install\win\Files\nginx-1.21.1" "%cd%\build\install\win\Files\nginx" /S /Y /B /I
rmdir %cd%\build\install\win\Files\nginx-1.21.1 /s /q
rmdir %cd%\build\install\win\kafka-zookeeper /s /q
md %cd%\build\install\win\kafka-zookeeper
md %cd%\build\install\win\Files\nginx\temp
md %cd%\build\install\win\Files\nginx\logs
%tar% -xvf %cd%\build\install\win\apache-zookeeper-3.7.0-bin.tar.gz -C %cd%\build\install\win\kafka-zookeeper
%tar% -xvf %cd%\build\install\win\kafka_2.12-2.8.0.tgz -C %cd%\build\install\win\kafka-zookeeper
REM del /f /q build\install\win\apache-zookeeper-3.7.0-bin.*
REM del /f /q build\install\win\kafka_2.12-2.8.0.*
copy "%cd%\build\install\win\zookeeper-server-save-start.bat" "%cd%\build\install\win\kafka-zookeeper\kafka_2.12-2.8.0\bin\windows\zookeeper-server-save-start.bat" /Y
copy "%cd%\build\install\win\kafka-server-save-start.bat" "%cd%\build\install\win\kafka-zookeeper\kafka_2.12-2.8.0\bin\windows\kafka-server-save-start.bat" /Y
xcopy "%cd%\build\install\win\tools" "%cd%\build\install\win\Files\tools" /S /Y /B /I
copy "%cd%\build\install\win\WinSW.NET4.exe" "%cd%\build\install\win\Files\tools\OnlyofficeProxy.exe" /Y
copy "%cd%\build\install\win\WinSW.NET4new.exe" "%cd%\build\install\win\Files\tools\kafka.exe" /Y
copy "%cd%\build\install\win\WinSW.NET4new.exe" "%cd%\build\install\win\Files\tools\zookeeper.exe" /Y
copy "%cd%\build\install\win\nginx.conf" "%cd%\build\install\win\Files\nginx\conf\nginx.conf" /Y
copy "%cd%\build\install\win\kafka-zookeeper\apache-zookeeper-3.7.0-bin\conf\zoo_sample.cfg" "%cd%\build\install\win\kafka-zookeeper\apache-zookeeper-3.7.0-bin\conf\zoo.cfg" /Y
del /f /q "%cd%\build\install\win\kafka-zookeeper\apache-zookeeper-3.7.0-bin\conf\zoo_sample.cfg"

REM echo ######## Edit zookeeper/kafka cfg and proprties files ########
%sed% -i "s/dataDir.*/dataDir=.\/..\/zookeeper\/Data/g" build/install/win/kafka-zookeeper/apache-zookeeper-3.7.0-bin/conf/zoo.cfg
%sed% -i "s/offset.storage.file.filename=.*/offset.storage.file.filename=kafka-offsets/g" build/install/win/kafka-zookeeper/kafka_2.12-2.8.0/config/connect-standalone.properties
%sed% -i "s/log.dirs=.*/log.dirs=kafka-logs/g" build/install/win/kafka-zookeeper/kafka_2.12-2.8.0/config/server.properties
%sed% -i "s/clientPort=.*/clientPort=2181/g" build/install/win/kafka-zookeeper/kafka_2.12-2.8.0/config/zookeeper.properties
%sed% -i "s/dataDir.*/dataDir=.\/..\/zookeeper\/Data/g" build/install/win/kafka-zookeeper/kafka_2.12-2.8.0/config/zookeeper.properties
%sed% -i "s/zookeeper.connect=.*/zookeeper.connect=localhost:2181/g" build/install/win/kafka-zookeeper/kafka_2.12-2.8.0/config/server.properties
%sed% -i "s/bootstrap.servers=.*/bootstrap.servers=localhost:9092/g" build/install/win/kafka-zookeeper/kafka_2.12-2.8.0/config/consumer.properties
%sed% -i "s/bootstrap.servers=.*/bootstrap.servers=localhost:9092/g" build/install/win/kafka-zookeeper/kafka_2.12-2.8.0/config/connect-standalone.properties
%sed% -i "s/logger.kafka.controller=.*,/logger.kafka.controller=INFO,/g" build/install/win/kafka-zookeeper/kafka_2.12-2.8.0/config/log4j.properties
%sed% -i "s/logger.state.change.logger=.*,/logger.state.change.logger=INFO,/g" build/install/win/kafka-zookeeper/kafka_2.12-2.8.0/config/log4j.properties
echo log4j.logger.kafka.producer.async.DefaultEventHandler=INFO, kafkaAppender >> build/install/win/kafka-zookeeper/kafka_2.12-2.8.0/config/log4j.properties

REM echo ######## Edit nginx conf files ########
%sed% -i "s!#rewrite!rewrite!g" build/install/win/Files/nginx/conf/onlyoffice.conf
%sed% -i "s!/etc/nginx/includes!includes!g" build/install/win/Files/nginx/conf/onlyoffice.conf
%sed% -i "s!/var/www!..!g" build/install/win/Files/nginx/conf/onlyoffice-*.conf
%sed% -i "s!/var/www!..!g" build/install/win/Files/nginx/conf/includes/onlyoffice-*.conf

REM echo ######## Edit json files ########
%sed% -i "s!\"machinekey\":.*!\"machinekey\": \"1123askdasjklasbnd\",!g" build/install/win/Files/config/appsettings.test.json
%sed% -i "s!\"path\":.*!\"path\": \"{APPDIRCONF}services\/ASC.Socket.IO\/service\",!g" build/install/win/Files/config/socket.json
%sed% -i "s!\"path\":.*!\"path\": \"{APPDIRCONF}services\/ASC.Thumbnails\/service\"!g" build/install/win/Files/config/thumb.test.json
%sed% -i "s!\"path\":.*!\"path\": \"{APPDIRCONF}services\/ASC.UrlShortener\/service\/index.js\"!g" build/install/win/Files/config/urlshortener.test.json
%sed% -i "s!\"folder\":.*!\"folder\": \"{APPDIRCONF}products\",!g" build/install/win/Files/config/appsettings.test.json

REM echo ######## Delete sed temp files ########
del /f /q build\install\win\Files\config\sed*
del /f /q build\install\win\Files\nginx\conf\sed*
del /f /q build\install\win\Files\nginx\conf\includes\sed*
del /f /q build\install\win\kafka-zookeeper\apache-zookeeper-3.7.0-bin\conf\sed*
del /f /q build\install\win\kafka-zookeeper\kafka_2.12-2.8.0\config\sed*