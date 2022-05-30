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
curl -sL https://deb.nodesource.com/setup_14.x | bash - 

#add yarn repo
curl -sL https://dl.yarnpkg.com/debian/pubkey.gpg | sudo apt-key add -
echo "deb https://dl.yarnpkg.com/debian/ stable main" | sudo tee /etc/apt/sources.list.d/yarn.list

#add dotnet repo
curl https://packages.microsoft.com/config/$DIST/$REV/packages-microsoft-prod.deb -O
dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

#install kafka
PRODUCT_DIR="/var/www/${product}"
if [ "$(ls "$PRODUCT_DIR/services/kafka" 2> /dev/null)" == "" ]; then
	mkdir -p ${PRODUCT_DIR}/services/kafka/
	if ! cat /etc/passwd | grep -q "onlyoffice"; then
		adduser --quiet --home ${PRODUCT_DIR} --system --group onlyoffice
	fi
	cd ${PRODUCT_DIR}/services/kafka
	KAFKA_VERSION=$(curl https://downloads.apache.org/kafka/ | grep -Eo '3.1.[0-9]' | tail -1)
	KAFKA_ARCHIVE=$(curl https://downloads.apache.org/kafka/$KAFKA_VERSION/ | grep -Eo "kafka_2.[0-9][0-9]-$KAFKA_VERSION.tgz" | tail -1)
	curl https://downloads.apache.org/kafka/$KAFKA_VERSION/$KAFKA_ARCHIVE -O
	tar xzf $KAFKA_ARCHIVE --strip 1 && rm -rf $KAFKA_ARCHIVE
	chown -R onlyoffice ${PRODUCT_DIR}/services/kafka/
	cd -
fi

if [ ! -e /lib/systemd/system/zookeeper.service ]; then
cat > /lib/systemd/system/zookeeper.service <<END
[Unit]
Requires=network.target remote-fs.target
After=network.target remote-fs.target
[Service]
Type=simple
User=onlyoffice
ExecStart=/bin/sh -c '${PRODUCT_DIR}/services/kafka/bin/zookeeper-server-start.sh ${PRODUCT_DIR}/services/kafka/config/zookeeper.properties > ${PRODUCT_DIR}/services/kafka/zookeeper.log 2>&1'
ExecStop=${PRODUCT_DIR}/services/kafka/bin/zookeeper-server-stop.sh
Restart=on-abnormal
[Install]
WantedBy=multi-user.target
END
fi

if [ ! -e /lib/systemd/system/kafka.service ]; then
cat > /lib/systemd/system/kafka.service <<END
[Unit]
Requires=zookeeper.service
After=zookeeper.service
[Service]
Type=simple
User=onlyoffice
ExecStart=/bin/sh -c '${PRODUCT_DIR}/services/kafka/bin/kafka-server-start.sh ${PRODUCT_DIR}/services/kafka/config/server.properties > ${PRODUCT_DIR}/services/kafka/kafka.log 2>&1'
ExecStop=${PRODUCT_DIR}/services/kafka/bin/kafka-server-stop.sh
Restart=on-abnormal
[Install]
WantedBy=multi-user.target
END
fi

if ! dpkg -l | grep -q "mysql-server"; then

	MYSQL_SERVER_HOST=${MYSQL_SERVER_HOST:-"localhost"}
	MYSQL_SERVER_DB_NAME=${MYSQL_SERVER_DB_NAME:-"${package_sysname}"}
	MYSQL_SERVER_USER=${MYSQL_SERVER_USER:-"root"}
	MYSQL_SERVER_PASS=${MYSQL_SERVER_PASS:-"$(cat /dev/urandom | tr -dc A-Za-z0-9 | head -c 12)"}

	# setup mysql 8.0 package
	MYSQL_PACKAGE_NAME="mysql-apt-config_0.8.22-1_all.deb"
	if [ "$DIST" = "debian" ] && [ "$DISTRIB_CODENAME" = "stretch" ]; then
		MYSQL_PACKAGE_NAME="mysql-apt-config_0.8.16-1_all.deb"
	fi
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

# add redis repo
if [ "$DIST" = "ubuntu" ]; then	
	add-apt-repository -y ppa:chris-lea/redis-server	
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
				expect \
				nano \
				nodejs \
				gcc \
				g++ \
				make \
				yarn \
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
