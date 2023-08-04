#!/bin/bash

set -e

cat<<EOF

#######################################
#  INSTALL PREREQUISITES
#######################################

EOF

# clean yum cache
${package_manager} clean all

${package_manager} -y install yum-utils

DIST=$(rpm -q --whatprovides redhat-release || rpm -q --whatprovides centos-release);
DIST=$(echo $DIST | sed -n '/-.*/s///p');
REV=$(cat /etc/redhat-release | sed s/.*release\ // | sed s/\ .*//);
REV_PARTS=(${REV//\./ });
REV=${REV_PARTS[0]};

if ! [[ "$REV" =~ ^[0-9]+$ ]]; then
	REV=7;
fi

{ yum check-update postgresql; PSQLExitCode=$?; } || true #Checking for postgresql update
{ yum check-update $DIST*-release; exitCode=$?; } || true #Checking for distribution update

UPDATE_AVAILABLE_CODE=100
if [[ $exitCode -eq $UPDATE_AVAILABLE_CODE ]]; then
	res_unsupported_version
	echo $RES_UNSPPORTED_VERSION
	echo $RES_SELECT_INSTALLATION
	echo $RES_ERROR_REMINDER
	echo $RES_QUESTIONS
	read_unsupported_installation
fi

if rpm -qa | grep mariadb.*config >/dev/null 2>&1; then
   echo $RES_MARIADB && exit 0
fi

#Add repositories: EPEL, REMI and RPMFUSION
rpm -ivh https://dl.fedoraproject.org/pub/epel/epel-release-latest-$REV.noarch.rpm || true
rpm -ivh https://rpms.remirepo.net/enterprise/remi-release-$REV.rpm || true
yum localinstall -y --nogpgcheck https://download1.rpmfusion.org/free/el/rpmfusion-free-release-$REV.noarch.rpm

if [ "$REV" = "9" ]; then
	[ $DIST != "redhat" ] && TESTING_REPO="--enablerepo=crb" || /usr/bin/crb enable
	update-crypto-policies --set DEFAULT:SHA1
elif [ "$REV" = "8" ]; then
	[ $DIST != "redhat" ] && POWERTOOLS_REPO="--enablerepo=powertools" || /usr/bin/crb enable
fi

#add rabbitmq & erlang repo
curl -s https://packagecloud.io/install/repositories/rabbitmq/rabbitmq-server/script.rpm.sh | os=centos dist=$REV bash
curl -s https://packagecloud.io/install/repositories/rabbitmq/erlang/script.rpm.sh | os=centos dist=$REV bash

#add nodejs repo
[ "$REV" = "7" ] && NODE_VERSION="16" || NODE_VERSION="18"
curl -sL https://rpm.nodesource.com/setup_${NODE_VERSION}.x | sed 's/centos|/'$DIST'|/g' |  sudo bash - || true
rpm --import http://rpm.nodesource.com/pub/el/NODESOURCE-GPG-SIGNING-KEY-EL

#add dotnet repo
if [ $REV = "7" ] || [[ $DIST != "redhat" && $REV = "8" ]]; then
	rpm -Uvh https://packages.microsoft.com/config/centos/$REV/packages-microsoft-prod.rpm || true
fi

#add mysql repo
[ "$REV" != "7" ] && dnf remove -y @mysql && dnf module -y reset mysql && dnf module -y disable mysql
MYSQL_REPO_VERSION="$(curl https://repo.mysql.com | grep -oP "mysql80-community-release-el${REV}-\K.*" | grep -o '^[^.]*' | sort | tail -n1)"
yum localinstall -y https://repo.mysql.com/mysql80-community-release-el${REV}-${MYSQL_REPO_VERSION}.noarch.rpm || true

if ! rpm -q mysql-community-server; then
	MYSQL_FIRST_TIME_INSTALL="true";
fi

#add elasticsearch repo
ELASTIC_VERSION="7.10.0"
ELASTIC_DIST=$(echo $ELASTIC_VERSION | awk '{ print int($1) }')
rpm --import https://artifacts.elastic.co/GPG-KEY-elasticsearch
cat > /etc/yum.repos.d/elasticsearch.repo <<END
[elasticsearch]
name=Elasticsearch repository for ${ELASTIC_DIST}.x packages
baseurl=https://artifacts.elastic.co/packages/${ELASTIC_DIST}.x/yum
gpgcheck=1
gpgkey=https://artifacts.elastic.co/GPG-KEY-elasticsearch
enabled=0
autorefresh=1
type=rpm-md
END

# add nginx repo
cat > /etc/yum.repos.d/nginx.repo <<END
[nginx-stable]
name=nginx stable repo
baseurl=https://nginx.org/packages/centos/$REV/\$basearch/
gpgcheck=1
enabled=1
gpgkey=https://nginx.org/keys/nginx_signing.key
module_hotfixes=true
END

${package_manager} -y install epel-release \
			python3 \
			nodejs \
			dotnet-sdk-7.0 \
			elasticsearch-${ELASTIC_VERSION} --enablerepo=elasticsearch \
			mysql-server \
			nginx \
			postgresql \
			postgresql-server \
			rabbitmq-server$rabbitmq_version \
			redis --enablerepo=remi \
			SDL2 $POWERTOOLS_REPO \
			expect \
			ffmpeg $TESTING_REPO

if [[ $PSQLExitCode -eq $UPDATE_AVAILABLE_CODE ]]; then
	yum -y install postgresql-upgrade
	postgresql-setup --upgrade || true
fi
postgresql-setup initdb	|| true

semanage permissive -a httpd_t

package_services="rabbitmq-server postgresql redis nginx mysqld"
