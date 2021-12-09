#!/bin/sh
NGINX_WORKER_PROCESSES=${NGINX_WORKER_PROCESSES:-$(grep processor /proc/cpuinfo | wc -l)};
NGINX_WORKER_CONNECTIONS=${NGINX_WORKER_CONNECTIONS:-$(ulimit -n)};

envsubst '$MAP_HASH_BUCKET_SIZE,$NGINX_WORKER_PROCESSES,$NGINX_WORKER_CONNECTIONS' < /etc/nginx/nginx.conf.template > /etc/nginx/nginx.conf
