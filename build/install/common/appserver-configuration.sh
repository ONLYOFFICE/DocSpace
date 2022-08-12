#!/bin/bash

set -e

PRODUCT="appserver"
ENVIRONMENT="production"

APP_DIR="/etc/onlyoffice/${PRODUCT}"
USER_CONF="$APP_DIR/appsettings.$ENVIRONMENT.json"
NGINX_DIR="/etc/nginx"
NGINX_CONF="${NGINX_DIR}/conf.d"
SYSTEMD_DIR="/lib/systemd/system"

MYSQL=""
DB_HOST=""
DB_PORT="3306"
DB_NAME=""
DB_USER=""
DB_PWD=""

APP_HOST="localhost"
APP_PORT="80"

DOCUMENT_SERVER_HOST="localhost";
DOCUMENT_SERVER_PORT="8083";

ELK_SHEME="http"
ELK_HOST="localhost"
ELK_PORT="9200"

JSON="json -I -f"
JSON_USERCONF="$JSON $USER_CONF -e"

[ $(id -u) -ne 0 ] && { echo "Root privileges required"; exit 1; }

while [ "$1" != "" ]; do
	case $1 in

		-ash | --appshost )
			if [ "$2" != "" ]; then
				APP_HOST=$2
				shift
			fi
		;;

		-asp | --appsport )
			if [ "$2" != "" ]; then
				APP_PORT=$2
				shift
			fi
		;;

		-dsh | --docshost )
			if [ "$2" != "" ]; then
				DOCUMENT_SERVER_HOST=$2
				shift
			fi
		;;

		-dsp | --docsport )
			if [ "$2" != "" ]; then
				DOCUMENT_SERVER_PORT=$2
				shift
			fi
		;;

		-ess | --elasticsheme )
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

		-e | --environment )
			if [ "$2" != "" ]; then
				ENVIRONMENT=$2
				shift
			fi
		;;

		-mysqlh | --mysqlhost )
			if [ "$2" != "" ]; then
				DB_HOST=$2
				shift
			fi
		;;

		-mysqld | --mysqldatabase )
			if [ "$2" != "" ]; then
				DB_NAME=$2
				shift
			fi
		;;

		-mysqlu | --mysqluser )
			if [ "$2" != "" ]; then
				DB_USER=$2
				shift
			fi
		;;

		-mysqlp | --mysqlpassword )
			if [ "$2" != "" ]; then
				DB_PWD=$2
				shift
			fi
		;;
		
		-? | -h | --help )
			echo "  Usage: bash ${PRODUCT}-configuration.sh [PARAMETER] [[PARAMETER], ...]"
			echo
			echo "    Parameters:"
			echo "      -ash, --appshost                    ${PRODUCT} ip"
			echo "      -asp, --appsport                    ${PRODUCT} port (default 80)"
			echo "      -dsh, --docshost                    document server ip"
			echo "      -dsp, --docsport                    document server port (default 8083)"
			echo "      -esh, --elastichost                 elasticsearch ip"
			echo "      -esp, --elasticport                 elasticsearch port (default 9200)"
			echo "      -mysqlh, --mysqlhost                mysql server host"
			echo "      -mysqld, --mysqldatabase            ${PRODUCT} database name"
			echo "      -mysqlu, --mysqluser                ${PRODUCT} database user"
			echo "      -mysqlp, --mysqlpassword            ${PRODUCT} database password"
			echo "      -e, --environment                   environment (default 'production')"
			echo "      -?, -h, --help                      this help"
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

set_core_machinekey () {
	if [ -f $APP_DIR/.private/machinekey ]; then
		CORE_MACHINEKEY=$(cat $APP_DIR/.private/machinekey)
	else
		CORE_MACHINEKEY=$(cat /dev/urandom | tr -dc A-Za-z0-9 | head -c 12);
		echo $CORE_MACHINEKEY >> $APP_DIR/.private/machinekey
	fi
}

install_json() {

	if [ ! -e /usr/bin/json ]; then
		echo -n "Install json package... "
		npm i json -g >/dev/null 2>&1
		echo "OK"
	fi

	#Creating a user-defined .json
	if [ ! -e $USER_CONF ]; then
		echo "{}" >> $USER_CONF
		chown onlyoffice:onlyoffice $USER_CONF
	
		set_core_machinekey
		$JSON_USERCONF "this.core={'base-domain': \"$APP_HOST\", 'machinekey': \"$CORE_MACHINEKEY\", \
		'products': { 'folder': '/var/www/appserver/products', 'subfolder': 'server'} }" \
		-e "this.urlshortener={ 'path': '../ASC.UrlShortener/index.js' }" -e "this.thumb={ 'path': '../ASC.Thumbnails/' }" \
		-e "this.socket={ 'path': '../ASC.Socket.IO/' }" -e "this.ssoauth={ 'path': '../ASC.SsoAuth/' }" >/dev/null 2>&1
	fi
}

restart_services() {
	echo -n "Restarting services... "

	sed -i "s/ENVIRONMENT=.*/ENVIRONMENT=$ENVIRONMENT/" $SYSTEMD_DIR/${PRODUCT}*.service >/dev/null 2>&1
	systemctl daemon-reload

	for SVC in api urlshortener socket studio-notify notify people-server files \
	files-services studio backup telegram-service webhooks-service \
	clear-events backup-background migration ssoauth
	do
		systemctl enable ${PRODUCT}-$SVC >/dev/null 2>&1
		systemctl restart ${PRODUCT}-$SVC
	done
	echo "OK"
}

input_db_params(){
    local user_connectionString=$(json -f $USER_CONF ConnectionStrings.default.connectionString)
    local def_DB_HOST=$(echo $user_connectionString | grep -oP 'Server=\K.*' | grep -o '^[^;]*')
    local def_DB_NAME=$(echo $user_connectionString | grep -oP 'Database=\K.*' | grep -o '^[^;]*')
    local def_DB_USER=$(echo $user_connectionString | grep -oP 'User ID=\K.*' | grep -o '^[^;]*')

	if [ -z $def_DB_HOST ] && [ -z $DB_HOST ]; then 
		read -e -p "Database host: " -i "$DB_HOST" DB_HOST;
	else
		DB_HOST=${DB_HOST:-$def_DB_HOST}
	fi

	if [ -z $def_DB_NAME ] && [ -z $DB_NAME ]; then 
		read -e -p "Database name: " -i "$DB_NAME" DB_NAME; 
	else
		DB_NAME=${DB_NAME:-$def_DB_NAME}
	fi

	if [ -z $def_DB_USER ] && [ -z $DB_USER ]; then 
		read -e -p "Database user: " -i "$DB_USER" DB_USER; 
	else
		DB_USER=${DB_USER:-$def_DB_USER}
	fi

	if [ -z $DB_PWD ]; then 
		read -e -p "Database password: " -i "$DB_PWD" -s DB_PWD; 
	fi
}

establish_mysql_conn(){
	echo -n "Trying to establish MySQL connection... "

	command -v mysql >/dev/null 2>&1 || { echo "MySQL client not found"; exit 1; }

	MYSQL="mysql -h$DB_HOST -u$DB_USER"
	if [ -n "$DB_PWD" ]; then
		MYSQL="$MYSQL -p$DB_PWD"
	fi

	$MYSQL -e ";" >/dev/null 2>&1
	ERRCODE=$?
	if [ $ERRCODE -ne 0 ]; then
		systemctl ${MYSQL_PACKAGE} start >/dev/null 2>&1
		$MYSQL -e ";" >/dev/null 2>&1 || { echo "FAILURE"; exit 1; }
	fi

    #Save db settings in .json
	$JSON_USERCONF "this.ConnectionStrings={'default': {'connectionString': \
	\"Server=$DB_HOST;Port=$DB_PORT;Database=$DB_NAME;User ID=$DB_USER;Password=$DB_PWD;Pooling=true;Character Set=utf8;AutoEnlist=false;SSL Mode=none;AllowPublicKeyRetrieval=true;Connection Timeout=30;Maximum Pool Size=300\"}}" >/dev/null 2>&1

	change_mysql_config

	#Enable database migration
	$JSON_USERCONF "this.migration={'enabled': \"true\"}" >/dev/null 2>&1

	echo "OK"
}

change_mysql_config(){
	if [ "$DIST" = "RedHat" ]; then
	
		local CNF_PATH="/etc/my.cnf";
		local CNF_SERVICE_PATH="/usr/lib/systemd/system/mysqld.service";

		if ! grep -q "\[mysqld\]" ${CNF_PATH}; then
			CNF_PATH="/etc/my.cnf.d/server.cnf";

			if ! grep -q "\[mysqld\]" ${CNF_PATH}; then
				exit 1;
			fi
		fi 

		if ! grep -q "\[Unit\]" ${CNF_SERVICE_PATH}; then
			CNF_SERVICE_PATH="/lib/systemd/system/mysqld.service";

			if ! grep -q "\[Unit\]" ${CNF_SERVICE_PATH}; then
				CNF_SERVICE_PATH="/lib/systemd/system/mariadb.service";
					
				if ! grep -q "\[Unit\]" ${CNF_SERVICE_PATH}; then 
					exit 1;
				fi
			fi
		fi 

	elif [ "$DIST" = "Debian" ]; then

		sed "s/#max_connections.*/max_connections = 1000/" -i /etc/mysql/my.cnf || true # ignore errors

		CNF_PATH="/etc/mysql/mysql.conf.d/mysqld.cnf";
		CNF_SERVICE_PATH="/lib/systemd/system/mysql.service";

		if mysql -V | grep -q "MariaDB"; then
			CNF_PATH="/etc/mysql/mariadb.conf.d/50-server.cnf";
			CNF_SERVICE_PATH="/lib/systemd/system/mariadb.service";
		fi

	fi

    sed '/skip-networking/d' -i ${CNF_PATH} || true # ignore errors

    if ! grep -q "^sql_mode" ${CNF_PATH}; then
		sed "/\[mysqld\]/a sql_mode = 'NO_ENGINE_SUBSTITUTION'" -i ${CNF_PATH} # disable new STRICT mode in mysql 5.7
	else
		sed "s/sql_mode.*/sql_mode = 'NO_ENGINE_SUBSTITUTION'/" -i ${CNF_PATH} || true # ignore errors
	fi

	if ! grep -q "^max_connections"  ${CNF_PATH}; then
		sed '/\[mysqld\]/a max_connections = 1000' -i ${CNF_PATH}
	else
		sed "s/max_connections.*/max_connections = 1000/" -i ${CNF_PATH} || true # ignore errors
	fi

	if ! grep -q "^group_concat_max_len"  ${CNF_PATH}; then
		sed '/\[mysqld\]/a group_concat_max_len = 2048' -i ${CNF_PATH}
	else
		sed "s/group_concat_max_len.*/group_concat_max_len = 2048/" -i ${CNF_PATH} || true # ignore errors
	fi

	if ! grep -q "^max_allowed_packet"  ${CNF_PATH}; then
		sed '/\[mysqld\]/a max_allowed_packet = 1048576000' -i ${CNF_PATH}
	else
		sed "s/max_allowed_packet.*/max_allowed_packet = 1048576000/" -i ${CNF_PATH} || true # ignore errors
	fi

	if ! grep -q "^character_set_server"  ${CNF_PATH}; then
		sed '/\[mysqld\]/a character_set_server = utf8' -i ${CNF_PATH}
	else
		sed "s/character_set_server.*/character_set_server = utf8/" -i ${CNF_PATH} || true # ignore errors
	fi
	
	if ! grep -q "^collation_server"  ${CNF_PATH}; then
		sed '/\[mysqld\]/a collation_server = utf8_general_ci' -i ${CNF_PATH}
	else
		sed "s/collation_server.*/collation_server = utf8_general_ci/" -i ${CNF_PATH} || true # ignore errors
	fi
	
	if ! grep -q "^default-authentication-plugin"  ${CNF_PATH}; then
		sed '/\[mysqld\]/a default-authentication-plugin = mysql_native_password' -i ${CNF_PATH}
	else
		sed "s/default-authentication-plugin.*/default-authentication-plugin = mysql_native_password/" -i ${CNF_PATH} || true # ignore errors
	fi

	if [ -e ${CNF_SERVICE_PATH} ]; then
		
		if ! grep -q "^LimitNOFILE"  ${CNF_SERVICE_PATH}; then
			sed '/\[Service\]/a LimitNOFILE = infinity' -i ${CNF_SERVICE_PATH}
		else
			sed "s/LimitNOFILE.*/LimitNOFILE = infinity/" -i ${CNF_SERVICE_PATH} || true # ignore errors
		fi

		if ! grep -q "^LimitMEMLOCK"  ${CNF_SERVICE_PATH}; then
			sed '/\[Service\]/a LimitMEMLOCK = infinity' -i ${CNF_SERVICE_PATH}
		else
			sed "s/LimitMEMLOCK.*/LimitMEMLOCK = infinity/" -i ${CNF_SERVICE_PATH} || true # ignore errors
		fi
	
	fi

    systemctl daemon-reload >/dev/null 2>&1
	systemctl enable ${MYSQL_PACKAGE} >/dev/null 2>&1
	systemctl restart ${MYSQL_PACKAGE}
}

setup_nginx(){
	echo -n "Configuring nginx... "
	
	# Remove default nginx website
	rm -f $NGINX_CONF/default.conf >/dev/null 2>&1 || rm -f $NGINX_DIR/sites-enabled/default >/dev/null 2>&1
    sed -i "s/listen.*;/listen $APP_PORT;/" $NGINX_CONF/onlyoffice.conf

	if [ "$DIST" = "RedHat" ]; then
		# Remove default nginx settings
		DELETION_LINE=$(sed -n '/server {/=' /etc/nginx/nginx.conf | head -n 1)
		if [ -n "$DELETION_LINE" ]; then 
			sed "$DELETION_LINE,\$d" -i /etc/nginx/nginx.conf
			echo "}" >> /etc/nginx/nginx.conf
		fi

		shopt -s nocasematch
		PORTS=()
		if $(getenforce) >/dev/null 2>&1; then
			case $(getenforce) in
				enforcing|permissive)
					PORTS+=('8081') #Storybook
					PORTS+=("$DOCUMENT_SERVER_PORT")
					PORTS+=('5001') #ASC.Web.Studio
					PORTS+=('5002') #ASC.People
					PORTS+=('5008') #ASC.Files/client
					PORTS+=('5013') #ASC.Files/editor
					setsebool -P httpd_can_network_connect on
				;;
				disabled)
					:
				;;
			esac

			for PORT in ${PORTS[@]}; do
				semanage port -a -t http_port_t -p tcp $PORT >/dev/null 2>&1 || \
				semanage port -m -t http_port_t -p tcp $PORT >/dev/null 2>&1 || \
				true
			done
		fi
	fi
    chown nginx:nginx /etc/nginx/* -R
    sudo sed -e 's/#//' -i $NGINX_CONF/onlyoffice.conf
	systemctl enable nginx >/dev/null 2>&1
	systemctl restart nginx
	echo "OK"
}

setup_docs() {
	echo -n "Configuring Docs... "
	local DS_CONF="/etc/onlyoffice/documentserver/local.json"
	local JSON_DSCONF="$JSON $DS_CONF -e"

	#Changing the Docs port in nginx conf
	sed -i "s/0.0.0.0:.*;/0.0.0.0:$DOCUMENT_SERVER_PORT;/" $NGINX_CONF/ds.conf
	sed -i "s/]:.*;/]:$DOCUMENT_SERVER_PORT default_server;/g" $NGINX_CONF/ds.conf 
	sed "0,/proxy_pass .*;/{s/proxy_pass .*;/proxy_pass http:\/\/${DOCUMENT_SERVER_HOST}:${DOCUMENT_SERVER_PORT};/}" -i $NGINX_CONF/onlyoffice.conf 	

	#Enable JWT validation for Docs
	$JSON_DSCONF "this.services.CoAuthoring.token.enable.browser='true'" >/dev/null 2>&1 
	$JSON_DSCONF "this.services.CoAuthoring.token.enable.request.inbox='true'" >/dev/null 2>&1
	$JSON_DSCONF "this.services.CoAuthoring.token.enable.request.outbox='true'" >/dev/null 2>&1
	
	local DOCUMENT_SERVER_JWT_SECRET=$(json -f ${DS_CONF} services.CoAuthoring.secret.inbox.string)
	local DOCUMENT_SERVER_JWT_HEADER=$(json -f ${DS_CONF} services.CoAuthoring.token.inbox.header)

	#Save Docs address and JWT in .json
	$JSON_USERCONF "this.files={'docservice': {\
	'secret': {'value': \"$DOCUMENT_SERVER_JWT_SECRET\",'header': \"$DOCUMENT_SERVER_JWT_HEADER\"}, \
	'url': {'public': '/ds-vpath/','internal': \"http://${DOCUMENT_SERVER_HOST}:${DOCUMENT_SERVER_PORT}\",'portal': \"http://$APP_HOST:$APP_PORT\"}}}" >/dev/null 2>&1
	
	#Enable ds-example autostart
	sed 's,autostart=false,autostart=true,' -i /etc/supervisord.d/ds-example.ini >/dev/null 2>&1 || sed 's,autostart=false,autostart=true,' -i /etc/supervisor/conf.d/ds-example.conf >/dev/null 2>&1
	supervisorctl start ds:example >/dev/null 2>&1
	
	echo "OK"
}

change_elasticsearch_config(){

	systemctl stop elasticsearch

	local ELASTIC_SEARCH_CONF_PATH="/etc/elasticsearch/elasticsearch.yml"
	local ELASTIC_SEARCH_JAVA_CONF_PATH="/etc/elasticsearch/jvm.options";

	if /usr/share/elasticsearch/bin/elasticsearch-plugin list | grep -q "ingest-attachment"; then
		/usr/share/elasticsearch/bin/elasticsearch-plugin remove -s ingest-attachment
	fi
		/usr/share/elasticsearch/bin/elasticsearch-plugin install -s -b ingest-attachment	

	if [ -f ${ELASTIC_SEARCH_CONF_PATH}.rpmnew ]; then
		cp -rf ${ELASTIC_SEARCH_CONF_PATH}.rpmnew ${ELASTIC_SEARCH_CONF_PATH};   
	fi

	if [ -f ${ELASTIC_SEARCH_JAVA_CONF_PATH}.rpmnew ]; then
		cp -rf ${ELASTIC_SEARCH_JAVA_CONF_PATH}.rpmnew ${ELASTIC_SEARCH_JAVA_CONF_PATH};   
	fi

	if ! grep -q "indices.fielddata.cache.size" ${ELASTIC_SEARCH_CONF_PATH}; then
		echo "indices.fielddata.cache.size: 30%" >> ${ELASTIC_SEARCH_CONF_PATH}
	else
		sed -i "s/indices.fielddata.cache.size.*/indices.fielddata.cache.size: 30%/" ${ELASTIC_SEARCH_CONF_PATH} 
	fi

	if ! grep -q "indices.memory.index_buffer_size" ${ELASTIC_SEARCH_CONF_PATH}; then
		echo "indices.memory.index_buffer_size: 30%" >> ${ELASTIC_SEARCH_CONF_PATH}
	else
		sed -i "s/indices.memory.index_buffer_size.*/indices.memory.index_buffer_size: 30%/" ${ELASTIC_SEARCH_CONF_PATH} 
	fi

	if grep -q "HeapDumpOnOutOfMemoryError" ${ELASTIC_SEARCH_JAVA_CONF_PATH}; then
		sed "/-XX:+HeapDumpOnOutOfMemoryError/d" -i ${ELASTIC_SEARCH_JAVA_CONF_PATH}
	fi

	local TOTAL_MEMORY=$(free -m | grep -oP '\d+' | head -n 1);
	local MEMORY_REQUIREMENTS=12228; #RAM ~4*3Gb

	if [ ${TOTAL_MEMORY} -gt ${MEMORY_REQUIREMENTS} ]; then
		if ! grep -q "[-]Xms1g" ${ELASTIC_SEARCH_JAVA_CONF_PATH}; then
			echo "-Xms4g" >> ${ELASTIC_SEARCH_JAVA_CONF_PATH}
		else
			sed -i "s/-Xms1g/-Xms4g/" ${ELASTIC_SEARCH_JAVA_CONF_PATH} 
		fi

		if ! grep -q "[-]Xmx1g" ${ELASTIC_SEARCH_JAVA_CONF_PATH}; then
			echo "-Xmx4g" >> ${ELASTIC_SEARCH_JAVA_CONF_PATH}
		else
			sed -i "s/-Xmx1g/-Xmx4g/" ${ELASTIC_SEARCH_JAVA_CONF_PATH} 
		fi
	fi

	if [ -d /etc/elasticsearch/ ]; then 
		chmod g+ws /etc/elasticsearch/
	fi
}

setup_elasticsearch() {
	echo -n "Configuring elasticsearch... "

	#Save elasticsearch parameters in .json
	$JSON $APP_DIR/elastic.json -e "this.elastic={'Scheme': \"${ELK_SHEME}\",'Host': \"${ELK_HOST}\",'Port': \"${ELK_PORT}\",'Threads': \"1\" }" >/dev/null 2>&1

	change_elasticsearch_config
	
	systemctl enable elasticsearch >/dev/null 2>&1
	systemctl restart elasticsearch
	echo "OK"
}

if command -v yum >/dev/null 2>&1; then
	DIST="RedHat"
	PACKAGE_MANAGER="rpm -q"
	MYSQL_PACKAGE="mysqld"
elif command -v apt >/dev/null 2>&1; then
	DIST="Debian"
	PACKAGE_MANAGER="dpkg -l"
	MYSQL_PACKAGE="mysql"
fi

install_json

if $PACKAGE_MANAGER mysql-client >/dev/null 2>&1 || $PACKAGE_MANAGER mysql-community-client >/dev/null 2>&1; then
    input_db_params
    establish_mysql_conn || exit $?
fi 

if $PACKAGE_MANAGER nginx >/dev/null 2>&1; then
    setup_nginx
fi

if $PACKAGE_MANAGER onlyoffice-documentserver >/dev/null 2>&1 || $PACKAGE_MANAGER onlyoffice-documentserver-de >/dev/null 2>&1 || $PACKAGE_MANAGER onlyoffice-documentserver-ee >/dev/null 2>&1; then
    setup_docs
fi

if $PACKAGE_MANAGER elasticsearch >/dev/null 2>&1; then
    setup_elasticsearch
fi

restart_services
