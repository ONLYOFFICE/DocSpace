#!/bin/bash

set -e

cat<<EOF

#######################################
#  INSTALL PREREQUISITES
#######################################

EOF

if [ "$DIST" = "debian" ] && [ $(apt-cache search ttf-mscorefonts-installer | wc -l) -eq 0 ]; then
		echo "deb http://ftp.uk.debian.org/debian/ $DISTRIB_CODENAME main contrib" >> /etc/apt/sources.list
		echo "deb-src http://ftp.uk.debian.org/debian/ $DISTRIB_CODENAME main contrib" >> /etc/apt/sources.list
fi

apt-get -y update

if ! command -v locale-gen &> /dev/null; then
	apt-get install -yq locales
fi

if ! dpkg -l | grep -q "apt-transport-https"; then
	apt-get install -yq apt-transport-https
fi

if ! dpkg -l | grep -q "software-properties-common"; then
	apt-get install -yq software-properties-common
fi

locale-gen en_US.UTF-8

# add elasticsearch repo
ELASTIC_VERSION="7.10.0"
ELASTIC_DIST=$(echo $ELASTIC_VERSION | awk '{ print int($1) }')
curl -fsSL https://artifacts.elastic.co/GPG-KEY-elasticsearch | gpg --no-default-keyring --keyring gnupg-ring:/usr/share/keyrings/elastic-${ELASTIC_DIST}.x.gpg --import
echo "deb [signed-by=/usr/share/keyrings/elastic-${ELASTIC_DIST}.x.gpg] https://artifacts.elastic.co/packages/${ELASTIC_DIST}.x/apt stable main" | tee /etc/apt/sources.list.d/elastic-${ELASTIC_DIST}.x.list
chmod 644 /usr/share/keyrings/elastic-${ELASTIC_DIST}.x.gpg

# add nodejs repo
[[ "$DISTRIB_CODENAME" =~ ^(bionic|stretch)$ ]] && NODE_VERSION="16" || NODE_VERSION="18"
echo "deb [signed-by=/usr/share/keyrings/nodesource.gpg] https://deb.nodesource.com/node_$NODE_VERSION.x nodistro main" | tee /etc/apt/sources.list.d/nodesource.list
curl -fsSL https://deb.nodesource.com/gpgkey/nodesource-repo.gpg.key | gpg --no-default-keyring --keyring gnupg-ring:/usr/share/keyrings/nodesource.gpg --import
chmod 644 /usr/share/keyrings/nodesource.gpg

#add dotnet repo
if [ "$DIST" = "debian" ] && [ "$DISTRIB_CODENAME" = "stretch" ]; then
	curl https://packages.microsoft.com/config/$DIST/10/packages-microsoft-prod.deb -O
elif [ "$DISTRIB_CODENAME" = "bookworm" ]; then
	#Temporary fix for missing dotnet repository for debian bookworm
	curl https://packages.microsoft.com/config/$DIST/11/packages-microsoft-prod.deb -O
else
	curl https://packages.microsoft.com/config/$DIST/$REV/packages-microsoft-prod.deb -O
fi
echo -e "Package: *\nPin: origin \"packages.microsoft.com\"\nPin-Priority: 1002" | tee /etc/apt/preferences.d/99microsoft-prod.pref
dpkg -i packages-microsoft-prod.deb && rm packages-microsoft-prod.deb

MYSQL_REPO_VERSION="$(curl https://repo.mysql.com | grep -oP 'mysql-apt-config_\K.*' | grep -o '^[^_]*' | sort --version-sort --field-separator=. | tail -n1)"
MYSQL_PACKAGE_NAME="mysql-apt-config_${MYSQL_REPO_VERSION}_all.deb"
if ! dpkg -l | grep -q "mysql-server"; then

	MYSQL_SERVER_HOST=${MYSQL_SERVER_HOST:-"localhost"}
	MYSQL_SERVER_DB_NAME=${MYSQL_SERVER_DB_NAME:-"${package_sysname}"}
	MYSQL_SERVER_USER=${MYSQL_SERVER_USER:-"root"}
	MYSQL_SERVER_PASS=${MYSQL_SERVER_PASS:-"$(cat /dev/urandom | tr -dc A-Za-z0-9 | head -c 12)"}

	# setup mysql 8.0 package
	curl -OL http://repo.mysql.com/${MYSQL_PACKAGE_NAME}
	echo "mysql-apt-config mysql-apt-config/repo-codename  select  $DISTRIB_CODENAME" | debconf-set-selections
	echo "mysql-apt-config mysql-apt-config/repo-distro  select  $DIST" | debconf-set-selections
	echo "mysql-apt-config mysql-apt-config/select-server  select  mysql-8.0" | debconf-set-selections
	DEBIAN_FRONTEND=noninteractive dpkg -i ${MYSQL_PACKAGE_NAME}
	rm -f ${MYSQL_PACKAGE_NAME}

	#Temporary fix for missing mysql repository for debian bookworm
	[ "$DISTRIB_CODENAME" = "bookworm" ] && sed -i "s/$DIST/ubuntu/g; s/$DISTRIB_CODENAME/jammy/g" /etc/apt/sources.list.d/mysql.list

	echo mysql-community-server mysql-community-server/root-pass password ${MYSQL_SERVER_PASS} | debconf-set-selections
	echo mysql-community-server mysql-community-server/re-root-pass password ${MYSQL_SERVER_PASS} | debconf-set-selections
	echo mysql-community-server mysql-server/default-auth-override select "Use Strong Password Encryption (RECOMMENDED)" | debconf-set-selections
	echo mysql-server-8.0 mysql-server/root_password password ${MYSQL_SERVER_PASS} | debconf-set-selections
	echo mysql-server-8.0 mysql-server/root_password_again password ${MYSQL_SERVER_PASS} | debconf-set-selections

elif dpkg -l | grep -q "mysql-apt-config" && [ "$(apt-cache policy mysql-apt-config | awk 'NR==2{print $2}')" != "${MYSQL_REPO_VERSION}" ]; then
	curl -OL http://repo.mysql.com/${MYSQL_PACKAGE_NAME}
	DEBIAN_FRONTEND=noninteractive dpkg -i ${MYSQL_PACKAGE_NAME}
	rm -f ${MYSQL_PACKAGE_NAME}
fi

if [ "$DIST" = "debian" ] && [ "$DISTRIB_CODENAME" = "stretch" ]; then
	apt-get install -yq mysql-server mysql-client --allow-unauthenticated
fi

# add redis repo
if [ "$DIST" = "ubuntu" ]; then	
	curl -fsSL https://packages.redis.io/gpg | gpg --no-default-keyring --keyring gnupg-ring:/usr/share/keyrings/redis.gpg --import
	echo "deb [signed-by=/usr/share/keyrings/redis.gpg] https://packages.redis.io/deb $DISTRIB_CODENAME main" | tee /etc/apt/sources.list.d/redis.list
	chmod 644 /usr/share/keyrings/redis.gpg
fi

#add openresty repo
curl -fsSL https://openresty.org/package/pubkey.gpg | gpg --no-default-keyring --keyring gnupg-ring:/usr/share/keyrings/openresty.gpg --import
echo "deb [signed-by=/usr/share/keyrings/openresty.gpg] http://openresty.org/package/$DIST $DISTRIB_CODENAME $([ "$DIST" = "ubuntu" ] && echo "main" || echo "openresty" )" | tee /etc/apt/sources.list.d/openresty.list
chmod 644 /usr/share/keyrings/openresty.gpg
#Temporary fix for missing openresty repository for debian bookworm
[ "$DISTRIB_CODENAME" = "bookworm" ] && sed -i "s/$DISTRIB_CODENAME/bullseye/g" /etc/apt/sources.list.d/openresty.list
systemctl list-units --type=service | grep -q nginx && systemctl stop nginx && systemctl disable nginx

# setup msttcorefonts
echo ttf-mscorefonts-installer msttcorefonts/accepted-mscorefonts-eula select true | debconf-set-selections

# install
apt-get -y update
apt-get install -o DPkg::options::="--force-confnew" -yq \
				expect \
				nano \
				nodejs \
				gcc \
				make \
				dotnet-sdk-7.0 \
				mysql-server \
				mysql-client \
				postgresql \
				redis-server \
				rabbitmq-server \
				openresty \
				ffmpeg 

if ! dpkg -l | grep -q "elasticsearch"; then
	apt-get install -yq elasticsearch=${ELASTIC_VERSION}
fi

# disable apparmor for mysql
if which apparmor_parser && [ ! -f /etc/apparmor.d/disable/usr.sbin.mysqld ] && [ -f /etc/apparmor.d/disable/usr.sbin.mysqld ]; then
	ln -sf /etc/apparmor.d/usr.sbin.mysqld /etc/apparmor.d/disable/;
	apparmor_parser -R /etc/apparmor.d/usr.sbin.mysqld;
fi
