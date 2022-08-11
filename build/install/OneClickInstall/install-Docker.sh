#!/bin/bash

 #
 # (c) Copyright Ascensio System SIA 2021
 #
 # This program is a free software product. You can redistribute it and/or
 # modify it under the terms of the GNU Affero General Public License (AGPL)
 # version 3 as published by the Free Software Foundation. In accordance with
 # Section 7(a) of the GNU AGPL its Section 15 shall be amended to the effect
 # that Ascensio System SIA expressly excludes the warranty of non-infringement
 # of any third-party rights.
 #
 # This program is distributed WITHOUT ANY WARRANTY; without even the implied
 # warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For
 # details, see the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
 #
 # You can contact Ascensio System SIA at 20A-12 Ernesta Birznieka-Upisha
 # street, Riga, Latvia, EU, LV-1050.
 #
 # The  interactive user interfaces in modified source and object code versions
 # of the Program must display Appropriate Legal Notices, as required under
 # Section 5 of the GNU AGPL version 3.
 #
 # Pursuant to Section 7(b) of the License you must retain the original Product
 # logo when distributing the program. Pursuant to Section 7(e) we decline to
 # grant you any rights under trademark law for use of our trademarks.
 #
 # All the Product's GUI elements, including illustrations and icon sets, as
 # well as technical writing content are licensed under the terms of the
 # Creative Commons Attribution-ShareAlike 4.0 International. See the License
 # terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode
 #

PACKAGE_SYSNAME="onlyoffice"
PRODUCT="appserver"
BASE_DIR="/app/$PACKAGE_SYSNAME";
STATUS=""
DOCKER_TAG=""
GIT_BRANCH="develop"

NETWORK=${PACKAGE_SYSNAME}

DISK_REQUIREMENTS=40960;
MEMORY_REQUIREMENTS=5500;
CORE_REQUIREMENTS=2;

DIST="";
REV="";
KERNEL="";

INSTALL_KAFKA="true";
INSTALL_MYSQL_SERVER="true";
INSTALL_DOCUMENT_SERVER="true";
INSTALL_APPSERVER="true";
UPDATE="false";

HUB="";
USERNAME="";
PASSWORD="";

MYSQL_DATABASE=""
MYSQL_USER=""
MYSQL_PASSWORD=""
MYSQL_ROOT_PASSWORD=""
MYSQL_HOST=""
DATABASE_MIGRATION="true"

ZOO_PORT=""
ZOO_HOST=""
KAFKA_HOST=""

ELK_HOST=""

DOCUMENT_SERVER_IMAGE_NAME=onlyoffice/4testing-documentserver-ee:latest
DOCUMENT_SERVER_JWT_SECRET=""
DOCUMENT_SERVER_HOST=""

APP_CORE_BASE_DOMAIN=""
APP_CORE_MACHINEKEY=""
APP_DOTNET_ENV=""

HELP_TARGET="install-Docker.sh";

SKIP_HARDWARE_CHECK="false";

EXTERNAL_PORT="8092"
SERVICE_PORT="5050"

while [ "$1" != "" ]; do
	case $1 in

		-u | --update )
			if [ "$2" != "" ]; then
				UPDATE=$2
				shift
			fi
		;;

		-hub | --hub )
			if [ "$2" != "" ]; then
				HUB=$2
				shift
			fi
		;;

		-un | --username )
			if [ "$2" != "" ]; then
				USERNAME=$2
				shift
			fi
		;;

		-p | --password )
			if [ "$2" != "" ]; then
				PASSWORD=$2
				shift
			fi
		;;

		-ias | --installappserver )
			if [ "$2" != "" ]; then
				INSTALL_APPSERVER=$2
				shift
			fi
		;;

		-ids | --installdocumentserver )
			if [ "$2" != "" ]; then
				INSTALL_DOCUMENT_SERVER=$2
				shift
			fi
		;;

		-imysql | --installmysql )
			if [ "$2" != "" ]; then
				INSTALL_MYSQL_SERVER=$2
				shift
			fi
		;;		
		
		-ikafka | --installkafka )
			if [ "$2" != "" ]; then
				INSTALL_KAFKA=$2
				shift
			fi
		;;

		-ht | --helptarget )
			if [ "$2" != "" ]; then
				HELP_TARGET=$2
				shift
			fi
		;;

		-mysqld | --mysqldatabase )
			if [ "$2" != "" ]; then
				MYSQL_DATABASE=$2
				shift
			fi
		;;

		-mysqlrp | --mysqlrootpassword )
			if [ "$2" != "" ]; then
				MYSQL_ROOT_PASSWORD=$2
				shift
			fi
		;;

		-mysqlu | --mysqluser )
			if [ "$2" != "" ]; then
				MYSQL_USER=$2
				shift
			fi
		;;

		-mysqlh | --mysqlhost )
			if [ "$2" != "" ]; then
				MYSQL_HOST=$2
				shift
			fi
		;;

		-mysqlp | --mysqlpassword )
			if [ "$2" != "" ]; then
				MYSQL_PASSWORD=$2
				shift
			fi
		;;

		-zp | --zookeeperport )
			if [ "$2" != "" ]; then
				ZOO_PORT=$2
				shift
			fi
		;;

		-zh | --zookeeperhost )
			if [ "$2" != "" ]; then
				ZOO_HOST=$2
				shift
			fi
		;;

		-kh | --kafkahost )
			if [ "$2" != "" ]; then
				KAFKA_HOST=$2
				shift
			fi
		;;

		-esh | --elasticsearchhost )
			if [ "$2" != "" ]; then
				ELK_HOST=$2
				shift
			fi
		;;

		-skiphc | --skiphardwarecheck )
			if [ "$2" != "" ]; then
				SKIP_HARDWARE_CHECK=$2
				shift
			fi
		;;

		-ip | --internalport )
			if [ "$2" != "" ]; then
				SERVICE_PORT=$2
				shift
			fi
		;;

		-ep | --externalport )
			if [ "$2" != "" ]; then
				EXTERNAL_PORT=$2
				shift
			fi
		;;

		-ash | --appserverhost )
			if [ "$2" != "" ]; then
				APP_CORE_BASE_DOMAIN=$2
				shift
			fi
		;;
		
		-mk | --machinekey )
			if [ "$2" != "" ]; then
				APP_CORE_MACHINEKEY=$2
				shift
			fi
		;;
		
		-env | --environment )
			if [ "$2" != "" ]; then
				APP_DOTNET_ENV=$2
				shift
			fi
		;;

		-s | --status )
			if [ "$2" != "" ]; then
				STATUS=$2
				shift
			fi
		;;

		-ls | --localscripts )
			if [ "$2" != "" ]; then
				shift
			fi
		;;
		
		-tag | --dockertag )
			if [ "$2" != "" ]; then
				DOCKER_TAG=$2
				shift
			fi
		;;
		
		-gb | --gitbranch )
			if [ "$2" != "" ]; then
				PARAMETERS="$PARAMETERS ${1}";
				GIT_BRANCH=$2
				shift
			fi
		;;
		
		-di | --documentserverimage )
			if [ "$2" != "" ]; then
				DOCUMENT_SERVER_IMAGE_NAME=$2
				shift
			fi
		;;
		
		-dbm | --databasemigration )
			if [ "$2" != "" ]; then
				DATABASE_MIGRATION=$2
				shift
			fi
		;;

		-? | -h | --help )
			echo "  Usage: bash $HELP_TARGET [PARAMETER] [[PARAMETER], ...]"
			echo
			echo "    Parameters:"
			echo "      -hub, --hub                       dockerhub name"
			echo "      -un, --username                   dockerhub username"
			echo "      -p, --password                    dockerhub password"
			echo "      -ias, --installappserver          install or update appserver (true|false)"
			echo "      -tag, --dockertag                 select the version to install appserver (latest|develop|version number)"
			echo "      -ids, --installdocumentserver     install or update document server (true|false)"
			echo "      -di, --documentserverimage        document server image name"
			echo "      -imysql, --installmysql           install or update mysql (true|false)"			
			echo "      -ikafka, --installkafka           install or update kafka (true|false)"
			echo "      -mysqlrp, --mysqlrootpassword     mysql server root password"
			echo "      -mysqld, --mysqldatabase          appserver database name"
			echo "      -mysqlu, --mysqluser              appserver database user"
			echo "      -mysqlp, --mysqlpassword          appserver database password"
			echo "      -mysqlh, --mysqlhost              mysql server host"
			echo "      -ash, --appserverhost             appserver host"
			echo "      -zp, --zookeeperport              zookeeper port (default value 2181)"
			echo "      -zh, --zookeeperhost              zookeeper host"
			echo "      -kh, --kafkahost                  kafka host"
			echo "      -esh, --elasticsearchhost         elasticsearch host"
			echo "      -env, --environment               appserver environment"
			echo "      -skiphc, --skiphardwarecheck      skip hardware check (true|false)"
			echo "      -ip, --internalport               internal appserver port (default value 5050)"
			echo "      -ep, --externalport               external appserver port (default value 8092)"
			echo "      -mk, --machinekey                 setting for core.machinekey"
			echo "      -ls, --local_scripts              run the installation from local scripts"
			echo "      -dbm, --databasemigration         database migration (true|false)"
			echo "      -?, -h, --help                    this help"
			echo
			echo "    Install all the components without document server:"
			echo "      bash $HELP_TARGET -ids false"
			echo
			echo "    Install Document Server only. Skip the installation of MYSQL and Appserver:"
			echo "      bash $HELP_TARGET -ias false -ids true -imysql false -ims false"
			echo "    Update all installed components. Stop the containers that need to be updated, remove them and run the latest versions of the corresponding components. The portal data should be picked up automatically:"
			echo "      bash $HELP_TARGET -u true"
			echo
			echo "    Update Document Server only to version 4.4.2.20 and skip the update for all other components:"
			echo "      bash $HELP_TARGET -u true -dv 4.4.2.20 -ias false"
			echo
			echo "    Update Appserver only to version 0.1.10 and skip the update for all other components:"
			echo "      bash $HELP_TARGET -u true -av 9.1.0.393 -ids false"
			echo
			exit 0
		;;

		* )
			echo "Unknown parameter $1" 1>&2
			exit 1
		;;
	esac
	shift
done

root_checking () {
	if [ ! $( id -u ) -eq 0 ]; then
		echo "To perform this action you must be logged in with root rights"
		exit 1;
	fi
}

command_exists () {
    type "$1" &> /dev/null;
}

file_exists () {
	if [ -z "$1" ]; then
		echo "file path is empty"
		exit 1;
	fi

	if [ -f "$1" ]; then
		return 0; #true
	else
		return 1; #false
	fi
}

to_lowercase () {
	echo "$1" | awk '{print tolower($0)}'
}

trim () {
	echo -e "$1" | sed -e 's/^[[:space:]]*//' -e 's/[[:space:]]*$//'
}

get_random_str () {
	LENGTH=$1;

	if [[ -z ${LENGTH} ]]; then
		LENGTH=12;
	fi

	VALUE=$(cat /dev/urandom | tr -dc A-Za-z0-9 | head -c ${LENGTH});
	echo "$VALUE"
}

get_os_info () {
	OS=`to_lowercase \`uname\``

	if [ "${OS}" == "windowsnt" ]; then
		echo "Not supported OS";
		exit 1;
	elif [ "${OS}" == "darwin" ]; then
		echo "Not supported OS";
		exit 1;
	else
		OS=`uname`

		if [ "${OS}" == "SunOS" ] ; then
			echo "Not supported OS";
			exit 1;
		elif [ "${OS}" == "AIX" ] ; then
			echo "Not supported OS";
			exit 1;
		elif [ "${OS}" == "Linux" ] ; then
			MACH=`uname -m`

			if [ "${MACH}" != "x86_64" ]; then
				echo "Currently only supports 64bit OS's";
				exit 1;
			fi

			KERNEL=`uname -r`

			if [ -f /etc/redhat-release ] ; then
				CONTAINS=$(cat /etc/redhat-release | { grep -sw release || true; });
				if [[ -n ${CONTAINS} ]]; then
					DIST=`cat /etc/redhat-release |sed s/\ release.*//`
					REV=`cat /etc/redhat-release | sed s/.*release\ // | sed s/\ .*//`
				else
					DIST=`cat /etc/os-release | grep -sw 'ID' | awk -F=  '{ print $2 }' | sed -e 's/^"//' -e 's/"$//'`
					REV=`cat /etc/os-release | grep -sw 'VERSION_ID' | awk -F=  '{ print $2 }' | sed -e 's/^"//' -e 's/"$//'`
				fi
			elif [ -f /etc/SuSE-release ] ; then
				REV=`cat /etc/os-release  | grep '^VERSION_ID' | awk -F=  '{ print $2 }' |  sed -e 's/^"//'  -e 's/"$//'`
				DIST='SuSe'
			elif [ -f /etc/debian_version ] ; then
				REV=`cat /etc/debian_version`
				DIST='Debian'
				if [ -f /etc/lsb-release ] ; then
					DIST=`cat /etc/lsb-release | grep '^DISTRIB_ID' | awk -F=  '{ print $2 }'`
					REV=`cat /etc/lsb-release | grep '^DISTRIB_RELEASE' | awk -F=  '{ print $2 }'`
				elif [ -f /etc/lsb_release ] || [ -f /usr/bin/lsb_release ] ; then
					DIST=`lsb_release -a 2>&1 | grep 'Distributor ID:' | awk -F ":" '{print $2 }'`
					REV=`lsb_release -a 2>&1 | grep 'Release:' | awk -F ":" '{print $2 }'`
				fi
			elif [ -f /etc/os-release ] ; then
				DIST=`cat /etc/os-release | grep -sw 'ID' | awk -F=  '{ print $2 }' | sed -e 's/^"//' -e 's/"$//'`
				REV=`cat /etc/os-release | grep -sw 'VERSION_ID' | awk -F=  '{ print $2 }' | sed -e 's/^"//' -e 's/"$//'`
			fi
		fi

		DIST=$(trim $DIST);
		REV=$(trim $REV);
	fi
}

check_os_info () {
	if [[ -z ${KERNEL} || -z ${DIST} || -z ${REV} ]]; then
		echo "$KERNEL, $DIST, $REV";
		echo "Not supported OS";
		exit 1;
	fi
}

check_kernel () {
	MIN_NUM_ARR=(3 10 0);
	CUR_NUM_ARR=();

	CUR_STR_ARR=$(echo $KERNEL | grep -Po "[0-9]+\.[0-9]+\.[0-9]+" | tr "." " ");
	for CUR_STR_ITEM in $CUR_STR_ARR
	do
		CUR_NUM_ARR=(${CUR_NUM_ARR[@]} $CUR_STR_ITEM)
	done

	INDEX=0;

	while [[ $INDEX -lt 3 ]]; do
		if [ ${CUR_NUM_ARR[INDEX]} -lt ${MIN_NUM_ARR[INDEX]} ]; then
			echo "Not supported OS Kernel"
			exit 1;
		elif [ ${CUR_NUM_ARR[INDEX]} -gt ${MIN_NUM_ARR[INDEX]} ]; then
			INDEX=3
		fi
		(( INDEX++ ))
	done
}

check_hardware () {
	AVAILABLE_DISK_SPACE=$(df -m /  | tail -1 | awk '{ print $4 }');

	if [ ${AVAILABLE_DISK_SPACE} -lt ${DISK_REQUIREMENTS} ]; then
		echo "Minimal requirements are not met: need at least $DISK_REQUIREMENTS MB of free HDD space"
		exit 1;
	fi

	TOTAL_MEMORY=$(free -m | grep -oP '\d+' | head -n 1);

	if [ ${TOTAL_MEMORY} -lt ${MEMORY_REQUIREMENTS} ]; then
		echo "Minimal requirements are not met: need at least $MEMORY_REQUIREMENTS MB of RAM"
		exit 1;
	fi

	CPU_CORES_NUMBER=$(cat /proc/cpuinfo | grep processor | wc -l);

	if [ ${CPU_CORES_NUMBER} -lt ${CORE_REQUIREMENTS} ]; then
		echo "The system does not meet the minimal hardware requirements. CPU with at least $CORE_REQUIREMENTS cores is required"
		exit 1;
	fi
}

install_service () {
	local COMMAND_NAME=$1
	local PACKAGE_NAME=$2

	PACKAGE_NAME=${PACKAGE_NAME:-"$COMMAND_NAME"}

	if command_exists apt-get; then
		apt-get -y update -qq
		apt-get -y -q install $PACKAGE_NAME
	elif command_exists yum; then
		yum -y install $PACKAGE_NAME
	fi

	if ! command_exists $COMMAND_NAME; then
		echo "command $COMMAND_NAME not found"
		exit 1;
	fi
}

install_docker_compose () {
	if ! command_exists python3; then
		install_service python3
	fi

	if command_exists apt-get; then
		apt-get -y update -qq
		apt-get -y -q install python3-pip
	elif command_exists yum; then
		curl -O https://bootstrap.pypa.io/get-pip.py
		python3 get-pip.py || true
		rm get-pip.py
	fi	

	python3 -m pip install --upgrade pip
	python3 -m pip install docker-compose
	sudo ln -s /usr/local/bin/docker-compose /usr/bin/docker-compose

	if ! command_exists docker-compose; then
		echo "command docker-compose not found"
		exit 1;
	fi
}

check_ports () {
	RESERVED_PORTS=(443 2181 2888 3306 3888 8081 8099 9092 9200 9300 9800 9899 9999 33060);
	ARRAY_PORTS=();
	USED_PORTS="";

	if ! command_exists netstat; then
		install_service netstat net-tools
	fi

	if [ "${EXTERNAL_PORT//[0-9]}" = "" ]; then
		for RESERVED_PORT in "${RESERVED_PORTS[@]}"
		do
			if [ "$RESERVED_PORT" -eq "$EXTERNAL_PORT" ] ; then
				echo "External port $EXTERNAL_PORT is reserved. Select another port"
				exit 1;
			fi

			if [ "$RESERVED_PORT" -eq "$SERVICE_PORT" ] ; then
				echo "Internal port $SERVICE_PORT is reserved. Select another port"
				exit 1;
			fi
		done
	else
		echo "Invalid external port $EXTERNAL_PORT"
		exit 1;
	fi

	if [ "$INSTALL_APPSERVER" == "true" ]; then
		ARRAY_PORTS=(${ARRAY_PORTS[@]} "$EXTERNAL_PORT");
	fi

	for PORT in "${ARRAY_PORTS[@]}"
	do
		REGEXP=":$PORT$"
		CHECK_RESULT=$(netstat -lnt | awk '{print $4}' | { grep $REGEXP || true; })

		if [[ $CHECK_RESULT != "" ]]; then
			if [[ $USED_PORTS != "" ]]; then
				USED_PORTS="$USED_PORTS, $PORT"
			else
				USED_PORTS="$PORT"
			fi
		fi
	done

	if [[ $USED_PORTS != "" ]]; then
		echo "The following TCP Ports must be available: $USED_PORTS"
		exit 1;
	fi
}

check_docker_version () {
	CUR_FULL_VERSION=$(docker -v | cut -d ' ' -f3 | cut -d ',' -f1);
	CUR_VERSION=$(echo $CUR_FULL_VERSION | cut -d '-' -f1);
	CUR_EDITION=$(echo $CUR_FULL_VERSION | cut -d '-' -f2);

	if [ "${CUR_EDITION}" == "ce" ] || [ "${CUR_EDITION}" == "ee" ]; then
		return 0;
	fi

	if [ "${CUR_VERSION}" != "${CUR_EDITION}" ]; then
		echo "Unspecific docker version"
		exit 1;
	fi

	MIN_NUM_ARR=(1 10 0);
	CUR_NUM_ARR=();

	CUR_STR_ARR=$(echo $CUR_VERSION | grep -Po "[0-9]+\.[0-9]+\.[0-9]+" | tr "." " ");

	for CUR_STR_ITEM in $CUR_STR_ARR
	do
		CUR_NUM_ARR=(${CUR_NUM_ARR[@]} $CUR_STR_ITEM)
	done

	INDEX=0;

	while [[ $INDEX -lt 3 ]]; do
		if [ ${CUR_NUM_ARR[INDEX]} -lt ${MIN_NUM_ARR[INDEX]} ]; then
			echo "The outdated Docker version has been found. Please update to the latest version."
			exit 1;
		elif [ ${CUR_NUM_ARR[INDEX]} -gt ${MIN_NUM_ARR[INDEX]} ]; then
			return 0;
		fi
		(( INDEX++ ))
	done
}

install_docker_using_script () {
	if ! command_exists curl ; then
		install_service curl
	fi

	curl -fsSL https://get.docker.com -o get-docker.sh
	sh get-docker.sh
	rm get-docker.sh
}

install_docker () {

	if [ "${DIST}" == "Ubuntu" ] || [ "${DIST}" == "Debian" ] || [[ "${DIST}" == CentOS* ]] || [ "${DIST}" == "Fedora" ]; then

		install_docker_using_script
		systemctl start docker
		systemctl enable docker

	elif [ "${DIST}" == "Red Hat Enterprise Linux Server" ]; then

		echo ""
		echo "Your operating system does not allow Docker CE installation."
		echo "You can install Docker EE using the manual here - https://docs.docker.com/engine/installation/linux/rhel/"
		echo ""
		exit 1;

	elif [ "${DIST}" == "SuSe" ]; then

		echo ""
		echo "Your operating system does not allow Docker CE installation."
		echo "You can install Docker EE using the manual here - https://docs.docker.com/engine/installation/linux/suse/"
		echo ""
		exit 1;

	elif [ "${DIST}" == "altlinux" ]; then

		apt-get -y install docker-io
		chkconfig docker on
		service docker start
		systemctl enable docker

	else

		echo ""
		echo "Docker could not be installed automatically."
		echo "Please use this official instruction https://docs.docker.com/engine/installation/linux/other/ for its manual installation."
		echo ""
		exit 1;

	fi

	if ! command_exists docker ; then
		echo "error while installing docker"
		exit 1;
	fi
}

docker_login () {
	if [[ -n ${USERNAME} && -n ${PASSWORD}  ]]; then
		docker login ${HUB} --username ${USERNAME} --password ${PASSWORD}
	fi
}

create_network () {
	EXIST=$(docker network ls | awk '{print $2;}' | { grep -x ${NETWORK} || true; });

	if [[ -z ${EXIST} ]]; then
		docker network create --driver bridge ${NETWORK}
	fi
}

get_container_env_parameter () {
	local CONTAINER_NAME=$1;
	local PARAMETER_NAME=$2;
	VALUE="";

	if [[ -z ${CONTAINER_NAME} ]]; then
		echo "Empty container name"
		exit 1;
	fi

	if [[ -z ${PARAMETER_NAME} ]]; then
		echo "Empty parameter name"
		exit 1;
	fi

	if command_exists docker ; then
		CONTAINER_EXIST=$(docker ps -aqf "name=$CONTAINER_NAME");

		if [[ -n ${CONTAINER_EXIST} ]]; then
			VALUE=$(docker inspect --format='{{range .Config.Env}}{{println .}}{{end}}' ${CONTAINER_NAME} | grep "${PARAMETER_NAME}=" | sed 's/^.*=//');
		fi
	fi

	echo "$VALUE"
}

set_jwt_secret () {
	CURRENT_JWT_SECRET="";

	if [[ -z ${JWT_SECRET} ]]; then
		CURRENT_JWT_SECRET=$(get_container_env_parameter "${PACKAGE_SYSNAME}-document-server" "JWT_SECRET");

		if [[ -n ${CURRENT_JWT_SECRET} ]]; then
			DOCUMENT_SERVER_JWT_SECRET="$CURRENT_JWT_SECRET";
		fi
	fi

	if [[ -z ${JWT_SECRET} ]]; then
		CURRENT_JWT_SECRET=$(get_container_env_parameter "${PACKAGE_SYSNAME}-api" "DOCUMENT_SERVER_JWT_SECRET");

		if [[ -n ${CURRENT_JWT_SECRET} ]]; then
			DOCUMENT_SERVER_JWT_SECRET="$CURRENT_JWT_SECRET";
		fi
	fi

	if [[ -z ${JWT_SECRET} ]] && [[ "$UPDATE" != "true" ]]; then
		DOCUMENT_SERVER_JWT_SECRET=$(get_random_str 12);
	fi
}

set_core_machinekey () {
	CURRENT_CORE_MACHINEKEY="";

	if [[ -z ${CORE_MACHINEKEY} ]]; then
		if file_exists ${BASE_DIR}/.private/machinekey; then
			CURRENT_CORE_MACHINEKEY=$(cat ${BASE_DIR}/.private/machinekey);

			if [[ -n ${CURRENT_CORE_MACHINEKEY} ]]; then
				APP_CORE_MACHINEKEY="$CURRENT_CORE_MACHINEKEY";
			fi
		fi
	fi

	if [[ -z ${CORE_MACHINEKEY} ]]; then
		CURRENT_CORE_MACHINEKEY=$(get_container_env_parameter "${PACKAGE_SYSNAME}-api" "$APP_CORE_MACHINEKEY");

		if [[ -n ${CURRENT_CORE_MACHINEKEY} ]]; then
			APP_CORE_MACHINEKEY="$CURRENT_CORE_MACHINEKEY";
		fi
	fi

	if [[ -z ${CORE_MACHINEKEY} ]] && [[ "$UPDATE" != "true" ]]; then
		APP_CORE_MACHINEKEY=$(get_random_str 12);
		mkdir -p ${BASE_DIR}/.private/
		echo $APP_CORE_MACHINEKEY > ${BASE_DIR}/.private/machinekey
	fi
}

download_files () {
	if ! command_exists svn; then
		install_service svn subversion
	fi

	svn export --force https://github.com/ONLYOFFICE/${PRODUCT}/branches/${GIT_BRANCH}/build/install/docker/ ${BASE_DIR}

	reconfigure STATUS ${STATUS}
}

reconfigure () {
	local VARIABLE_NAME=$1
	local VARIABLE_VALUE=$2

	if [[ -n ${VARIABLE_VALUE} ]]; then
		sed -i "s~${VARIABLE_NAME}=.*~${VARIABLE_NAME}=${VARIABLE_VALUE}~g" $BASE_DIR/.env
	fi
}

install_mysql_server () {
	if ! command_exists docker-compose; then
		install_docker_compose
	fi

	if [[ -z ${MYSQL_PASSWORD} ]] && [[ -z ${MYSQL_ROOT_PASSWORD} ]]; then
		MYSQL_PASSWORD=$(get_random_str 20 | sed -e 's/;/%/g' -e 's/=/%/g' -e 's/!/%/g');
		MYSQL_ROOT_PASSWORD=$(get_random_str 20 | sed -e 's/;/%/g' -e 's/=/%/g' -e 's/!/%/g');
	elif [[ -z ${MYSQL_PASSWORD} ]] || [[ -z ${MYSQL_ROOT_PASSWORD} ]]; then
		MYSQL_PASSWORD=${MYSQL_PASSWORD:-"$MYSQL_ROOT_PASSWORD"}
		MYSQL_ROOT_PASSWORD=${MYSQL_ROOT_PASSWORD:-"$MYSQL_PASSWORD"}
	fi

	reconfigure MYSQL_DATABASE ${MYSQL_DATABASE}
	reconfigure MYSQL_USER ${MYSQL_USER}
	reconfigure MYSQL_PASSWORD ${MYSQL_PASSWORD}
	reconfigure MYSQL_ROOT_PASSWORD ${MYSQL_ROOT_PASSWORD}
	reconfigure MYSQL_HOST ${MYSQL_HOST}
	reconfigure DATABASE_MIGRATION ${DATABASE_MIGRATION}

	docker-compose -f $BASE_DIR/db.yml up -d
}

install_document_server () {
	if ! command_exists docker-compose; then
		install_docker_compose
	fi

	reconfigure DOCUMENT_SERVER_IMAGE_NAME ${DOCUMENT_SERVER_IMAGE_NAME}
	reconfigure DOCUMENT_SERVER_JWT_SECRET ${DOCUMENT_SERVER_JWT_SECRET}
	reconfigure DOCUMENT_SERVER_HOST ${DOCUMENT_SERVER_HOST}

	docker-compose -f $BASE_DIR/ds.yml up -d
}

install_kafka () {
	reconfigure ZOO_PORT ${ZOO_PORT}
	reconfigure ZOO_HOST ${ZOO_HOST}
	reconfigure KAFKA_HOST ${KAFKA_HOST}

	docker-compose -f $BASE_DIR/kafka.yml up -d
}

install_appserver () {
	if ! command_exists docker-compose; then
		install_docker_compose
	fi
	reconfigure ELK_HOST ${ELK_HOST}
	reconfigure SERVICE_PORT ${SERVICE_PORT}
	reconfigure APP_CORE_MACHINEKEY ${APP_CORE_MACHINEKEY}
	reconfigure APP_CORE_BASE_DOMAIN ${APP_CORE_BASE_DOMAIN}
	reconfigure DOCKER_TAG ${DOCKER_TAG}

	if [[ -n $EXTERNAL_PORT ]]; then
		sed -i "s/8092:8092/${EXTERNAL_PORT}:8092/g" $BASE_DIR/appserver.yml
	fi

	docker-compose -f $BASE_DIR/appserver.yml up -d
	docker-compose -f $BASE_DIR/notify.yml up -d
}

get_local_image_RepoDigests() {
   local CONTAINER_IMAGE=$1;
   LOCAL_IMAGE_RepoDigest=$(docker inspect --format='{{index .RepoDigests 0}}' $CONTAINER_IMAGE)
   if [ -z ${LOCAL_IMAGE_RepoDigest} ]; then
        echo "Local docker image not found, check the name of docker image $CONTAINER_IMAGE"
        exit 1 
   fi
   echo $LOCAL_IMAGE_RepoDigest
}

check_pull_image() {
    local CONTAINER_IMAGE=$1;
    CHECK_STATUS_IMAGE="$(docker pull  $CONTAINER_IMAGE | grep Status | awk '{print $2" "$3" "$4" "$5" "$6}')"
    if [ "${CHECK_STATUS_IMAGE}" == "Image is up to date" ]; then
        echo "No updates required"
    fi
}

check_image_RepoDigest() {
    local OLD_LOCAL_IMAGE_RepoDigest=$1
    local NEW_LOCAL_IMAGE_RepoDigest=$2
    if [ "${OLD_LOCAL_IMAGE_RepoDigest}" == "${NEW_LOCAL_IMAGE_RepoDigest}" ]; then
       CHECK_RepoDigest="false";
    else
       CHECK_RepoDigest="true";
    fi
}

docker_image_update() {
    docker-compose -f $BASE_DIR/notify.yml -f $BASE_DIR/appserver.yml down --volumes
    docker-compose -f $BASE_DIR/build.yml pull
}

update_appserver () {
	if ! command_exists docker-compose; then
		install_docker_compose
	fi
	
	IMAGE_NAME="onlyoffice-api"
	CONTAINER_IMAGE=$(docker inspect --format='{{.Config.Image}}' $IMAGE_NAME)
	
	OLD_LOCAL_IMAGE_RepoDigest=$(get_local_image_RepoDigests "${CONTAINER_IMAGE}")
	check_pull_image "${CONTAINER_IMAGE}"
	NEW_LOCAL_IMAGE_RepoDigest=$(get_local_image_RepoDigests "${CONTAINER_IMAGE}")
	check_image_RepoDigest ${OLD_LOCAL_IMAGE_RepoDigest} ${NEW_LOCAL_IMAGE_RepoDigest}

	if [ ${CHECK_RepoDigest} == "true" ]; then
		docker_image_update
	fi
}

save_parameter() {
	local VARIABLE_NAME=$1
	local VARIABLE_VALUE=$2

	if [[ -z ${VARIABLE_VALUE} ]]; then
		sed -n "/.*${VARIABLE_NAME}=/s///p" $BASE_DIR/.env
	else
		echo $VARIABLE_VALUE
	fi
}

save_parameters_from_configs() {
	MYSQL_DATABASE=$(save_parameter MYSQL_DATABASE $MYSQL_DATABASE)
	MYSQL_USER=$(save_parameter MYSQL_USER $MYSQL_USER)
	MYSQL_PASSWORD=$(save_parameter MYSQL_PASSWORD $MYSQL_PASSWORD)
	MYSQL_ROOT_PASSWORD=$(save_parameter MYSQL_ROOT_PASSWORD $MYSQL_ROOT_PASSWORD)
	MYSQL_HOST=$(save_parameter MYSQL_HOST $MYSQL_HOST)
	DOCUMENT_SERVER_JWT_SECRET=$(save_parameter DOCUMENT_SERVER_JWT_SECRET $DOCUMENT_SERVER_JWT_SECRET)
	DOCUMENT_SERVER_HOST=$(save_parameter DOCUMENT_SERVER_HOST $DOCUMENT_SERVER_HOST)
	ZOO_PORT=$(save_parameter ZOO_PORT $ZOO_PORT)
	ZOO_HOST=$(save_parameter ZOO_HOST $ZOO_HOST)
	KAFKA_HOST=$(save_parameter KAFKA_HOST $KAFKA_HOST)
	ELK_HOST=$(save_parameter ELK_HOST $ELK_HOST)
	SERVICE_PORT=$(save_parameter SERVICE_PORT $SERVICE_PORT)
	APP_CORE_MACHINEKEY=$(save_parameter APP_CORE_MACHINEKEY $APP_CORE_MACHINEKEY)
	APP_CORE_BASE_DOMAIN=$(save_parameter APP_CORE_BASE_DOMAIN $APP_CORE_BASE_DOMAIN)
	if [ ${EXTERNAL_PORT} = "8092" ]; then 
		EXTERNAL_PORT=$(grep -oP '(?<=- ).*?(?=:8092)' /app/onlyoffice/appserver.yml)
	fi
}

start_installation () {
	root_checking

	get_os_info

	check_os_info
	
	if [ "$UPDATE" != "true" ]; then
		check_ports
	fi

	if [ "$SKIP_HARDWARE_CHECK" != "true" ]; then
		check_hardware
	fi

	if command_exists docker ; then
		check_docker_version
		service docker start
	else
		install_docker
	fi

	docker_login

	if [ "$UPDATE" = "true" ]; then
		save_parameters_from_configs
	fi

	download_files

	set_jwt_secret

	set_core_machinekey

	create_network

	if [ "$UPDATE" = "true" ]; then
		update_appserver
	fi

	if [ "$INSTALL_MYSQL_SERVER" == "true" ]; then
		install_mysql_server
	fi
	
	if [ "$INSTALL_DOCUMENT_SERVER" == "true" ]; then
		install_document_server
	fi

	if [ "$INSTALL_KAFKA" == "true" ]; then
		install_kafka
	fi

	if [ "$INSTALL_APPSERVER" == "true" ]; then
		install_appserver
	fi

	echo ""
	echo "Thank you for installing ONLYOFFICE ${PRODUCT^^}."
	echo "In case you have any questions contact us via http://support.onlyoffice.com or visit our forum at http://dev.onlyoffice.org"
	echo ""

	exit 0;
}

start_installation