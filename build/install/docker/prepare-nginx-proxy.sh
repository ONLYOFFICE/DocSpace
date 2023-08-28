#!/bin/sh
REDIS_HOST=${REDIS_HOST:-"onlyoffice-redis"}

envsubst '$MAP_HASH_BUCKET_SIZE,$COUNT_WORKER_CONNECTIONS' < /etc/nginx/nginx.conf.template > /etc/nginx/nginx.conf
