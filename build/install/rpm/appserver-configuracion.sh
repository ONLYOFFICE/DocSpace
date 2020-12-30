MYSQL=""

DB_PORT="3306"  
APP_HOST="localhost"
APP_PORT="80"

APP_CONF="/etc/onlyoffice/appserver/config/appsettings.test.json"
DS_CONF="/etc/onlyoffice/documentserver/local.json"
KAFKA_CONF="/etc/onlyoffice/appserver/config/kafka.test.json"
NGINX_CONF="/etc/nginx/conf.d/onlyoffice.conf"

DB_HOST=""
DB_NAME=""
DB_USER=""
DB_PWD=""

DOCUMENT_SERVER_HOST="localhost";
DOCUMENT_SERVER_PORT="8083";

KAFKA_HOST="localhost"
KAFKA_PORT="9092"

ELK_SHEME="http"
ELK_HOST="localhost"
ELK_PORT="9200"
ELK_VALUE='"elastic": { "Scheme": "'${ELK_SHEME}'", "Host": "'${ELK_HOST}'", "Port": "'${ELK_PORT}'" },'

[ -e $APP_CONF ] || { echo "Configuration file not found at path $APP_CONF"; exit 1; }
[ $(id -u) -ne 0 ] && { echo "Root privileges required"; exit 1; }

restart_services() {
	echo -n "Restarting services... "

	for SVC in nginx mysqld appserver-api appserver-socket appserver-api_system appserver-backup appserver-files appserver-files_service appserver-notify appserver-people appserver-studio appserver-studio_notify appserver-thumbnails appserver-urlshortener
	do
		systemctl stop $SVC.service  >/dev/null 2>&1
		systemctl start $SVC.service  >/dev/null 2>&1
	done
	echo "OK"
}

input_db_params(){
    local def_DB_HOST="localhost"
    local def_DB_NAME="onlyoffice"
    local def_DB_USER="root"
    local def_DB_PWD="root"
    if read -i "default" 2>/dev/null <<< "test"; then 
		read -e -p "Database host: " -i "$DB_HOST" DB_HOST
		read -e -p "Database name: " -i "$DB_NAME" DB_NAME
		read -e -p "Database user: " -i "$DB_USER" DB_USER
		read -e -p "Database password: " -s DB_PWD
	else
		read -e -p "Database host (default $def_DB_HOST): " DB_HOST
		read -e -p "Database name (default $def_DB_NAME): " DB_NAME
		read -e -p "Database user (default $def_DB_USER): " DB_USER
		read -e -p "Database password: " -s DB_PWD
    fi
    
    if [ -z $DB_HOST ]; then
		DB_HOST="${def_DB_HOST}";
	fi

	if [ -z $DB_NAME ]; then
		DB_NAME="${def_DB_NAME}";
	fi

	if [ -z $DB_USER ]; then
		DB_USER="${def_DB_USER}";
	fi

    echo
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
		systemctl mysqld start >/dev/null 2>&1
		$MYSQL -e ";" >/dev/null 2>&1 || { echo "FAILURE"; exit 1; }
	fi

    #save db params
    sed -i "s/Server=.*;Port=/Server=$DB_HOST;Port=/" /etc/onlyoffice/appserver/config/appsettings.test.json
    sed -i "s/Database=.*;User/Database=$DB_NAME;User/" /etc/onlyoffice/appserver/config/appsettings.test.json
    sed -i "s/User ID=.*;Password=/User ID=$DB_USER;Password=/" /etc/onlyoffice/appserver/config/appsettings.test.json
    sed -i "s/Password=.*;Pooling=/Password=$DB_PWD;Pooling=/" /etc/onlyoffice/appserver/config/appsettings.test.json
    
	echo "OK"
}

mysql_check_connection() {
	while ! $MYSQL -e ";" >/dev/null 2>&1; do
    		sleep 1
	done
}

execute_mysql_sqript(){
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

    systemctl daemon-reload >/dev/null 2>&1
	systemctl stop mysqld >/dev/null 2>&1
	systemctl start mysqld >/dev/null 2>&1

    mysql_check_connection
    
    if [ "$DB_USER" = "root" ] && [ ! "$(mysql -V | grep ' 5.5.')" ]; then
	   # allow connect via mysql_native_password with root and empty password
	   $MYSQL -D "mysql" -e "update user set plugin='mysql_native_password' where user='root';ALTER USER '${DB_USER}'@'localhost' IDENTIFIED WITH mysql_native_password BY '${DB_PWD}';" >/dev/null 2>&1
	fi

    DB_TABLES_COUNT=$($MYSQL --silent --skip-column-names -e "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema='${DB_NAME}'"); 
    
    if [ "${DB_TABLES_COUNT}" -eq "0" ]; then
	echo -n "Installing MYSQL database... "

    sed -i -e '1 s/^/SET SQL_MODE='ALLOW_INVALID_DATES';\n/;' /etc/onlyoffice/appserver/onlyoffice.sql
	$MYSQL -e "CREATE DATABASE IF NOT EXISTS $DB_NAME CHARACTER SET utf8 COLLATE 'utf8_general_ci';" >/dev/null 2>&1
    $MYSQL "$DB_NAME" < "/etc/onlyoffice/appserver/createdb.sql" >/dev/null 2>&1
    $MYSQL "$DB_NAME" < "/etc/onlyoffice/appserver/onlyoffice.sql" >/dev/null 2>&1
    $MYSQL "$DB_NAME" < "/etc/onlyoffice/appserver/onlyoffice.data.sql" >/dev/null 2>&1
    $MYSQL "$DB_NAME" < "/etc/onlyoffice/appserver/onlyoffice.resources.sql" >/dev/null 2>&1
	else
		echo -n "Upgrading MySQL database... "
    fi
    echo "OK"
}

setup_nginx(){
	echo -n "Configuring nginx... "

	rm -rf /etc/nginx/conf.d/default.conf

    sed -i "s/listen.*;/listen $APP_PORT;/" $NGINX_CONF

    shopt -s nocasematch
    PORTS=()
    case $(getenforce) in
        enforcing|permissive)
            PORTS+=('8081')
            PORTS+=('5001')
            PORTS+=('5002')
            PORTS+=('5008')
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
    chown nginx:nginx /etc/nginx/* -R
    sudo sed -e 's/#//' -i $NGINX_CONF
    systemctl reload nginx
	echo "OK"
}

setup_docs() {
	echo -n "Configuring Docs... "
	DOCUMENT_SERVER_JWT_SECRET=$(cat ${DS_CONF} | jq -r '.services.CoAuthoring.secret.inbox.string')
	DOCUMENT_SERVER_JWT_HEADER=$(cat ${DS_CONF} | jq -r '.services.CoAuthoring.token.inbox.header')

	sed "s!\"browser\": .*!\"browser\": false!" -i ${DS_CONF}
	sed "s!\"internal\": .*,!\"internal\": \"${DOCUMENT_SERVER_HOST}:${DOCUMENT_SERVER_PORT}\",!" -i ${APP_CONF}
	sed "s!\"header\": \".*\"!\"header\": \"${DOCUMENT_SERVER_JWT_HEADER}\"!" -i ${APP_CONF}
	sed "0,/\"value\": \".*\",/{s/\"value\": \".*\",/\"value\": \"$DOCUMENT_SERVER_JWT_SECRET\",/}" -i ${APP_CONF}
	sed "s!\"portal\": \".*\"!\"portal\": \"\"!" -i ${APP_CONF}
	sed "0,/proxy_pass .*;/{s/proxy_pass .*;/proxy_pass http:\/\/${DOCUMENT_SERVER_HOST}:${DOCUMENT_SERVER_PORT};/}" -i $NGINX_CONF

	echo "OK"
}

if rpm -q mysql-community-client >/dev/null; then
    input_db_params
    establish_mysql_conn || exit $?
    execute_mysql_sqript || exit $?
fi 

if rpm -q nginx >/dev/null; then
    setup_nginx
fi

if rpm -q onlyoffice-documentserver >/dev/null || rpm -q onlyoffice-documentserver-de >/dev/null || rpm -q onlyoffice-documentserver-ee >/dev/null; then
    setup_docs
fi

#kafka
sed -i "s!\"BootstrapServers\".*!\"BootstrapServers\": \"${KAFKA_HOST}:${KAFKA_PORT}\"!g" ${KAFKA_CONF}

#elastic
grep -q "${ELK_VALUE}" ${APP_CONF} || sed -i "s!\"files\".*!${ELK_VALUE}\n\"files\": {!" ${APP_CONF}

restart_services
