#!/bin/bash

MYSQL_CONTAINER_NAME=${MYSQL_CONTAINER_NAME:-"localhost"}
MYSQL_HOST=${MYSQL_HOST:-${MYSQL_CONTAINER_NAME}}
MYSQL_PORT=${MYSQL_PORT:-"3306"}
MYSQL_DATABASE=${MYSQL_DATABASE:-"docspace"}
MYSQL_USER=${MYSQL_USER:-"onlyoffice_user"}
MYSQL_PASSWORD=${MYSQL_PASSWORD:-"onlyoffice_pass"}
MYSQL_ROOT_PASSWORD=${MYSQL_ROOT_PASSWORD:-"my-secret-pw"}

# Wait for MySQL to start (adjust sleep time if needed)
until mysqladmin ping -h ${MYSQL_HOST} --port=${MYSQL_PORT} -u root --silent; do
  sleep 2
done

# Create the database if it doesn't exist
mysql -h localhost -u root --skip-password -e "ALTER USER 'root'@'localhost' IDENTIFIED BY '${MYSQL_ROOT_PASSWORD}'; FLUSH PRIVILEGES;"
mysql -h localhost -u root --port=${MYSQL_PORT} -p${MYSQL_ROOT_PASSWORD} -e "CREATE DATABASE IF NOT EXISTS ${MYSQL_DATABASE};"
mysql -h localhost -u root --port=${MYSQL_PORT} -p${MYSQL_ROOT_PASSWORD} -e "CREATE USER IF NOT EXISTS '${MYSQL_USER}' IDENTIFIED BY '${MYSQL_PASSWORD}';"
mysql -h localhost -u root --port=${MYSQL_PORT} -p${MYSQL_ROOT_PASSWORD} -e "GRANT ALL ON \`${MYSQL_DATABASE//_/\\_}\`.* TO '$MYSQL_USER'@'%' ;" 
