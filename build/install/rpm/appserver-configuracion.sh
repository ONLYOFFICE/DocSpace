MYSQL=""
DB_PORT=3306  
AS_PORT=""
AS_PORT=${AS_PORT:-80}

input_db_params(){
    echo "Configuring database access... "
	read -e -p "Host: " -i "$DB_HOST" DB_HOST
	read -e -p "Database name: " -i "$DB_NAME" DB_NAME
	read -e -p "User: " -i "$DB_USER" DB_USER 
	read -e -p "Password: " -s DB_PWD
	echo
}

establish_mysql_conn(){
	echo -n "Trying to database MySQL connection... "
	command -v mysql >/dev/null 2>&1 || { echo "MySQL client not found"; exit 1; }
	MYSQL="mysql -h$DB_HOST -u$DB_USER"
	if [ -n "$DB_PWD" ]; then
		MYSQL="$MYSQL -p$DB_PWD"
	fi 

	$MYSQL -e ";" >/dev/null 2>&1 || { echo "FAILURE"; exit 1; }

	echo "OK"
}

execute_mysql_sqript(){
	echo -n "Installing MYSQL database... "

    sed -i "s/Server=.*;Port=/Server=$DB_HOST;Port=/" /app/onlyoffice/config/appsettings.test.json
    sed -i "s/Database=.*;User/Database=$DB_NAME;User/" /app/onlyoffice/config/appsettings.test.json
    sed -i "s/User ID=.*;Password=/User ID=$DB_USER;Password=/" /app/onlyoffice/config/appsettings.test.json
    sed -i "s/Password=.*;Pooling=/Password=$DB_PWD;Pooling=/" /app/onlyoffice/config/appsettings.test.json

    $MYSQL -e "SET SQL_MODE='ALLOW_INVALID_DATES';"
	$MYSQL -e "CREATE DATABASE IF NOT EXISTS $DB_NAME CHARACTER SET utf8 COLLATE 'utf8_general_ci';"
    $MYSQL "$DB_NAME" < "/app/onlyoffice/createdb.sql"
    $MYSQL "$DB_NAME" < "/app/onlyoffice/onlyoffice.sql"
    $MYSQL "$DB_NAME" < "/app/onlyoffice/onlyoffice.data.sql"
    $MYSQL "$DB_NAME" < "/app/onlyoffice/onlyoffice.resources.sql"
    #tenants_tenants res_files webstudio_settings
	echo "OK"
}

setup_nginx(){
    sed -i "s/listen.*;/listen $AS_PORT;/" /etc/nginx/conf.d/onlyoffice.conf

    shopt -s nocasematch
    PORTS=()
    case $(getenforce) in
        enforcing|permissive)
            PORTS+=('8081')
            PORTS+=('5001')
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
    sudo sed -e 's/#//' -i /etc/nginx/conf.d/onlyoffice.conf
}

if rpm -q mysql-community-client >/dev/null; then
    input_db_params
    establish_mysql_conn || exit $?
    execute_mysql_sqript || exit $?
fi 
if rpm -q nginx >/dev/null; then
    setup_nginx
fi
