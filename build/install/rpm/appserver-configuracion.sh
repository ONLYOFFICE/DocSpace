
APP_HOST="localhost"
APP_PORT="80"

APP_CONF="/etc/onlyoffice/appserver/config/appsettings.test.json"
NGINX_CONF="/etc/nginx/conf.d"

MYSQL=""
DB_HOST=""
DB_NAME=""
DB_USER=""
DB_PWD=""

DOCUMENT_SERVER_HOST="localhost";
DOCUMENT_SERVER_PORT="8083";

KAFKA_HOST="localhost"
KAFKA_PORT="9092"
ZOOKEEPER_HOST="localhost"
ZOOKEEPER_PORT="2181"

ELK_SHEME="http"
ELK_HOST="localhost"
ELK_PORT="9200"
ELK_VALUE='"elastic": { "Scheme": "'${ELK_SHEME}'", "Host": "'${ELK_HOST}'", "Port": "'${ELK_PORT}'" },'

[ -e $APP_CONF ] || { echo "Configuration file not found at path $APP_CONF"; exit 1; }
[ $(id -u) -ne 0 ] && { echo "Root privileges required"; exit 1; }

restart_services() {
	echo -n "Restarting services... "

	for SVC in nginx mysqld elasticsearch kafka zookeeper appserver-api appserver-socket appserver-api_system appserver-backup \
	appserver-files appserver-files_service appserver-notify appserver-people appserver-studio appserver-studio_notify \
	appserver-thumbnails appserver-urlshortener
	do
		if systemctl is-active $SVC | grep -q "active"; then
			systemctl restart $SVC.service >/dev/null 2>&1
		else
			systemctl enable $SVC.service  >/dev/null 2>&1
			systemctl start $SVC.service  >/dev/null 2>&1
		fi
		if systemctl is-active $SVC | grep -v "active" >/dev/null; then
			echo -e "\033[31m $SVC not started \033[0m"
		fi
	done
	echo "OK"
}

input_db_params(){
    local def_DB_HOST="localhost"
    local def_DB_NAME="onlyoffice"
    local def_DB_USER="onlyoffice_user"
    local def_DB_PWD="onlyoffice_pass"

	read -e -p "Database host: " -i "$DB_HOST" DB_HOST
	read -e -p "Database name: " -i "$DB_NAME" DB_NAME
	read -e -p "Database user: " -i "$DB_USER" DB_USER
	read -e -p "Database password: " -s DB_PWD
    
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

change_mysql_config(){
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
	systemctl restart mysqld >/dev/null 2>&1
}

execute_mysql_script(){

	change_mysql_config

    mysql_check_connection
    
    if [ "$DB_USER" = "root" ] && [ ! "$(mysql -V | grep ' 5.5.')" ]; then
	   # allow connect via mysql_native_password with root and empty password
	   $MYSQL -D "mysql" -e "update user set plugin='mysql_native_password' where user='root';ALTER USER '${DB_USER}'@'localhost' IDENTIFIED WITH mysql_native_password BY '${DB_PWD}';" >/dev/null 2>&1
	fi

    DB_TABLES_COUNT=$($MYSQL --silent --skip-column-names -e "SELECT COUNT(*) FROM information_schema.tables WHERE table_schema='${DB_NAME}'"); 
    
    if [ "${DB_TABLES_COUNT}" -eq "0" ]; then
		echo -n "Installing MYSQL database... "

		sed -i -e '1 s/^/SET SQL_MODE='ALLOW_INVALID_DATES';\n/;' /etc/onlyoffice/appserver/onlyoffice.sql
		$MYSQL -e "CREATE DATABASE IF NOT EXISTS $DB_NAME CHARACTER SET utf8 COLLATE 'utf8_general_ci';"
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

	mv -f $NGINX_CONF/default.conf $NGINX_CONF/default.conf.old >/dev/null 2>&1

    sed -i "s/listen.*;/listen $APP_PORT;/" $NGINX_CONF/onlyoffice.conf

    shopt -s nocasematch
    PORTS=()
    case $(getenforce) in
        enforcing|permissive)
            PORTS+=('8081')
            PORTS+=('8083')
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
    sudo sed -e 's/#//' -i $NGINX_CONF/onlyoffice.conf
    systemctl reload nginx
	echo "OK"
}

setup_docs() {
	echo -n "Configuring Docs... "
	local DS_CONF="/etc/onlyoffice/documentserver/local.json"

	sed -i "s/0.0.0.0:.*;/0.0.0.0:$DOCUMENT_SERVER_PORT;/" $NGINX_CONF/ds.conf
	sed -i "s/]:.*;/]:$DOCUMENT_SERVER_PORT default_server;/g" $NGINX_CONF/ds.conf  	

	local DOCUMENT_SERVER_JWT_SECRET=$(cat ${DS_CONF} | jq -r '.services.CoAuthoring.secret.inbox.string')
	local DOCUMENT_SERVER_JWT_HEADER=$(cat ${DS_CONF} | jq -r '.services.CoAuthoring.token.inbox.header')

	sed "s!\"browser\": .*!\"browser\": true!" -i ${DS_CONF}
	sed "0,/\"inbox\": .*/{s/\"inbox\": .*/\"inbox\": true,/}" -i ${DS_CONF}
	sed "0,/\"outbox\": .*/{s/\"outbox\": .*/\"outbox\": true/}" -i ${DS_CONF}
	sed "s!\"internal\": .*,!\"internal\": \"http://${DOCUMENT_SERVER_HOST}:${DOCUMENT_SERVER_PORT}\",!" -i ${APP_CONF}
	sed "s!\"header\": \".*\"!\"header\": \"${DOCUMENT_SERVER_JWT_HEADER}\"!" -i ${APP_CONF}
	sed "0,/\"value\": \".*\",/{s/\"value\": \".*\",/\"value\": \"$DOCUMENT_SERVER_JWT_SECRET\",/}" -i ${APP_CONF}
	sed "s!\"portal\": \".*\"!\"portal\": \"http://$APP_HOST:$APP_PORT\"!" -i ${APP_CONF}
	sed "0,/proxy_pass .*;/{s/proxy_pass .*;/proxy_pass http:\/\/${DOCUMENT_SERVER_HOST}:${DOCUMENT_SERVER_PORT};/}" -i $NGINX_CONF/onlyoffice.conf
	
	sudo sed 's,autostart=false,autostart=true,' -i /etc/supervisord.d/ds-example.ini
	sudo supervisorctl start ds:example >/dev/null 2>&1
	sudo supervisorctl restart ds:* >/dev/null 2>&1
	
	echo "OK"
}

setup_elasticsearch() {
	echo -n "Configuring elasticsearch... "

	grep -q "${ELK_VALUE}" ${APP_CONF} || sed -i "s!\"files\".*!${ELK_VALUE}\n\"files\": {!" ${APP_CONF}
	local ELASTIC_SEARCH_VERSION=$(rpm -qi elasticsearch | grep Version | tail -n1 | awk -F': ' '/Version/ {print $2}');
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
	
	echo "OK"
}

setup_kafka() {

	local KAFKA_SERVICE=$(systemctl --type=service | grep 'kafka' | grep -oE '[^ ]+$' | tail -n1)

	if [ $KAFKA_SERVICE ]; then

		local APP_KAFKA_CONF="/etc/onlyoffice/appserver/config/kafka.test.json"

		echo -n "Configuring kafka... "

		local KAFKA_CONF="$(cat /etc/systemd/system/$KAFKA_SERVICE | grep ExecStop= | cut -c 10- | rev | cut -c 26- | rev)/config"

		sed -i "s/clientPort=.*/clientPort=${ZOOKEEPER_PORT}/g" $KAFKA_CONF/zookeeper.properties
		sed -i "s/zookeeper.connect=.*/zookeeper.connect=${ZOOKEEPER_HOST}:${ZOOKEEPER_PORT}/g" $KAFKA_CONF/server.properties
		sed -i "s/bootstrap.servers=.*/bootstrap.servers=${KAFKA_HOST}:${KAFKA_PORT}/g" $KAFKA_CONF/consumer.properties
		sed -i "s/bootstrap.servers=.*/bootstrap.servers=${KAFKA_HOST}:${KAFKA_PORT}/g" $KAFKA_CONF/connect-standalone.properties
		sed -i "s/logger.kafka.controller=.*,/logger.kafka.controller=INFO,/g" $KAFKA_CONF/log4j.properties
		sed -i "s/logger.state.change.logger=.*,/logger.state.change.logger=INFO,/g" $KAFKA_CONF/log4j.properties
		echo "log4j.logger.kafka.producer.async.DefaultEventHandler=INFO, kafkaAppender" >> $KAFKA_CONF/log4j.properties

		sed -i "s/\"BootstrapServers\".*/\"BootstrapServers\": \"${KAFKA_HOST}:${KAFKA_PORT}\"/g" ${APP_KAFKA_CONF}
	
		echo "OK"
	fi

}

if rpm -q mysql-community-client >/dev/null; then
    input_db_params
    establish_mysql_conn || exit $?
    execute_mysql_script || exit $?
fi 

if rpm -q nginx >/dev/null; then
    setup_nginx
fi

if rpm -q onlyoffice-documentserver >/dev/null || rpm -q onlyoffice-documentserver-de >/dev/null || rpm -q onlyoffice-documentserver-ee >/dev/null; then
    setup_docs
fi

if rpm -q elasticsearch >/dev/null; then
    setup_elasticsearch
fi

setup_kafka

restart_services
