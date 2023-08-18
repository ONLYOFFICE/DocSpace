#!/bin/sh
REDIS_HOST=${REDIS_HOST:-"onlyoffice-redis"}

envsubst '$MAP_HASH_BUCKET_SIZE,$COUNT_WORKER_CONNECTIONS' < /etc/nginx/nginx.conf.template > /etc/nginx/nginx.conf
sed -i "s!redis_host = \"127.0.0.1\"!redis_host = \"${REDIS_HOST}\"!g"  /etc/nginx/conf.d/onlyoffice.conf
