#!/bin/bash 

MYSQL_HOST=${MYSQL_HOST:-"localhost"}
MYSQL_DATABASE=${MYSQL_DATABASE:-"onlyoffice"}
MYSQL_USER=${MYSQL_USER:-"onlyoffice_user"}
MYSQL_PASSWORD=${MYSQL_PASSWORD:-"onlyoffice_pass"}

sed -i "s!\"ConnectionString\".*!\"ConnectionString\": \"Server=${MYSQL_HOST};Database=${MYSQL_DATABASE};User ID=${MYSQL_USER};Password=${MYSQL_PASSWORD}\",!g" ./appsettings.json

dotnet ASC.Migration.Runner.dll
