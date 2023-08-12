#!/bin/bash

LETSENCRYPT_ROOT_DIR="/etc/letsencrypt/live";
DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
DOCKERFILE_DIR="${DIR}"
LETS_ENCRYPT_DIR="${DIR}/letsencrypt"
CONTAINER_NAME="onlyoffice-proxy"

if [ "$#" -ge "2" ]; then

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

    SSL_CERT="${LETSENCRYPT_ROOT_DIR}/${LETS_ENCRYPT_DOMAIN}/fullchain.pem";
    SSL_KEY="${LETSENCRYPT_ROOT_DIR}/${LETS_ENCRYPT_DOMAIN}/privkey.pem";

    docker ps -f name=$CONTAINER_NAME | grep -q $CONTAINER_NAME && docker stop $CONTAINER_NAME

    echo certbot certonly --expand --webroot --noninteractive --agree-tos --email $LETS_ENCRYPT_MAIL -d $LETS_ENCRYPT_DOMAIN > /var/log/le-start.log

    certbot certonly --expand --webroot --noninteractive --agree-tos --email $LETS_ENCRYPT_MAIL -d $LETS_ENCRYPT_DOMAIN > /var/log/le-new.log

    if [ -f ${SSL_CERT} -a -f ${SSL_KEY} ]; then
        if [ -f ${DOCKERFILE_DIR}/docspace.yml ]; then
            sed -i -e 's/^\(.*\)#\(.*443\)/\1\2/' \
                   -e 's/^\(.*\)-\(.*onlyoffice-proxy\.conf\)/\1#-\2/' \
                   -e 's/^\(.*\)#\(.*onlyoffice-proxy-ssl\.conf\)/\1\2/' \
                   -e '/tls/ s/#//' \
                   -e "s|-\( .*\):\(/etc/ssl/private/tls.key\)|- ${SSL_KEY}:\2|" \
                   -e "s|-\( .*\):\(/usr/local/share/ca-certificates/tls.crt\)|- ${SSL_CERT}:\2|" \
                   "${DOCKERFILE_DIR}/docspace.yml"
                   
            docker-compose -f ${DOCKERFILE_DIR}/docspace.yml up -d
        fi
    fi

    mkdir -p ${LETS_ENCRYPT_DIR}
    cat > ${LETS_ENCRYPT_DIR}/renew_letsencrypt.sh <<END
    certbot renew >> /var/log/le-renew.log
    docker ps -f name=$CONTAINER_NAME | grep -q $CONTAINER_NAME && docker stop $CONTAINER_NAME
    docker-compose -f ${DOCKERFILE_DIR}/docspace.yml up -d
END
    chmod a+x ${LETS_ENCRYPT_DIR}/renew_letsencrypt.sh

    [[ -d /etc/cron.d ]] && cat > /etc/cron.d/letsencrypt <<END
    @weekly root ${LETS_ENCRYPT_DIR}/renew_letsencrypt.sh
END

else
    echo "This script provided to automatically get Let's Encrypt SSL Certificates for DocSpace"
    echo "usage:"
    echo "  docspace-letsencrypt.sh EMAIL DOMAIN"
    echo "      EMAIL       Email used for registration and recovery contact. Use"
    echo "                  comma to register multiple emails, ex:"
    echo "                  u1@example.com,u2@example.com."
    echo "      DOMAIN      Domain name to apply"
fi