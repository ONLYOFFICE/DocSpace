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

if ! dpkg -l | grep -q "locales"; then
	apt-get install -yq locales
fi

if ! dpkg -l | grep -q "dirmngr"; then
	apt-get install -yq dirmngr
fi

if ! dpkg -l | grep -q "software-properties-common"; then
	apt-get install -yq software-properties-common
fi

if [ $(dpkg-query -W -f='${Status}' curl 2>/dev/null | grep -c "ok installed") -eq 0 ]; then
  apt-get install -yq curl;
fi

locale-gen en_US.UTF-8

# add elasticsearch repo
ELASTIC_VERSION="7.13.1"
ELASTIC_DIST=$(echo $ELASTIC_VERSION | awk '{ print int($1) }')
curl https://artifacts.elastic.co/GPG-KEY-elasticsearch | apt-key add -
echo "deb https://artifacts.elastic.co/packages/${ELASTIC_DIST}.x/apt stable main" | tee /etc/apt/sources.list.d/elastic-${ELASTIC_DIST}.x.list

# add nodejs repo
curl -sL https://deb.nodesource.com/setup_16.x | bash - 

#add dotnet repo
if [ "$DIST" = "debian" ] && [ "$DISTRIB_CODENAME" = "stretch" ]; then
	curl -sSL https://packages.microsoft.com/keys/microsoft.asc | apt-key add -
	wget -O /etc/apt/sources.list.d/microsoft-prod.list https://packages.microsoft.com/config/debian/9/prod.list
else
	curl https://packages.microsoft.com/config/$DIST/$REV/packages-microsoft-prod.deb -O
	dpkg -i packages-microsoft-prod.deb && rm packages-microsoft-prod.deb
fi

if ! dpkg -l | grep -q "mysql-server"; then

	MYSQL_SERVER_HOST=${MYSQL_SERVER_HOST:-"localhost"}
	MYSQL_SERVER_DB_NAME=${MYSQL_SERVER_DB_NAME:-"${package_sysname}"}
	MYSQL_SERVER_USER=${MYSQL_SERVER_USER:-"root"}
	MYSQL_SERVER_PASS=${MYSQL_SERVER_PASS:-"$(cat /dev/urandom | tr -dc A-Za-z0-9 | head -c 12)"}

	# setup mysql 8.0 package
	MYSQL_PACKAGE_NAME="mysql-apt-config_0.8.23-1_all.deb"
	curl -OL http://dev.mysql.com/get/${MYSQL_PACKAGE_NAME}
	echo "mysql-apt-config mysql-apt-config/repo-codename  select  $DISTRIB_CODENAME" | debconf-set-selections
	echo "mysql-apt-config mysql-apt-config/repo-distro  select  $DIST" | debconf-set-selections
	echo "mysql-apt-config mysql-apt-config/select-server  select  mysql-8.0" | debconf-set-selections
	DEBIAN_FRONTEND=noninteractive dpkg -i ${MYSQL_PACKAGE_NAME}
	rm -f ${MYSQL_PACKAGE_NAME}

	echo mysql-community-server mysql-community-server/root-pass password ${MYSQL_SERVER_PASS} | debconf-set-selections
	echo mysql-community-server mysql-community-server/re-root-pass password ${MYSQL_SERVER_PASS} | debconf-set-selections
	echo mysql-community-server mysql-server/default-auth-override select "Use Strong Password Encryption (RECOMMENDED)" | debconf-set-selections
	echo mysql-server-8.0 mysql-server/root_password password ${MYSQL_SERVER_PASS} | debconf-set-selections
	echo mysql-server-8.0 mysql-server/root_password_again password ${MYSQL_SERVER_PASS} | debconf-set-selections

	apt-get -y update
fi

if [ "$DIST" = "debian" ] && [ "$DISTRIB_CODENAME" = "stretch" ]; then
	apt-get install -yq mysql-server mysql-client --allow-unauthenticated
fi

# add redis repo
if [ "$DIST" = "ubuntu" ]; then	
	add-apt-repository -y ppa:chris-lea/redis-server
	if [ "$DISTRIB_CODENAME" = "jammy" ]; then sed -i 's/jammy/focal/g' /etc/apt/sources.list.d/*redis-server*.list; fi; #Fix missing repository for jammy
fi

#add nginx repo
curl http://nginx.org/keys/nginx_signing.key -O
apt-key add nginx_signing.key
echo "deb [arch=$ARCH] http://nginx.org/packages/$DIST $DISTRIB_CODENAME nginx" | tee /etc/apt/sources.list.d/nginx.list
rm nginx_signing.key

# setup msttcorefonts
echo ttf-mscorefonts-installer msttcorefonts/accepted-mscorefonts-eula select true | debconf-set-selections

# install
apt-get install -o DPkg::options::="--force-confnew" -yq \
				python3-pip \
				expect \
				nano \
				nodejs \
				gcc \
				make \
				dotnet-sdk-6.0 \
				mysql-server \
				mysql-client \
				postgresql \
				redis-server \
				rabbitmq-server \
				nginx-extras 
		
if [ -e /etc/redis/redis.conf ]; then
 sed -i "s/bind .*/bind 127.0.0.1/g" /etc/redis/redis.conf
 sed -r "/^save\s[0-9]+/d" -i /etc/redis/redis.conf
 
 service redis-server restart
fi

if [ ! -e /usr/bin/json ]; then
	npm i json -g >/dev/null 2>&1
fi

if ! dpkg -l | grep -q "elasticsearch"; then
	apt-get install -yq elasticsearch=${ELASTIC_VERSION}
fi

# disable apparmor for mysql
if which apparmor_parser && [ ! -f /etc/apparmor.d/disable/usr.sbin.mysqld ] && [ -f /etc/apparmor.d/disable/usr.sbin.mysqld ]; then
	ln -sf /etc/apparmor.d/usr.sbin.mysqld /etc/apparmor.d/disable/;
	apparmor_parser -R /etc/apparmor.d/usr.sbin.mysqld;
fi
