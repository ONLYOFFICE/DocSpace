#!/bin/bash 
PARAMETERS=${PARAMETERS:-""}
# read parameters

if [ -n "$1" ]; then
	RUN_FILE="${1}";
	shift
fi

if [ -n "$1" ]; then
	NAME_SERVICE="${1}";
	shift
fi

while [ "$1" != "" ]; do
	PARAMETERS="$PARAMETERS ${1}";
	echo $PARAMETERS
	shift
done

echo "Executing -- ${NAME_SERVICE}"

PRODUCT=${PRODUCT:-"onlyoffice"}
ENV_EXTENSION=${ENV_EXTENSION:-"test"}
PROXY_HOST=${PROXY_HOST:-"proxy"}

SHEME=${SHEME:-"http"}
SERVICE_PORT=${SERVICE_PORT:-"5050"}
URLS=${URLS:-"${SHEME}://0.0.0.0:${SERVICE_PORT}"}

PATH_TO_CONF=${PATH_TO_CONF:-"/app/${PRODUCT}/config"}
LOG_DIR=${LOG_DIR:-"/var/log/${PRODUCT}"}


python3 /app/modify-json-config.py

node ${RUN_FILE} --app.port=${SERVICE_PORT} --app.appsettings=${PATH_TO_CONF} --app.environment=${ENV_EXTENSION}
