#!/bin/sh

until cat /var/www/products/status.txt
do
  echo "waiting for the storage to be ready"
  sleep 5
done
