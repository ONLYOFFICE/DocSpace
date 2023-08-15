#!/bin/bash

DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
LETSENCRYPT_CERTIFICATE_PATH="/etc/letsencrypt/live";
DOCKERCOMPOSE_DIR=$(dirname "$DIR")
SERVICES_FILES="-f ${DOCKERCOMPOSE_DIR}/notify.yml -f ${DOCKERCOMPOSE_DIR}/healthchecks.yml -f ${DOCKERCOMPOSE_DIR}/docspace.yml"
CONTAINER_NAME="onlyoffice-proxy"

if [ "$#" -ge "2" ]; then

    # Install certbot if not already installed
    if ! type "certbot" &> /dev/null; then
        if command_exists apt-get; then
            apt-get -y update -qq
            apt-get -y -q install certbot
        elif command_exists yum; then
            yum -y install certbot
        fi
    fi

    LETS_ENCRYPT_MAIL=$1
    LETS_ENCRYPT_DOMAIN=$2

    docker-compose ${SERVICES_FILES} -f ${DOCKERCOMPOSE_DIR}/proxy.yml down

    # Request and generate Let's Encrypt SSL certificate
    echo certbot certonly --expand --webroot --noninteractive --agree-tos --email ${LETS_ENCRYPT_MAIL} -d ${LETS_ENCRYPT_DOMAIN} > /var/log/le-start.log
    certbot certonly --expand --webroot --noninteractive --agree-tos --email ${LETS_ENCRYPT_MAIL} -d ${LETS_ENCRYPT_DOMAIN} > /var/log/le-new.log
    openssl dhparam -out /etc/ssl/certs/dhparam.pem 4096

    if [ -f "${LETSENCRYPT_CERTIFICATE_PATH}/${LETS_ENCRYPT_DOMAIN}/fullchain.pem" -a -f ${LETSENCRYPT_CERTIFICATE_PATH}/${LETS_ENCRYPT_DOMAIN}/privkey.pem ]; then
        if [ -f ${DOCKERCOMPOSE_DIR}/.env -a -f ${DOCKERCOMPOSE_DIR}/proxy-ssl.yml ]; then
            # Update .env file with Let's Encrypt domain
            sed -i "s~\(LETSENCRYPT_CERTIFICATE_PATH=\).*~\1\"${LETSENCRYPT_CERTIFICATE_PATH}/${LETS_ENCRYPT_DOMAIN}\"~g" ${DOCKERCOMPOSE_DIR}/.env
            sed -i "s~\(APP_URL_PORTAL=\).*~\1\"https://${LETS_ENCRYPT_DOMAIN}\"~g" ${DOCKERCOMPOSE_DIR}/.env

            docker-compose ${SERVICES_FILES} -f ${DOCKERCOMPOSE_DIR}/proxy-ssl.yml up -d

            # Create and set permissions for renew_letsencrypt.sh
            echo '#!/bin/bash' > ${DIR}/renew_letsencrypt.sh
            echo "docker ps -f name=$CONTAINER_NAME | grep -q $CONTAINER_NAME && docker stop $CONTAINER_NAME" >> ${DIR}/renew_letsencrypt.sh
            echo "certbot renew >> /var/log/le-renew.log" >> ${DIR}/renew_letsencrypt.sh
            echo "docker-compose -f ${DOCKERCOMPOSE_DIR}/proxy-ssl.yml up -d" >> ${DIR}/renew_letsencrypt.sh
            chmod a+x ${DIR}/renew_letsencrypt.sh

            # Add cron job if /etc/cron.d directory exists
            [ -d /etc/cron.d ] && echo -e "@weekly root ${DIR}/renew_letsencrypt.sh" | tee /etc/cron.d/letsencrypt
        fi
    fi
else
    echo "This script provided to automatically get Let's Encrypt SSL Certificates for DocSpace"
    echo "usage:"
    echo "  docspace-letsencrypt.sh EMAIL DOMAIN"
    echo "      EMAIL       Email used for registration and recovery contact. Use"
    echo "                  comma to register multiple emails, ex:"
    echo "                  u1@example.com,u2@example.com."
    echo "      DOMAIN      Domain name to apply"
fi
