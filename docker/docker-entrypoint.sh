#!/bin/bash
set -e  # Exit on any error

echo "🚀 Starting Docker entrypoint..."
echo "=================================="

# Default values
PATH_TO_CONF=${PATH_TO_CONF:-"/app/onlyoffice/config"}
SRC_PATH=${SRC_PATH:-"/app/onlyoffice/src"}
APP_CORE_BASE_DOMAIN=${APP_CORE_BASE_DOMAIN:-"localhost"}
APP_URL_PORTAL=${APP_URL_PORTAL:-"http://127.0.0.1:8092"}
APP_CORE_MACHINEKEY=${APP_CORE_MACHINEKEY:-"your_core_machinekey"}

DOCUMENT_CONTAINER_NAME=${DOCUMENT_CONTAINER_NAME:-"onlyoffice-document-server"}
DOCUMENT_SERVER_URL_PUBLIC=${DOCUMENT_SERVER_URL_PUBLIC:-"/ds-vpath/"}
DOCUMENT_SERVER_URL_EXTERNAL=${DOCUMENT_SERVER_URL_EXTERNAL:-"http://${DOCUMENT_CONTAINER_NAME}"}
DOCUMENT_SERVER_JWT_SECRET=${DOCUMENT_SERVER_JWT_SECRET:-"your_jwt_secret"}
DOCUMENT_SERVER_JWT_HEADER=${DOCUMENT_SERVER_JWT_HEADER:-"AuthorizationJwt"}
OAUTH_REDIRECT_URL=${OAUTH_REDIRECT_URL:-"https://service.onlyoffice.com/oauth2.aspx"}

HIDE_SETTINGS=[\n\"Monitoring\",\n\"LdapSettings\",\n\"DocService\",\n\"MailService\",\n\"PublicPortal\",\n\"ProxyHttpContent\",\n\"SpamSubscription\",\n\"FullTextSearch\",\n\"IdentityServer\"\n]

MYSQL_CONTAINER_NAME=${MYSQL_CONTAINER_NAME:-"localhost"}
MYSQL_HOST=${MYSQL_HOST:-${MYSQL_CONTAINER_NAME}}
MYSQL_PORT=${MYSQL_PORT:-"3306"}
MYSQL_DATABASE=${MYSQL_DATABASE:-"docspace"}
MYSQL_USER=${MYSQL_USER:-"onlyoffice_user"}
MYSQL_PASSWORD=${MYSQL_PASSWORD:-"onlyoffice_pass"}
COMMAND_TIMEOUT=${COMMAND_TIMEOUT:-"100"}

RABBIT_CONTAINER_NAME=${RABBIT_CONTAINER_NAME:-"onlyoffice-rabbitmq"}
REDIS_CONTAINER_NAME=${REDIS_CONTAINER_NAME:-"onlyoffice-redis"}

ELK_SHEME=${ELK_SHEME:-"http"}
ELK_HOST=${ELK_HOST:-"localhost"}
ELK_PORT=${ELK_PORT:-"9200"}
ELK_THREADS=${ELK_THREADS:-"1"}

MIGRATION_TYPE=${MIGRATION_TYPE:-"STANDALONE"}  # STANDALONE or SAAS

# Function to log with timestamp
log() {
    echo "[$(date +'%Y-%m-%d %H:%M:%S')] $1"
}

# Function to update configuration files
update_configs() {
    log "📝 Updating configuration files..."
    
    # Main appsettings
    sed -i "s!\"connectionString\".*;Pooling=!\"connectionString\": \"Server=${MYSQL_HOST};Port=${MYSQL_PORT};Database=${MYSQL_DATABASE};User ID=${MYSQL_USER};Password=${MYSQL_PASSWORD};Pooling=!g" ${PATH_TO_CONF}/appsettings.json
    sed -i "s!\"base-domain\".*,!\"base-domain\": \"${APP_CORE_BASE_DOMAIN}\",!g" ${PATH_TO_CONF}/appsettings.json
    sed -i "s!\"machinekey\".*,!\"machinekey\": \"${APP_CORE_MACHINEKEY}\",!g" ${PATH_TO_CONF}/appsettings.json
    sed -i "s!\"public\".*,!\"public\": \"${DOCUMENT_SERVER_URL_PUBLIC}\",!g" ${PATH_TO_CONF}/appsettings.json
    sed -i "s!\"internal\".*,!\"internal\": \"${DOCUMENT_SERVER_URL_EXTERNAL}/\\\",!g" ${PATH_TO_CONF}/appsettings.json
    sed -i "0,/\"value\"/s!\"value\".*,!\"value\": \"${DOCUMENT_SERVER_JWT_SECRET}\",!" ${PATH_TO_CONF}/appsettings.json
    sed -i "s!\"portal\".*!\"portal\": \"${APP_URL_PORTAL}\"!g" ${PATH_TO_CONF}/appsettings.json
    #sed -i "s!\"hide-settings\".*],!\"hide-settings\": ${HIDE_SETTINGS},!g" ${PATH_TO_CONF}/appsettings.json 
    
    # API System
    sed -i "s!\"connectionString\".*;Pooling=!\"connectionString\": \"Server=${MYSQL_HOST};Port=${MYSQL_PORT};Database=${MYSQL_DATABASE};User ID=${MYSQL_USER};Password=${MYSQL_PASSWORD};Pooling=!g" ${PATH_TO_CONF}/apisystem.json
    sed -i "s!\"base-domain\".*,!\"base-domain\": \"${APP_CORE_BASE_DOMAIN}\",!g" ${PATH_TO_CONF}/apisystem.json
    sed -i "s!\"machinekey\".*,!\"machinekey\": \"${APP_CORE_MACHINEKEY}\",!g" ${PATH_TO_CONF}/apisystem.json
    sed -i "s!\"postman\".*!\"postman\": \"services\"!g" ${PATH_TO_CONF}/appsettings.json
    
    # Migration Runner
    MIGRATION_PARAMS=""
    if [[ ${MIGRATION_TYPE} == "STANDALONE" ]]; then
        MIGRATION_PARAMS="standalone=true"
    fi

    sed -i "s!\"ConnectionString\".*!\"ConnectionString\": \"Server=${MYSQL_HOST};Port=${MYSQL_PORT};Database=${MYSQL_DATABASE};User ID=${MYSQL_USER};Password=${MYSQL_PASSWORD};Command Timeout=${COMMAND_TIMEOUT}\"!g" ${SRC_PATH}/publish/services/backend/appsettings.runner.json

    # Autofac consumers
    sed -i "s!\"https://service\.teamlab\.info/oauth2\.aspx\"!\"${OAUTH_REDIRECT_URL}\"!g" ${PATH_TO_CONF}/autofac.consumers.json
    
    # Message queue
    sed -i "s!\"Hostname\".*!\"Hostname\": \"${RABBIT_CONTAINER_NAME}\",!g" ${PATH_TO_CONF}/rabbitmq.json
    sed -i "s!\"Host\".*!\"Host\": \"${REDIS_CONTAINER_NAME}\",!g" ${PATH_TO_CONF}/redis.json
    
    # Elastic
    sed -i "s!\"Scheme\".*!\"Scheme\": \"${ELK_SHEME}\",!g" ${PATH_TO_CONF}/elastic.json
    sed -i "s!\"Host\".*!\"Host\": \"${ELK_HOST}\",!g" ${PATH_TO_CONF}/elastic.json
    sed -i "s!\"Port\".*!\"Port\": \"${ELK_PORT}\",!g" ${PATH_TO_CONF}/elastic.json
    sed -i "s!\"Threads\".*!\"Threads\": \"${ELK_THREADS}\"!g" ${PATH_TO_CONF}/elastic.json
    
    # RabbitMQ override
    cat > "${PATH_TO_CONF}/rabbitmq.json" <<EOF
{
  "RabbitMQ": {}
}
EOF
    
    log "✅ Configuration files updated"
}

# Function to wait for MySQL and run migrations
run_migrations() {
    log "🔍 Starting migration process..."
    
    # Build migration parameters
    MIGRATION_PARAMS=""
    if [[ ${MIGRATION_TYPE} == "STANDALONE" ]]; then
        MIGRATION_PARAMS="standalone=true"
        log "   Mode: STANDALONE"
    else
        log "   Mode: SAAS"
    fi
    
    # Wait for MySQL
    log "⏳ Waiting for MySQL to be ready..."
    counter=0
    MAX_RETRIES=30
    until mysql -h "$MYSQL_HOST" -P "$MYSQL_PORT" \
        -u "$MYSQL_USER" -p"$MYSQL_PASSWORD" \
        -e "SELECT 1" "$MYSQL_DATABASE" >/dev/null 2>&1; do
        counter=$((counter + 1))
        if [ $counter -ge $MAX_RETRIES ]; then
            log "❌ MySQL not available after $MAX_RETRIES attempts"
            return 1
        fi
        log "   Waiting... ($counter/$MAX_RETRIES)"
        sleep 2
    done
    log "✅ MySQL is ready!"
    
    # Check current migration state
    log "📋 Current migration state:"
    mysql -h "$MYSQL_HOST" -P "$MYSQL_PORT" \
        -u "$MYSQL_USER" -p"$MYSQL_PASSWORD" \
        -e "SELECT COUNT(*) as 'Total Migrations' FROM __EFMigrationsHistory;" "$MYSQL_DATABASE" 2>/dev/null || log "   No migrations table yet"
    
    log ""
    log "📋 Last 5 migrations:"
    mysql -h "$MYSQL_HOST" -P "$MYSQL_PORT" \
        -u "$MYSQL_USER" -p"$MYSQL_PASSWORD" \
        -e "SELECT MigrationId, ProductVersion FROM __EFMigrationsHistory ORDER BY MigrationId DESC LIMIT 5;" "$MYSQL_DATABASE" 2>/dev/null || log "   No migrations applied yet"
    
    log ""
    
    # Run migration
    log "🚀 Running database migration..."
    cd ${SRC_PATH}/publish/services/backend/
    
    if dotnet ASC.Migration.Runner.dll ${MIGRATION_PARAMS}; then
        log "✅ Migration completed successfully"
        
        # Show updated state
        log ""
        log "📋 Updated migration state:"
        mysql -h "$MYSQL_HOST" -P "$MYSQL_PORT" \
            -u "$MYSQL_USER" -p"$MYSQL_PASSWORD" \
            -e "SELECT COUNT(*) as 'Total Migrations' FROM __EFMigrationsHistory;" "$MYSQL_DATABASE" 2>/dev/null
        
        log ""
        log "📋 Most recent migrations:"
        mysql -h "$MYSQL_HOST" -P "$MYSQL_PORT" \
            -u "$MYSQL_USER" -p"$MYSQL_PASSWORD" \
            -e "SELECT MigrationId, ProductVersion FROM __EFMigrationsHistory ORDER BY MigrationId DESC LIMIT 5;" "$MYSQL_DATABASE" 2>/dev/null
        
        # Create success flag
        touch /tmp/migration-completed
        return 0
    else
        log "❌ Migration failed"
        return 1
    fi
}

# Main execution
main() {
    log "=== Starting initialization ==="
    
    # Step 1: Update configuration files
    update_configs
    
    # Step 2: Run migrations
    if ! run_migrations; then
        log "❌ Migration failed - exiting"
        exit 1
    fi
    
    log "✅ Initialization complete - starting supervisord"
    log "=================================="
    
    # Step 3: Start supervisord (which will start all services)
    exec supervisord -n
}

# Run main function
main
