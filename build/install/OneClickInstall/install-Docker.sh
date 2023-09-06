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
PRODUCT_NAME="DocSpace"
PRODUCT=$(tr '[:upper:]' '[:lower:]' <<< ${PRODUCT_NAME})
BASE_DIR="/app/$PACKAGE_SYSNAME";
PROXY_YML="${BASE_DIR}/proxy.yml"
STATUS=""
DOCKER_TAG=""
GIT_BRANCH="master"
INSTALLATION_TYPE="ENTERPRISE"
IMAGE_NAME="${PACKAGE_SYSNAME}/${PRODUCT}-api"
CONTAINER_NAME="${PACKAGE_SYSNAME}-api"

NETWORK_NAME=${PACKAGE_SYSNAME}

SWAPFILE="/${PRODUCT}_swapfile";
MAKESWAP="true";

DISK_REQUIREMENTS=40960;
MEMORY_REQUIREMENTS=8192;
CORE_REQUIREMENTS=4;

DIST="";
REV="";
KERNEL="";

INSTALL_REDIS="true";
INSTALL_RABBITMQ="true";
INSTALL_MYSQL_SERVER="true";
INSTALL_DOCUMENT_SERVER="true";
INSTALL_ELASTICSEARCH="true";
INSTALL_PRODUCT="true";
UPDATE="false";

HUB="";
USERNAME="";
PASSWORD="";

MYSQL_VERSION=""
MYSQL_DATABASE=""
MYSQL_USER=""
MYSQL_PASSWORD=""
MYSQL_ROOT_PASSWORD=""
MYSQL_HOST=""
MYSQL_PORT=""
DATABASE_MIGRATION="true"

ELK_VERSION=""
ELK_SHEME=""
ELK_HOST=""
ELK_PORT=""

REDIS_HOST=""
REDIS_PORT=""
REDIS_USER_NAME=""
REDIS_PASSWORD=""

RABBIT_HOST=""
RABBIT_PORT=""
RABBIT_USER_NAME=""
RABBIT_PASSWORD=""

DOCUMENT_SERVER_IMAGE_NAME=""
DOCUMENT_SERVER_VERSION=""
DOCUMENT_SERVER_JWT_SECRET=""
DOCUMENT_SERVER_JWT_HEADER=""
DOCUMENT_SERVER_URL_EXTERNAL=""

APP_CORE_BASE_DOMAIN=""
APP_CORE_MACHINEKEY=""
ENV_EXTENSION=""
LETS_ENCRYPT_DOMAIN=""
LETS_ENCRYPT_MAIL=""

HELP_TARGET="install-Docker.sh";

SKIP_HARDWARE_CHECK="false";

EXTERNAL_PORT="80"

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

		-ids | --installdocspace )
			if [ "$2" != "" ]; then
				INSTALL_PRODUCT=$2
				shift
			fi
		;;

		-idocs | --installdocs )
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
		
		-irbt | --installrabbitmq )
			if [ "$2" != "" ]; then
				INSTALL_RABBITMQ=$2
				shift
			fi
		;;

		-irds | --installredis )
			if [ "$2" != "" ]; then
				INSTALL_REDIS=$2
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

		-mysqlport | --mysqlport )
			if [ "$2" != "" ]; then
				MYSQL_PORT=$2
				shift
			fi
		;;

		-mysqlp | --mysqlpassword )
			if [ "$2" != "" ]; then
				MYSQL_PASSWORD=$2
				shift
			fi
		;;

		-espr | --elasticprotocol )
			if [ "$2" != "" ]; then
				ELK_SHEME=$2
				shift
			fi
		;;

		-esh | --elastichost )
			if [ "$2" != "" ]; then
				ELK_HOST=$2
				shift
			fi
		;;

		-esp | --elasticport )
			if [ "$2" != "" ]; then
				ELK_PORT=$2
				shift
			fi
		;;

		-skiphc | --skiphardwarecheck )
			if [ "$2" != "" ]; then
				SKIP_HARDWARE_CHECK=$2
				shift
			fi
		;;

		-ep | --externalport )
			if [ "$2" != "" ]; then
				EXTERNAL_PORT=$2
				shift
			fi
		;;

		-dsh | --docspacehost )
			if [ "$2" != "" ]; then
				APP_URL_PORTAL=$2
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
				ENV_EXTENSION=$2
				shift
			fi
		;;

		-s | --status )
			if [ "$2" != "" ]; then
				STATUS=$2
				IMAGE_NAME="${PACKAGE_SYSNAME}/${STATUS}${PRODUCT}-api"
				shift
			fi
		;;

		-ls | --localscripts )
			if [ "$2" != "" ]; then
				shift
			fi
		;;
		
		-dsv | --docspaceversion )
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
		
		-docsi | --docsimage )
			if [ "$2" != "" ]; then
				DOCUMENT_SERVER_IMAGE_NAME=$2
				shift
			fi
		;;
		
		-docsv | --docsversion )
			if [ "$2" != "" ]; then
				DOCUMENT_SERVER_VERSION=$2
				shift
			fi
		;;
		
		-docsurl | --docsurl )
			if [ "$2" != "" ]; then
				DOCUMENT_SERVER_URL_EXTERNAL=$2
				shift
			fi
		;;
		
		-dbm | --databasemigration )
			if [ "$2" != "" ]; then
				DATABASE_MIGRATION=$2
				shift
			fi
		;;

		-jh | --jwtheader )
			if [ "$2" != "" ]; then
				DOCUMENT_SERVER_JWT_HEADER=$2
				shift
			fi
		;;

		-js | --jwtsecret )
			if [ "$2" != "" ]; then
				DOCUMENT_SERVER_JWT_SECRET=$2
				shift
			fi
		;;

		-it | --installation_type )
			if [ "$2" != "" ]; then
				INSTALLATION_TYPE=$(echo "$2" | awk '{print toupper($0)}');
				shift
			fi
		;;

		-ms | --makeswap )
			if [ "$2" != "" ]; then
				MAKESWAP=$2
				shift
			fi
		;;

		-ies | --installelastic )
			if [ "$2" != "" ]; then
				INSTALL_ELASTICSEARCH=$2
				shift
			fi
		;;

		-rdsh | --redishost )
			if [ "$2" != "" ]; then
				REDIS_HOST=$2
				shift
			fi
		;;

		-rdsp | --redisport )
			if [ "$2" != "" ]; then
				REDIS_PORT=$2
				shift
			fi
		;;

		-rdsu | --redisusername )
			if [ "$2" != "" ]; then
				REDIS_USER_NAME=$2
				shift
			fi
		;;

		-rdspass | --redispassword )
			if [ "$2" != "" ]; then
				REDIS_PASSWORD=$2
				shift
			fi
		;;

		-rbth | --rabbitmqhost )
			if [ "$2" != "" ]; then
				RABBIT_HOST=$2
				shift
			fi
		;;

		-rbtp | --rabbitmqport )
			if [ "$2" != "" ]; then
				RABBIT_PORT=$2
				shift
			fi
		;;

		-rbtu | --rabbitmqusername )
			if [ "$2" != "" ]; then
				RABBIT_USER_NAME=$2
				shift
			fi
		;;

		-rbtpass | --rabbitmqpassword )
			if [ "$2" != "" ]; then
				RABBIT_PASSWORD=$2
				shift
			fi
		;;

		-rbtvh | --rabbitmqvirtualhost )
			if [ "$2" != "" ]; then
				RABBIT_VIRTUAL_HOST=$2
				shift
			fi
		;;

		-led | --letsencryptdomain )
			if [ "$2" != "" ]; then
				LETS_ENCRYPT_DOMAIN=$2
				shift
			fi
		;;

		-lem | --letsencryptmail )
			if [ "$2" != "" ]; then
				LETS_ENCRYPT_MAIL=$2
				shift
			fi
		;;

		-cf | --certfile )
			if [ "$2" != "" ]; then
				CERTIFICATE_PATH=$2
				shift
			fi
		;;

		-ckf | --certkeyfile )
			if [ "$2" != "" ]; then
				CERTIFICATE_KEY_PATH=$2
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
			echo "      -it, --installation_type          installation type (community|enterprise)"
			echo "      -skiphc, --skiphardwarecheck      skip hardware check (true|false)"
			echo "      -u, --update                      use to update existing components (true|false)"
			echo "      -ids, --installdocspace           install or update $PRODUCT (true|false)"
			echo "      -dsv, --docspaceversion           select the $PRODUCT version"
			echo "      -dsh, --docspacehost              $PRODUCT host"
			echo "      -env, --environment               $PRODUCT environment"
			echo "      -mk, --machinekey                 setting for core.machinekey"
			echo "      -ep, --externalport               external $PRODUCT port (default value 80)"
			echo "      -idocs, --installdocs             install or update document server (true|false)"
			echo "      -docsi, --docsimage               document server image name"
			echo "      -docsv, --docsversion             document server version"
			echo "      -docsurl, --docsurl               $PACKAGE_SYSNAME docs server address (example http://$PACKAGE_SYSNAME-docs-address:8083)"
			echo "      -jh, --jwtheader                  defines the http header that will be used to send the JWT"
			echo "      -js, --jwtsecret                  defines the secret key to validate the JWT in the request"	
			echo "      -irbt, --installrabbitmq          install or update rabbitmq (true|false)"	
			echo "      -irds, --installredis             install or update redis (true|false)"
			echo "      -imysql, --installmysql           install or update mysql (true|false)"		
			echo "      -ies, --installelastic            install or update elasticsearch (true|false)"
			echo "      -espr, --elasticprotocol          the protocol for the connection to elasticsearch (default value http)"
			echo "      -esh, --elastichost               the IP address or hostname of the elasticsearch"
			echo "      -esp, --elasticport               elasticsearch port number (default value 6379)"
			echo "      -rdsh, --redishost                the IP address or hostname of the redis server"
			echo "      -rdsp, --redisport                redis server port number (default value 9200)"
			echo "      -rdsu, --redisusername            redis user name"
			echo "      -rdspass, --redispassword         password set for redis account"
			echo "      -rbth, --rabbitmqhost             the IP address or hostname of the rabbitmq server"
			echo "      -rbtp, --rabbitmqport             rabbitmq server port number (default value 5672)"
			echo "      -rbtu, --rabbitmqusername         username for rabbitmq server account"
			echo "      -rbtpass, --rabbitmqpassword      password set for rabbitmq server account"
			echo "      -rbtvh, --rabbitmqvirtualhost     rabbitmq virtual host (default value \"/\")"
			echo "      -mysqlrp, --mysqlrootpassword     mysql server root password"
			echo "      -mysqld, --mysqldatabase          $PRODUCT database name"
			echo "      -mysqlu, --mysqluser              $PRODUCT database user"
			echo "      -mysqlp, --mysqlpassword          $PRODUCT database password"
			echo "      -mysqlh, --mysqlhost              mysql server host"
			echo "      -mysqlport, --mysqlport           mysql server port number (default value 3306)"
			echo "      -led, --letsencryptdomain         defines the domain for Let's Encrypt certificate"
			echo "      -lem, --letsencryptmail           defines the domain administator mail address for Let's Encrypt certificate"
			echo "      -cf, --certfile                   path to the certificate file for the domain"
			echo "      -ckf, --certkeyfile               path to the private key file for the certificate"
			echo "      -dbm, --databasemigration         database migration (true|false)"
			echo "      -ms, --makeswap                   make swap file (true|false)"
			echo "      -?, -h, --help                    this help"
			echo
			echo "    Install all the components without document server:"
			echo "      bash $HELP_TARGET -idocs false"
			echo
			echo "    Install Document Server only. Skip the installation of mysql, $PRODUCT, rabbitmq, redis:"
			echo "      bash $HELP_TARGET -ids false -idocs true -imysql false -irbt false -irds false"
			echo
			echo "    Update all installed components. Stop the containers that need to be updated, remove them and run the latest versions of the corresponding components."
			echo "    The portal data should be picked up automatically:"
			echo "      bash $HELP_TARGET -u true"
			echo
			echo "    Update Document Server only to version 7.2.1.34 and skip the update for all other components:"
			echo "      bash $HELP_TARGET -u true -docsi ${PACKAGE_SYSNAME}/documentserver-ee -docsv 7.2.1.34 -idocs true -ids false -irbt false -irds false"
			echo
			echo "    Update $PRODUCT only to version 1.2.0 and skip the update for all other components:"
			echo "      bash $HELP_TARGET -u true -dsv v1.2.0 -idocs false -irbt false -irds false"
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
					REV=`cat /etc/redhat-release | grep -oP '(?<=release )\d+'`
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

	if [ -f /etc/needrestart/needrestart.conf ]; then
		sed -e "s_#\$nrconf{restart}_\$nrconf{restart}_" -e "s_\(\$nrconf{restart} =\).*_\1 'a';_" -i /etc/needrestart/needrestart.conf
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
	curl -L "https://github.com/docker/compose/releases/latest/download/docker-compose-$(uname -s)-$(uname -m)" -o /usr/bin/docker-compose
	chmod +x /usr/bin/docker-compose
}

check_ports () {
	RESERVED_PORTS=();
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
		done
	else
		echo "Invalid external port $EXTERNAL_PORT"
		exit 1;
	fi

	if [ "$INSTALL_PRODUCT" == "true" ]; then
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
	EXIST=$(docker network ls | awk '{print $2;}' | { grep -x ${NETWORK_NAME} || true; });

	if [[ -z ${EXIST} ]]; then
		docker network create --driver bridge ${NETWORK_NAME}
	fi
}

read_continue_installation () {
	read -p "Continue installation [Y/N]? " CHOICE_INSTALLATION
	case "$CHOICE_INSTALLATION" in
		y|Y )
			return 0
		;;

		n|N )
			exit 0;
		;;

		* )
			echo "Please, enter Y or N";
			read_continue_installation
		;;
	esac
}

domain_check () {
	if ! command_exists dig; then
		install_service dig dnsutils
	fi

	if ! command_exists ping; then
		if command_exists apt-get; then
			install_service ping iputils-ping
		elif command_exists yum; then
			install_service ping iputils
		fi
	fi

	if ! command_exists ip; then
		if command_exists apt-get; then
			install_service ip iproute2
		elif command_exists yum; then
			install_service ip iproute
		fi
	fi

	DOMAINS=$(dig +short -x $(curl -s ifconfig.me) | sed 's/\.$//')

	if [[ -n "$DOMAINS" ]]; then
		while IFS= read -r DOMAIN; do
			IP_ADDRESS=$(ping -c 1 -W 1 $DOMAIN | grep -oP '(\d+\.\d+\.\d+\.\d+)' | head -n 1)
			if [[ -n "$IP_ADDRESS" && "$IP_ADDRESS" =~ ^(10\.|127\.|172\.(1[6-9]|2[0-9]|3[0-1])\.|192\.168\.) ]]; then
				LOCAL_RESOLVED_DOMAINS+="$DOMAIN"
			elif [[ -n "$IP_ADDRESS" ]]; then
				APP_URL_PORTAL=${APP_URL_PORTAL:-"http://${DOMAIN}:${EXTERNAL_PORT}"}
			fi
		done <<< "$DOMAINS"
	fi
	
	if [[ -n "$LOCAL_RESOLVED_DOMAINS" ]] || [[ $(ip route get 8.8.8.8 | awk '{print $7}') != $(curl -s ifconfig.me) ]]; then
		DOCKER_DAEMON_FILE="/etc/docker/daemon.json"
		if ! grep -q '"dns"' "$DOCKER_DAEMON_FILE" 2>/dev/null; then
			echo "A problem was detected for ${LOCAL_RESOLVED_DOMAINS[@]} domains when using a loopback IP address or when using NAT."
			echo "Select 'Y' to continue installing with configuring the use of external IP in Docker via Google Public DNS."
			echo "Select 'N' to cancel ${PACKAGE_SYSNAME^^} ${PRODUCT_NAME} installation."
			if read_continue_installation; then
				if [[ -f "$DOCKER_DAEMON_FILE" ]]; then	
					sed -i '/{/a\    "dns": ["8.8.8.8", "8.8.4.4"],' "$DOCKER_DAEMON_FILE"
				else
					echo "{\"dns\": [\"8.8.8.8\", \"8.8.4.4\"]}" | tee "$DOCKER_DAEMON_FILE" >/dev/null
				fi
				systemctl restart docker
			fi
		fi
	fi
}

establish_conn() {
	echo -n "Trying to establish $3 connection... "
  
	exec {FD}<> /dev/tcp/$1/$2 && exec {FD}>&-

	if [ "$?" != 0 ]; then
		echo "FAILURE";
		exit 1;
	fi

	echo "OK"
}

get_container_env_parameter () {
	local CONTAINER_NAME=$1;
	local PARAMETER_NAME=$2;

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

		if [ -z $VALUE ] && [ -f $BASE_DIR/.env ]; then
			VALUE=$(awk -F= "/${PARAMETER_NAME}/ {print \$2}" $BASE_DIR/.env | tr -d '\r')
		fi
	fi

	echo ${VALUE//\"}
}

get_available_version () {
	if [[ -z "$1" ]]; then
		echo "image name is empty";
		exit 1;
	fi

	if ! command_exists curl ; then
		install_curl;
	fi

	CREDENTIALS="";
	AUTH_HEADER="";
	TAGS_RESP="";

	if [[ -n ${HUB} ]]; then
		DOCKER_CONFIG="$HOME/.docker/config.json";

		if [[ -f "$DOCKER_CONFIG" ]]; then
			CREDENTIALS=$(jq -r '.auths."'$HUB'".auth' < "$DOCKER_CONFIG");
			if [ "$CREDENTIALS" == "null" ]; then
				CREDENTIALS="";
			fi
		fi

		if [[ -z ${CREDENTIALS} && -n ${USERNAME} && -n ${PASSWORD} ]]; then
			CREDENTIALS=$(echo -n "$USERNAME:$PASSWORD" | base64);
		fi

		if [[ -n ${CREDENTIALS} ]]; then
			AUTH_HEADER="Authorization: Basic $CREDENTIALS";
		fi

		REPO=$(echo $1 | sed "s/$HUB\///g");
		TAGS_RESP=$(curl -s -H "$AUTH_HEADER" -X GET https://$HUB/v2/$REPO/tags/list);
		TAGS_RESP=$(echo $TAGS_RESP | jq -r '.tags')
	else
		if [[ -n ${USERNAME} && -n ${PASSWORD} ]]; then
			CREDENTIALS="{\"username\":\"$USERNAME\",\"password\":\"$PASSWORD\"}";
		fi

		if [[ -n ${CREDENTIALS} ]]; then
			LOGIN_RESP=$(curl -s -H "Content-Type: application/json" -X POST -d "$CREDENTIALS" https://hub.docker.com/v2/users/login/);
			TOKEN=$(echo $LOGIN_RESP | jq -r '.token');
			AUTH_HEADER="Authorization: JWT $TOKEN";
			sleep 1;
		fi

		TAGS_RESP=$(curl -s -H "$AUTH_HEADER" -X GET https://hub.docker.com/v2/repositories/$1/tags/);
		TAGS_RESP=$(echo $TAGS_RESP | jq -r '.results[].name')
	fi

	VERSION_REGEX="[0-9]+\.[0-9]+\.[0-9]+(\.[0-9]+)?$"
	
	TAG_LIST=""

	for item in $TAGS_RESP
	do
		if [[ $item =~ $VERSION_REGEX ]]; then
			TAG_LIST="$item,$TAG_LIST"
		fi
	done

	LATEST_TAG=$(echo $TAG_LIST | tr ',' '\n' | sort -t. -k 1,1n -k 2,2n -k 3,3n -k 4,4n | awk '/./{line=$0} END{print line}');

	echo "$LATEST_TAG" | sed "s/\"//g"
}

set_url_external () {
	DOCUMENT_SERVER_URL_EXTERNAL=${DOCUMENT_SERVER_URL_EXTERNAL:-$(get_container_env_parameter "${CONTAINER_NAME}" "DOCUMENT_SERVER_URL_EXTERNAL")};

	if [[ ! -z ${DOCUMENT_SERVER_URL_EXTERNAL} ]] && [[ $DOCUMENT_SERVER_URL_EXTERNAL =~ ^(https?://)?([^:/]+)(:([0-9]+))?(/.*)?$ ]]; then
		[[ -z ${BASH_REMATCH[1]} ]] && DOCUMENT_SERVER_URL_EXTERNAL="http://$DOCUMENT_SERVER_URL_EXTERNAL"
		DOCUMENT_SERVER_HOST="${BASH_REMATCH[2]}"
		DOCUMENT_SERVER_PORT="${BASH_REMATCH[4]:-"80"}"
	fi
}

set_jwt_secret () {
	CURRENT_JWT_SECRET="";

	if [[ -z ${DOCUMENT_SERVER_JWT_SECRET} ]]; then
		CURRENT_JWT_SECRET=$(get_container_env_parameter "${PACKAGE_SYSNAME}-document-server" "JWT_SECRET");

		if [[ -n ${CURRENT_JWT_SECRET} ]]; then
			DOCUMENT_SERVER_JWT_SECRET="$CURRENT_JWT_SECRET";
		fi
	fi

	if [[ -z ${DOCUMENT_SERVER_JWT_SECRET} ]]; then
		CURRENT_JWT_SECRET=$(get_container_env_parameter "${CONTAINER_NAME}" "DOCUMENT_SERVER_JWT_SECRET");

		if [[ -n ${CURRENT_JWT_SECRET} ]]; then
			DOCUMENT_SERVER_JWT_SECRET="$CURRENT_JWT_SECRET";
		fi
	fi

	if [[ -z ${DOCUMENT_SERVER_JWT_SECRET} ]] && [ -f $BASE_DIR/.env ]; then
		DOCUMENT_SERVER_JWT_SECRET=$(sed -n "/.*DOCUMENT_SERVER_JWT_SECRET=/s///p" $BASE_DIR/.env)
	fi

	if [[ -z ${DOCUMENT_SERVER_JWT_SECRET} ]]; then
		DOCUMENT_SERVER_JWT_SECRET=$(get_random_str 32);
	fi
}

set_jwt_header () {
	CURRENT_JWT_HEADER="";

	if [[ -z ${DOCUMENT_SERVER_JWT_HEADER} ]]; then
		CURRENT_JWT_HEADER=$(get_container_env_parameter  "${PACKAGE_SYSNAME}-document-server" "JWT_HEADER");

		if [[ -n ${CURRENT_JWT_HEADER} ]]; then
			DOCUMENT_SERVER_JWT_HEADER="$CURRENT_JWT_HEADER";
		fi
	fi	
	
	if [[ -z ${DOCUMENT_SERVER_JWT_HEADER} ]]; then
		CURRENT_JWT_HEADER=$(get_container_env_parameter "${CONTAINER_NAME}" "DOCUMENT_SERVER_JWT_HEADER");

		if [[ -n ${CURRENT_JWT_HEADER} ]]; then
			DOCUMENT_SERVER_JWT_HEADER="$CURRENT_JWT_HEADER";
		fi
	fi

	if [[ -z ${DOCUMENT_SERVER_JWT_HEADER} ]] && [ -f $BASE_DIR/.env ]; then
		DOCUMENT_SERVER_JWT_HEADER=$(sed -n "/.*DOCUMENT_SERVER_JWT_HEADER=/s///p" $BASE_DIR/.env)
	fi

	if [[ -z ${DOCUMENT_SERVER_JWT_HEADER} ]]; then
		DOCUMENT_SERVER_JWT_HEADER="AuthorizationJwt"
	fi
}

set_core_machinekey () {
	if [[ -z ${APP_CORE_MACHINEKEY} ]]; then
		CURRENT_CORE_MACHINEKEY=$(get_container_env_parameter "${CONTAINER_NAME}" "APP_CORE_MACHINEKEY");

		if [[ -n ${CURRENT_CORE_MACHINEKEY} ]]; then
			APP_CORE_MACHINEKEY="$CURRENT_CORE_MACHINEKEY";
		fi
	fi

	if [[ -z ${APP_CORE_MACHINEKEY} ]] && [[ "$UPDATE" != "true" ]]; then
		APP_CORE_MACHINEKEY=$(get_random_str 12);
	fi
}

set_mysql_params () {
	if [[ -z ${MYSQL_PASSWORD} ]]; then
		MYSQL_PASSWORD=$(get_container_env_parameter "${CONTAINER_NAME}" "MYSQL_PASSWORD");

		if [[ -z ${MYSQL_PASSWORD} ]]; then
			MYSQL_PASSWORD=$(get_random_str 20);
		fi
	fi
	
	if [[ -z ${MYSQL_ROOT_PASSWORD} ]]; then
		MYSQL_ROOT_PASSWORD=$(get_container_env_parameter "${CONTAINER_NAME}" "MYSQL_ROOT_PASSWORD");

		if [[ -z ${MYSQL_ROOT_PASSWORD} ]]; then
			MYSQL_ROOT_PASSWORD=${MYSQL_PASSWORD:-$(get_random_str 20)};
		fi
	fi
	
	if [[ -z ${MYSQL_DATABASE} ]]; then
		MYSQL_DATABASE=$(get_container_env_parameter "${CONTAINER_NAME}" "MYSQL_DATABASE");
	fi

	if [[ -z ${MYSQL_USER} ]]; then
		MYSQL_USER=$(get_container_env_parameter "${CONTAINER_NAME}" "MYSQL_USER");
	fi

	if [[ -z ${MYSQL_HOST} ]]; then
		MYSQL_HOST=$(get_container_env_parameter "${CONTAINER_NAME}" "MYSQL_HOST");
	fi

	if [[ -z ${MYSQL_PORT} ]]; then
		MYSQL_PORT=$(get_container_env_parameter "${CONTAINER_NAME}" "MYSQL_PORT");
	fi
}

set_docspace_params() {
	ENV_EXTENSION=${ENV_EXTENSION:-$(get_container_env_parameter "${CONTAINER_NAME}" "ENV_EXTENSION")};
	APP_CORE_BASE_DOMAIN=${APP_CORE_BASE_DOMAIN:-$(get_container_env_parameter "${CONTAINER_NAME}" "APP_CORE_BASE_DOMAIN")};
	APP_URL_PORTAL=${APP_URL_PORTAL:-$(get_container_env_parameter "${CONTAINER_NAME}" "APP_URL_PORTAL")};
	EXTERNAL_PORT=${EXTERNAL_PORT:-$(get_container_env_parameter "${CONTAINER_NAME}" "EXTERNAL_PORT")};

	ELK_SHEME=${ELK_SHEME:-$(get_container_env_parameter "${CONTAINER_NAME}" "ELK_SHEME")};
	ELK_HOST=${ELK_HOST:-$(get_container_env_parameter "${CONTAINER_NAME}" "ELK_HOST")};
	ELK_PORT=${ELK_PORT:-$(get_container_env_parameter "${CONTAINER_NAME}" "ELK_PORT")};

	REDIS_HOST=${REDIS_HOST:-$(get_container_env_parameter "${CONTAINER_NAME}" "REDIS_HOST")};
	REDIS_PORT=${REDIS_PORT:-$(get_container_env_parameter "${CONTAINER_NAME}" "REDIS_PORT")};
	REDIS_USER_NAME=${REDIS_USER_NAME:-$(get_container_env_parameter "${CONTAINER_NAME}" "REDIS_USER_NAME")};
	REDIS_PASSWORD=${REDIS_PASSWORD:-$(get_container_env_parameter "${CONTAINER_NAME}" "REDIS_PASSWORD")};

	RABBIT_HOST=${RABBIT_HOST:-$(get_container_env_parameter "${CONTAINER_NAME}" "RABBIT_HOST")};
	RABBIT_PORT=${RABBIT_PORT:-$(get_container_env_parameter "${CONTAINER_NAME}" "RABBIT_PORT")};
	RABBIT_USER_NAME=${RABBIT_USER_NAME:-$(get_container_env_parameter "${CONTAINER_NAME}" "RABBIT_USER_NAME")};
	RABBIT_PASSWORD=${RABBIT_PASSWORD:-$(get_container_env_parameter "${CONTAINER_NAME}" "RABBIT_PASSWORD")};
	RABBIT_VIRTUAL_HOST=${RABBIT_VIRTUAL_HOST:-$(get_container_env_parameter "${CONTAINER_NAME}" "RABBIT_VIRTUAL_HOST")};
	
	CERTIFICATE_PATH=${CERTIFICATE_PATH:-$(get_container_env_parameter "${CONTAINER_NAME}" "CERTIFICATE_PATH")};
	CERTIFICATE_KEY_PATH=${CERTIFICATE_KEY_PATH:-$(get_container_env_parameter "${CONTAINER_NAME}" "CERTIFICATE_KEY_PATH")};
	DHPARAM_PATH=${DHPARAM_PATH:-$(get_container_env_parameter "${CONTAINER_NAME}" "DHPARAM_PATH")};
}

set_installation_type_data () {
	if [ "$INSTALLATION_TYPE" == "COMMUNITY" ]; then
		DOCUMENT_SERVER_IMAGE_NAME=${DOCUMENT_SERVER_IMAGE_NAME:-"${PACKAGE_SYSNAME}/${STATUS}documentserver"}
	elif [ "$INSTALLATION_TYPE" == "ENTERPRISE" ]; then
		DOCUMENT_SERVER_IMAGE_NAME=${DOCUMENT_SERVER_IMAGE_NAME:-"${PACKAGE_SYSNAME}/${STATUS}documentserver-ee"}
	fi
}

download_files () {
	if ! command_exists svn; then
		install_service svn subversion
	fi

	if ! command_exists jq ; then
		if command_exists yum; then 
			rpm -ivh https://dl.fedoraproject.org/pub/epel/epel-release-latest-$REV.noarch.rpm
		fi
		install_service jq
	fi

	if ! command_exists docker-compose; then
		install_docker_compose
	fi

	HOSTS=("ELK_HOST" "REDIS_HOST" "RABBIT_HOST" "MYSQL_HOST"); 
	for HOST in "${HOSTS[@]}"; do [[ "${!HOST}" == *CONTAINER_PREFIX* || "${!HOST}" == *$PACKAGE_SYSNAME* ]] && export "$HOST="; done

	svn export --force https://github.com/${PACKAGE_SYSNAME}/${PRODUCT}/branches/${GIT_BRANCH}/build/install/docker/ ${BASE_DIR}

	reconfigure STATUS ${STATUS}
	reconfigure INSTALLATION_TYPE ${INSTALLATION_TYPE}
	reconfigure NETWORK_NAME ${NETWORK_NAME}
}

reconfigure () {
	local VARIABLE_NAME="$1"
	local VARIABLE_VALUE="$2"

	if [[ -n ${VARIABLE_VALUE} ]]; then
		sed -i "s~${VARIABLE_NAME}=.*~${VARIABLE_NAME}=${VARIABLE_VALUE}~g" $BASE_DIR/.env
	fi
}

install_mysql_server () {
	reconfigure DATABASE_MIGRATION ${DATABASE_MIGRATION}	
	reconfigure MYSQL_DATABASE ${MYSQL_DATABASE}
	reconfigure MYSQL_USER ${MYSQL_USER}
	reconfigure MYSQL_PASSWORD ${MYSQL_PASSWORD}
	reconfigure MYSQL_ROOT_PASSWORD ${MYSQL_ROOT_PASSWORD}

	if [[ -z ${MYSQL_HOST} ]] && [ "$INSTALL_MYSQL_SERVER" == "true" ]; then	
		reconfigure MYSQL_VERSION ${MYSQL_VERSION}
		docker-compose -f $BASE_DIR/db.yml up -d
	elif [ ! -z "$MYSQL_HOST" ]; then
		establish_conn ${MYSQL_HOST} "${MYSQL_PORT:-"3306"}" "MySQL"
		reconfigure MYSQL_HOST ${MYSQL_HOST}
		reconfigure MYSQL_PORT "${MYSQL_PORT:-"3306"}"
	fi
}

install_document_server () {
	reconfigure DOCUMENT_SERVER_JWT_HEADER ${DOCUMENT_SERVER_JWT_HEADER}
	reconfigure DOCUMENT_SERVER_JWT_SECRET ${DOCUMENT_SERVER_JWT_SECRET}
	if [[ -z ${DOCUMENT_SERVER_HOST} ]] && [ "$INSTALL_DOCUMENT_SERVER" == "true" ]; then
		reconfigure DOCUMENT_SERVER_IMAGE_NAME "${DOCUMENT_SERVER_IMAGE_NAME}:${DOCUMENT_SERVER_VERSION:-$(get_available_version "$DOCUMENT_SERVER_IMAGE_NAME")}"
		docker-compose -f $BASE_DIR/ds.yml up -d
	elif [ ! -z "$DOCUMENT_SERVER_HOST" ]; then
		APP_URL_PORTAL=${APP_URL_PORTAL:-"http://$(curl -s ifconfig.me):${EXTERNAL_PORT}"}  
		establish_conn ${DOCUMENT_SERVER_HOST} ${DOCUMENT_SERVER_PORT} "${PACKAGE_SYSNAME^^} Docs"
		reconfigure DOCUMENT_SERVER_URL_EXTERNAL ${DOCUMENT_SERVER_URL_EXTERNAL}
		reconfigure DOCUMENT_SERVER_URL_PUBLIC ${DOCUMENT_SERVER_URL_EXTERNAL}
	fi
}

install_rabbitmq () {
	if [[ -z ${RABBIT_HOST} ]] && [ "$INSTALL_RABBITMQ" == "true" ]; then
		docker-compose -f $BASE_DIR/rabbitmq.yml up -d
	elif [ ! -z "$RABBIT_HOST" ]; then
		establish_conn ${RABBIT_HOST} "${RABBIT_PORT:-"5672"}" "RabbitMQ"
		reconfigure RABBIT_HOST ${RABBIT_HOST}
		reconfigure RABBIT_PORT "${RABBIT_PORT:-"5672"}"
		reconfigure RABBIT_USER_NAME ${RABBIT_USER_NAME}
		reconfigure RABBIT_PASSWORD ${RABBIT_PASSWORD}
		reconfigure RABBIT_VIRTUAL_HOST "${RABBIT_VIRTUAL_HOST:-"/"}"
	fi
}

install_redis () {
	if [[ -z ${REDIS_HOST} ]] && [ "$INSTALL_REDIS" == "true" ]; then
		docker-compose -f $BASE_DIR/redis.yml up -d
	elif [ ! -z "$REDIS_HOST" ]; then
		establish_conn ${REDIS_HOST} "${REDIS_PORT:-"6379"}" "Redis"
		reconfigure REDIS_HOST ${REDIS_HOST}
		reconfigure REDIS_PORT "${REDIS_PORT:-"6379"}"
		reconfigure REDIS_USER_NAME ${REDIS_USER_NAME}
		reconfigure REDIS_PASSWORD ${REDIS_PASSWORD}
	fi
}

install_elasticsearch () {
	if [[ -z ${ELK_HOST} ]] && [ "$INSTALL_ELASTICSEARCH" == "true" ]; then
		if [ $(free -m | grep -oP '\d+' | head -n 1) -gt "12228" ]; then #RAM ~12Gb
			sed -i 's/Xms[0-9]g/Xms4g/g; s/Xmx[0-9]g/Xmx4g/g' $BASE_DIR/elasticsearch.yml
		else
			sed -i 's/Xms[0-9]g/Xms1g/g; s/Xmx[0-9]g/Xmx1g/g' $BASE_DIR/elasticsearch.yml
		fi
		reconfigure ELK_VERSION ${ELK_VERSION}
		docker-compose -f $BASE_DIR/elasticsearch.yml up -d
	elif [ ! -z "$ELK_HOST" ]; then
		establish_conn ${ELK_HOST} "${ELK_PORT:-"9200"}" "Elasticsearch"
		reconfigure ELK_SHEME "${ELK_SHEME:-"http"}"	
		reconfigure ELK_HOST ${ELK_HOST}
		reconfigure ELK_PORT "${ELK_PORT:-"9200"}"	
	fi
}

install_product () {
	DOCKER_TAG="${DOCKER_TAG:-$(get_available_version ${IMAGE_NAME})}"
	reconfigure DOCKER_TAG ${DOCKER_TAG}

	[ "${UPDATE}" = "true" ] && LOCAL_CONTAINER_TAG="$(docker inspect --format='{{index .Config.Image}}' ${CONTAINER_NAME} | awk -F':' '{print $2}')"

	if [ "${UPDATE}" = "true" ] && [ "${LOCAL_CONTAINER_TAG}" != "${DOCKER_TAG}" ]; then
		docker-compose -f $BASE_DIR/build.yml pull
		docker-compose -f $BASE_DIR/migration-runner.yml -f $BASE_DIR/notify.yml -f $BASE_DIR/healthchecks.yml -f ${PROXY_YML} down
		docker-compose -f $BASE_DIR/${PRODUCT}.yml down --volumes
	fi

	reconfigure ENV_EXTENSION ${ENV_EXTENSION}
	reconfigure APP_CORE_MACHINEKEY ${APP_CORE_MACHINEKEY}
	reconfigure APP_CORE_BASE_DOMAIN ${APP_CORE_BASE_DOMAIN}
	reconfigure APP_URL_PORTAL "${APP_URL_PORTAL:-"http://${PACKAGE_SYSNAME}-router:8092"}"
	reconfigure EXTERNAL_PORT ${EXTERNAL_PORT}

	if [ ! -z "${CERTIFICATE_PATH}" ] && [ ! -z "${CERTIFICATE_KEY_PATH}" ]; then
		bash $BASE_DIR/config/${PRODUCT}-ssl-setup -f "${CERTIFICATE_PATH}" "${CERTIFICATE_KEY_PATH}"
		PROXY_YML="${BASE_DIR}/proxy-ssl.yml"
	elif [ ! -z "${LETS_ENCRYPT_DOMAIN}" ] && [ ! -z "${LETS_ENCRYPT_MAIL}" ]; then
		bash $BASE_DIR/config/${PRODUCT}-ssl-setup "${LETS_ENCRYPT_MAIL}" "${LETS_ENCRYPT_DOMAIN}"
		PROXY_YML="${BASE_DIR}/proxy-ssl.yml"
	fi

	docker-compose -f $BASE_DIR/migration-runner.yml up -d
	docker-compose -f $BASE_DIR/${PRODUCT}.yml up -d
	docker-compose -f ${PROXY_YML} up -d
	docker-compose -f $BASE_DIR/notify.yml up -d
	docker-compose -f $BASE_DIR/healthchecks.yml up -d
}

make_swap () {
	DISK_REQUIREMENTS=6144; #6Gb free space
	MEMORY_REQUIREMENTS=11000; #RAM ~12Gb

	AVAILABLE_DISK_SPACE=$(df -m /  | tail -1 | awk '{ print $4 }');
	TOTAL_MEMORY=$(free -m | grep -oP '\d+' | head -n 1);
	EXIST=$(swapon -s | awk '{ print $1 }' | { grep -x ${SWAPFILE} || true; });

	if [[ -z $EXIST ]] && [ ${TOTAL_MEMORY} -lt ${MEMORY_REQUIREMENTS} ] && [ ${AVAILABLE_DISK_SPACE} -gt ${DISK_REQUIREMENTS} ]; then

		if [ "${DIST}" == "Ubuntu" ] || [ "${DIST}" == "Debian" ]; then
			fallocate -l 6G ${SWAPFILE}
		else
			dd if=/dev/zero of=${SWAPFILE} count=6144 bs=1MiB
		fi

		chmod 600 ${SWAPFILE}
		mkswap ${SWAPFILE}
		swapon ${SWAPFILE}
		echo "$SWAPFILE none swap sw 0 0" >> /etc/fstab
	fi
}


start_installation () {
	root_checking

	set_installation_type_data

	get_os_info
	check_os_info
	check_kernel

	if [ "$UPDATE" != "true" ]; then
		check_ports
	fi

	if [ "$SKIP_HARDWARE_CHECK" != "true" ]; then
		check_hardware
	fi

	if [ "$MAKESWAP" == "true" ]; then
		make_swap
	fi

	if command_exists docker ; then
		check_docker_version
		service docker start
	else
		install_docker
	fi

	docker_login

	create_network

	domain_check

	if [ "$UPDATE" = "true" ]; then
		set_docspace_params
	fi

	set_url_external
	set_jwt_secret
	set_jwt_header

	set_core_machinekey

	set_mysql_params

	download_files

	install_mysql_server
	
	install_document_server

	install_rabbitmq

	install_redis

	install_elasticsearch

	if [ "$INSTALL_PRODUCT" == "true" ]; then
		install_product
	fi

	echo ""
	echo "Thank you for installing ${PACKAGE_SYSNAME^^} ${PRODUCT_NAME}."
	echo "In case you have any questions contact us via http://support.${PACKAGE_SYSNAME}.com or visit our forum at http://forum.${PACKAGE_SYSNAME}.com"
	echo ""

	exit 0;
}

start_installation