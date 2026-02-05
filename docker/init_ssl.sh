#!/bin/bash

DOMAIN="yourdomain.com"                     # <-- Replace with your domain
EMAIL="admin@yourdomain.com"                # <-- Replace with your email
APP_IMAGE="onlyoffice-docspace-preview"     # <-- Replace with your Docker app image
HTTP_PORT=8092                              # <-- Port your app listens to internally

while [ "$1" != "" ]; do
    case $1 in
    --domain )
    if [ "$2" != "" ]; then
			DOMAIN=$2
		       	shift
		fi
		;;

        --email )
		if [ "$2" != "" ]; then
			EMAIL=$2
			shift
		fi
		;;
	--port )
    if [ "$2" != "" ]; then
      HTTP_PORT=$2
      shift
    fi
    ;;
	--app )
    if [ "$2" != "" ]; then
      APP_IMAGE=$2
      shift
    fi
    ;;
    -? | -h | --help )
            echo "    Parameters:"
            echo "     --domain             Add your domain"
            echo "     --email              Add your email"
            echo "     --port               Add your app's port for internally"
            echo "     --app                Add your app docker service"
      	    echo "     -?, -h, --help       this help"
            exit 0
	  ;;

		* )
			echo "Unknown parameter $1" 1>&2
			exit 1
		;;
    esac
  shift
done

# Create directories if they do not exist

dirs=(
  "config/nginx/ssl/conf.d"
)

for dir in "${dirs[@]}"; do
  if [ -d "$dir" ]; then
    echo "Directory already exists: $(realpath "$dir")"
  else
    mkdir -p "$dir"
    echo "Created directory: $(realpath "$dir")"
  fi
done

# Preparing domain(s) config 
IFS=',' read -ra DOMAIN_LIST <<< "$DOMAIN"

MAIN_DOMAIN=$(echo "${DOMAIN_LIST[0]}" | xargs)

SERVER_NAME=""
DOMAIN_ARGS=""
for domain in "${DOMAIN_LIST[@]}"; do
  domain=$(echo "$domain" | xargs)
  SERVER_NAME+="$domain "
  DOMAIN_ARGS+=" -d $domain"
done

cat <<EOF > ./config/nginx/ssl/conf.d/onlyoffice-proxy.conf
map \$http_host \$this_host {
  "" \$host;
  default \$http_host;
}

map \$http_x_forwarded_port \$proxy_x_forwarded_port {
  default \$http_x_forwarded_port;
  '' \$server_port;
}

map \$http_x_forwarded_host \$proxy_x_forwarded_host {
  default \$http_x_forwarded_host;
  "" \$this_host;
}

map \$scheme \$proxy_x_forwarded_ssl {
  default off;
  https on;
}

map \$http_upgrade \$proxy_connection {
  default upgrade;
  '' close;
}

# Security headers
add_header Strict-Transport-Security "max-age=31536000; includeSubDomains" always;
add_header X-Content-Type-Options nosniff;
add_header X-Frame-Options "SAMEORIGIN";
add_header X-XSS-Protection "1; mode=block";
add_header Referrer-Policy "strict-origin";

# SSL protocols
ssl_protocols TLSv1.2 TLSv1.3;
ssl_prefer_server_ciphers on;
ssl_ciphers "EECDH+AESGCM:EDH+AESGCM:AES256+EECDH:AES256+EDH";
ssl_ecdh_curve secp384r1;
ssl_session_cache shared:SSL:10m;
ssl_session_tickets off;
ssl_stapling on;
ssl_stapling_verify on;
resolver 8.8.8.8 8.8.4.4 valid=300s;
resolver_timeout 5s;

server {
    listen 80;
    server_name ${SERVER_NAME};

    location /.well-known/acme-challenge/ {
        root /var/www/certbot;
    }

    location / {
        return 301 https://\$host\$request_uri;
    }
}

server {
    listen 443 ssl;
    server_name ${SERVER_NAME};

    ssl_certificate /etc/letsencrypt/live/${MAIN_DOMAIN}/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/${MAIN_DOMAIN}/privkey.pem;

    ssl_protocols TLSv1.2 TLSv1.3;
    ssl_ciphers HIGH:!aNULL:!MD5;

    client_max_body_size 4G;
    proxy_set_header Upgrade \$http_upgrade;
    proxy_set_header Connection \$proxy_connection;
    proxy_set_header Host \$host;
    proxy_set_header X-Forwarded-Host \$host;
    proxy_set_header X-Forwarded-Proto https;
    proxy_set_header X-Forwarded-For \$proxy_add_x_forwarded_for;
    proxy_hide_header 'Server';
    proxy_hide_header 'X-Powered-By';
    proxy_buffering off;

    location / {
        proxy_pass http://${APP_IMAGE}:${HTTP_PORT};
    }
}
EOF


docker compose -f docker-compose-preview.yml -f ssl.yml run --rm certbot certonly \
  --webroot --webroot-path=/var/www/certbot \
  --email ${EMAIL} --agree-tos --no-eff-email ${DOMAIN_ARGS}

docker compose -f docker-compose-preview.yml -f ssl.yml up -d onlyoffice-proxy
docker compose restart onlyoffice-proxy
