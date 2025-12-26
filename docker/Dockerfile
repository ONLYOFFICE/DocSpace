ARG SRC_PATH="/app/onlyoffice/src"
ARG BUILD_PATH="/var/www"
ARG APP_STORAGE_ROOT="/app/onlyoffice/data"
ARG PATH_TO_CONF="/app/onlyoffice/config"
ARG LOG_DIR="/var/log/onlyoffice"

#----------------------------------
#         Image SDK base         
#----------------------------------
FROM ubuntu:22.04 AS build
ARG SRC_PATH
ARG BUILD_PATH
ARG APP_STORAGE_ROOT
ARG PATH_TO_CONF
ARG LOG_DIR
ARG PRODUCT="onlyoffice"
ARG DEBIAN_FRONTEND="noninteractive"
ARG OPENSEARCH_PATH="/opt/opensearch/config"
ARG BUILD_ARGS="build"
ARG DEPLOY_ARGS="deploy"
ARG ENV_EXTENSION=""
ARG BUILD_DATE=""

ARG MYSQL_PORT=3306
ARG MYSQL_DATABASE=docspace
ARG MYSQL_USER=${PRODUCT}_user
ARG MYSQL_PASSWORD=${PRODUCT}_pass
ARG MYSQL_HOST="127.0.0.1"
ARG MYSQL_ROOT_PASSWORD=my-secret-pw

ARG REDIS_HOST="127.0.0.1"
ARG REDIS_PORT=6379
ARG REDIS_USER_NAME=""
ARG REDIS_PASSWORD=""
ARG REDIS_DB="0"

ARG RABBIT_PROTOCOL=""
ARG RABBIT_HOST="localhost"
ARG RABBIT_PORT=5672
ARG RABBIT_VIRTUAL_HOST=/
ARG RABBIT_USER_NAME=guest
ARG RABBIT_PASSWORD=guest

ARG OPENSEARCH_VERSION=2.18.0
ARG DASHBOARDS_VERSION=2.18.0

ARG DOCUMENT_CONTAINER_NAME="onlyoffice-document-server"
ARG DOCUMENT_SERVER_URL_EXTERNAL=""
ARG COUNT_WORKER_CONNECTIONS=1024

ARG JDBC_URL=${MYSQL_HOST}
ARG JDBC_DATABASE=${MYSQL_DATABASE}
ARG JDBC_USER_NAME=${MYSQL_USER}
ARG JDBC_PASSWORD=${MYSQL_PASSWORD}

ENV DEBIAN_FRONTEND=${DEBIAN_FRONTEND} \
    DNS_NAMESERVER=127.0.0.11 \
    COUNT_WORKER_CONNECTIONS=$COUNT_WORKER_CONNECTIONS \
    MAP_HASH_BUCKET_SIZE="" \
    SRC_PATH=${SRC_PATH} \
    APP_STORAGE_ROOT=${APP_STORAGE_ROOT} \
    PATH_TO_CONF=${PATH_TO_CONF} \
    LOG_DIR=${LOG_DIR} \
    BUILD_PATH=${BUILD_PATH} \
    ENV_EXTENSION=${ENV_EXTENSION} \
    BUILD_DATE=${BUILD_DATE} \
    DOCUMENT_CONTAINER_NAME=${DOCUMENT_CONTAINER_NAME} \
    DOCUMENT_SERVER_URL_EXTERNAL=${DOCUMENT_SERVER_URL_EXTERNAL} \
    OPENSEARCH_VERSION=${OPENSEARCH_VERSION} \
    DASHBOARDS_VERSION=${DASHBOARDS_VERSION} \
    MIGRATION_TYPE="standalone=true" \
    RABBIT_PROTOCOL=${RABBIT_PROTOCOL} \
    RABBIT_HOST=${RABBIT_HOST} \
    RABBIT_PORT=${RABBIT_PORT} \
    RABBIT_VIRTUAL_HOST=${RABBIT_VIRTUAL_HOST} \
    RABBIT_USER_NAME=${RABBIT_USER_NAME} \
    RABBIT_PASSWORD=${RABBIT_PASSWORD} \
    REDIS_HOST=${REDIS_HOST} \
    REDIS_PORT=${REDIS_PORT} \
    REDIS_USER_NAME=${REDIS_USER_NAME} \
    REDIS_PASSWORD=${REDIS_PASSWORD} \
    REDIS_DB=${REDIS_DB} \
    MYSQL_PORT=${MYSQL_PORT} \
    MYSQL_DATABASE=${MYSQL_DATABASE} \
    MYSQL_USER=${MYSQL_USER} \
    MYSQL_PASSWORD=${MYSQL_PASSWORD} \
    MYSQL_HOST=${MYSQL_HOST} \
    MYSQL_ROOT_PASSWORD=${MYSQL_ROOT_PASSWORD} \
    JDBC_URL=${MYSQL_HOST} \
    JDBC_DATABASE=${MYSQL_DATABASE} \
    JDBC_USER_NAME=${MYSQL_USER} \
    JDBC_PASSWORD=${MYSQL_PASSWORD}

#----------------------------------
#       Install dependencies
#----------------------------------
RUN mkdir -p /var/log/onlyoffice && \
    mkdir -p /app/onlyoffice/data && \
    apt-get -y update && \
    apt-get install -yq \
        sudo \
        adduser \
        dos2unix \
        nano \
        git \
        wget \
        curl \
        vim \
        gnupg \
        lsb-release \
        ca-certificates \
        gettext-base \
        supervisor \
        python3-launchpadlib \
        apt-transport-https \
        openjdk-21-jdk \
        software-properties-common && \
    addgroup --system --gid 107 onlyoffice && \
    adduser -uid 1004 --quiet --home /var/www/onlyoffice --system --gid 107 onlyoffice

#----------------------------------
#       Install .Net
#----------------------------------
RUN add-apt-repository -y ppa:dotnet/backports && \
    apt-get -y update --fix-missing && \
    apt-get install -yq \
        dotnet-sdk-10.0

#----------------------------------
#       Install RabbitMQ
#----------------------------------
RUN add-apt-repository ppa:rabbitmq/rabbitmq-erlang-25 -y && \
    apt-get update -y && \
    apt-get install -y erlang-base \
                        erlang-asn1 erlang-crypto erlang-eldap erlang-ftp erlang-inets \
                        erlang-mnesia erlang-os-mon erlang-parsetools erlang-public-key \
                        erlang-runtime-tools erlang-snmp erlang-ssl \
                        erlang-syntax-tools erlang-tftp erlang-tools erlang-xmerl \
                        rabbitmq-server -y --fix-missing

#----------------------------------
#       Install MySql
#----------------------------------
RUN apt-get update -y && \
    apt-get install -yq \
        mysql-server

#----------------------------------
#       Install Openresty
#----------------------------------
RUN <<EOF
    #!/bin/bash
    set -xe
    ARCH_LINUX=$(lscpu | grep Architecture | awk '{print $2}')
    echo "--- ADD necessary services arh: ${ARCH_LINUX} ---"
    if [ "$ARCH_LINUX" = "x86_64" ] ; then
	    wget -O - https://openresty.org/package/pubkey.gpg | sudo gpg --dearmor -o /usr/share/keyrings/openresty.gpg && \
	    echo "deb [arch=amd64 signed-by=/usr/share/keyrings/openresty.gpg] http://openresty.org/package/debian bookworm openresty" | sudo tee /etc/apt/sources.list.d/openresty.list && \
        apt update && \
        apt install -y openresty
    fi
    if [ "$ARCH_LINUX" = "aarch64" ] ; then
        wget -O - https://openresty.org/package/pubkey.gpg | sudo gpg --dearmor -o /usr/share/keyrings/openresty.gpg && \
        echo "deb [arch=arm64 signed-by=/usr/share/keyrings/openresty.gpg] http://openresty.org/package/arm64/debian bookworm openresty" | sudo tee /etc/apt/sources.list.d/openresty.list && \
        apt update && \
        apt install -y openresty
    fi
EOF

#----------------------------------
#          Install Redis           
#----------------------------------
RUN add-apt-repository -y ppa:redislabs/redis && \
    apt-get update && \
    apt-get install -y redis-server 

#----------------------------------
#         clean up         
#----------------------------------    
RUN echo "--- delete temporary files ---" && \
    apt-get clean && \
    rm -rf /var/lib/apt/lists/* \
    /tmp/* 

#----------------------------------
#       Install Nodejs and yarn
#----------------------------------
RUN curl -sS https://dl.yarnpkg.com/debian/pubkey.gpg | gpg --dearmor -o /usr/share/keyrings/yarn-archive-keyring.gpg && \
    echo "deb [signed-by=/usr/share/keyrings/yarn-archive-keyring.gpg] https://dl.yarnpkg.com/debian/ stable main" > /etc/apt/sources.list.d/yarn.list && \
    apt-get -y update && \
    apt-get install -yq \
	    yarn && \ 
    curl -fsSL https://deb.nodesource.com/setup_22.x | bash - && \
    apt-get install -y nodejs && \
    node -v    

#----------------------------------
#       Install OpenSearch             
#----------------------------------    
RUN <<EOF
    #!/bin/bash
    set -xe
    ARCH_LINUX=$(lscpu | grep Architecture | awk '{print $2}')
    useradd -M -s /bin/bash -r opensearch
    mkdir -p /var/lib/opensearch /var/log/opensearch

    echo "--- ADD necessary services arh: ${ARCH_LINUX} ---"
    if [ "$ARCH_LINUX" = "x86_64" ] ; then
	    wget -qO- https://artifacts.opensearch.org/releases/bundle/opensearch/${OPENSEARCH_VERSION}/opensearch-${OPENSEARCH_VERSION}-linux-x64.tar.gz | tar -xz -C /opt/ && \
        wget -qO- https://artifacts.opensearch.org/releases/bundle/opensearch-dashboards/${DASHBOARDS_VERSION}/opensearch-dashboards-${DASHBOARDS_VERSION}-linux-x64.tar.gz | tar -xz -C /opt/
    fi
    if [ "$ARCH_LINUX" = "aarch64" ] ; then
        wget -qO- https://artifacts.opensearch.org/releases/bundle/opensearch/${OPENSEARCH_VERSION}/opensearch-${OPENSEARCH_VERSION}-linux-arm64.tar.gz | tar -xz -C /opt/ && \
        wget -qO- https://artifacts.opensearch.org/releases/bundle/opensearch-dashboards/${DASHBOARDS_VERSION}/opensearch-dashboards-${DASHBOARDS_VERSION}-linux-arm64.tar.gz | tar -xz -C /opt/
    fi
    chown -R opensearch:opensearch /var/lib/opensearch /var/log/opensearch /opt/opensearch-${OPENSEARCH_VERSION} /opt/opensearch-dashboards-${DASHBOARDS_VERSION} && \
    ln -s /opt/opensearch-${OPENSEARCH_VERSION} /opt/opensearch && \
    ln -s /opt/opensearch-dashboards-${DASHBOARDS_VERSION} /opt/opensearch-dashboards && \
    chown -R opensearch:opensearch /var/lib/opensearch \
                                   /var/log/opensearch \
                                   /opt/opensearch-${OPENSEARCH_VERSION} \
                                   /opt/opensearch \
                                   /opt/opensearch-dashboards-${DASHBOARDS_VERSION} \
                                   /opt/opensearch-dashboards && \
    chmod 755 /opt/opensearch && \
    chmod 755 /opt/opensearch-dashboards
EOF

# opensearch install ingest-attachment
RUN /opt/opensearch/bin/opensearch-plugin install  -s -b ingest-attachment

# Configure OpenSearch
RUN echo "cluster.name: docker-opensearch" >> ${OPENSEARCH_PATH}/opensearch.yml && \
    echo "network.host: 0.0.0.0" >> ${OPENSEARCH_PATH}/opensearch.yml && \
    echo "discovery.type: single-node" >> ${OPENSEARCH_PATH}/opensearch.yml && \
    echo "plugins.security.disabled: true" >> ${OPENSEARCH_PATH}/opensearch.yml

# Reduce JVM heap size for containers
RUN sed -i 's/-Xms[0-9]\+[a-z]/-Xms512m/g; s/-Xmx[0-9]\+[a-z]/-Xmx512m/g' ${OPENSEARCH_PATH}/jvm.options

#----------------------------------
#              Get src             
#----------------------------------
ADD https://api.github.com/repos/ONLYOFFICE/DocSpace-buildtools/git/refs/heads/${GIT_BRANCH} version.json
RUN git clone -b "master" --depth 1 https://github.com/ONLYOFFICE/docspace-plugins.git ${SRC_PATH}/plugins && \
        git clone -b "master" --depth 1 https://github.com/ONLYOFFICE/ASC.Web.Campaigns.git ${SRC_PATH}/campaigns
COPY --chown=onlyoffice:onlyoffice ./server/ ${SRC_PATH}/server/
COPY --chown=onlyoffice:onlyoffice ./client/ ${SRC_PATH}/client/
COPY --chown=onlyoffice:onlyoffice ./buildtools/config/ ${SRC_PATH}/buildtools/config/
COPY --chown=onlyoffice:onlyoffice ./docker/docker-entrypoint.sh /docker-entrypoint.sh
COPY --chown=onlyoffice:onlyoffice ./docker/init_db.sh /init_db.sh
COPY --chown=onlyoffice:onlyoffice ./docker/supervisord/supervisord.conf /etc/supervisor/conf.d/supervisord.conf

WORKDIR ${SRC_PATH}/buildtools/config
RUN <<EOF
    echo "--- customize config base files ---" && \
    mkdir -p /app/onlyoffice/config/ && \
    ls | grep -v "test" | grep -v "\.dev\." | grep -v "nginx" | xargs cp -t /app/onlyoffice/config/
    cp *.config /app/onlyoffice/config/
EOF

#----------------------------------
#         Nodejs build client
#----------------------------------
FROM node:22.12.0 AS build-nodejs-client
ARG SRC_PATH
ARG DEBIAN_FRONTEND
ARG BUILD_ARGS="build"
ARG DEPLOY_ARGS="deploy"

# build frondend from DocSpace-client
WORKDIR ${SRC_PATH}
COPY ./client/ ${SRC_PATH}/client/
COPY ./buildtools/config/ ${SRC_PATH}/buildtools/config/
WORKDIR ${SRC_PATH}/client

RUN <<EOF
#!/bin/bash
echo "--- build/publish docspace-client node ---" && \
yarn install
node common/scripts/before-build.js

CLIENT_PACKAGES+=("@docspace/client")
CLIENT_PACKAGES+=("@docspace/login")
CLIENT_PACKAGES+=("@docspace/doceditor")
CLIENT_PACKAGES+=("@docspace/sdk")
CLIENT_PACKAGES+=("@docspace/management")

for PKG in ${CLIENT_PACKAGES[@]}; do
  echo "--- build/publish ${PKG} ---"
  yarn workspace ${PKG} ${BUILD_ARGS} $([[ "${PKG}" =~ (client|management) ]] && echo "--env lint=false")
  yarn workspace ${PKG} ${DEPLOY_ARGS}
done

echo "--- publish public web files ---" && \
cp -rf public "${SRC_PATH}/publish/web/"
echo "--- publish locales ---" && \
node common/scripts/minify-common-locales.js
rm -rf ${SRC_PATH}/client/*
EOF

# build plugins
COPY --from=build ${SRC_PATH}/plugins ${SRC_PATH}/plugins
WORKDIR ${SRC_PATH}/buildtools/install/common
COPY ./buildtools/install/common/plugins-build.sh ./plugins-build.sh
RUN echo "--- build/publish plugins ---" && \
    apt-get update && apt-get install -y dos2unix && \
    dos2unix *.sh && \
    bash plugins-build.sh "${SRC_PATH}/plugins"

#----------------------------------
#         Nodejs build server
#----------------------------------
FROM node:22.12.0 AS build-nodejs-server
ARG DEBIAN_FRONTEND
ARG SRC_PATH

# build services Socket, SsoAuth from DocSpace-server 
WORKDIR ${SRC_PATH}/server
COPY ./server/common/ASC.Socket.IO ./common/ASC.Socket.IO
COPY ./server/common/ASC.SsoAuth ./common/ASC.SsoAuth

RUN echo "--- build/publish ASC.Socket.IO ---" && \ 
    cd ${SRC_PATH}/server/common/ASC.Socket.IO &&\
    yarn install --immutable &&\
    echo "--- build/publish ASC.SsoAuth ---" && \ 
    cd ${SRC_PATH}/server/common/ASC.SsoAuth &&\
    yarn install --immutable

#----------------------------------
#          Dotnet build            
#----------------------------------   
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-dotnet
ARG DEBIAN_FRONTEND
ARG SRC_PATH

WORKDIR ${SRC_PATH}/server
COPY ./server/ .

RUN echo "--- build/publishh docspace-server .net 9.0 ---" && \
    dotnet build ASC.Web.slnf && \
    dotnet build ASC.Migrations.sln --property:OutputPath=${SRC_PATH}/publish/services/ASC.Migration.Runner/service/ && \
    dotnet publish ASC.Web.slnf -p PublishProfile=ReleaseProfile && \
    rm -rf ${SRC_PATH}/server/*

#----------------------------------
#       Identity java build           
#----------------------------------
FROM maven:3.9-eclipse-temurin-21 AS java-build
ARG SRC_PATH

WORKDIR ${SRC_PATH}/server/common/ASC.Identity/
COPY ./server/common/ASC.Identity/ .

RUN echo "--- build/publish docspace-server java (ASC.Identity) ---" && \
    mvn -B dependency:go-offline && \
    mvn clean package -B -DskipTests -pl authorization/authorization-container -am && \
    mvn clean package -B -DskipTests -pl registration/registration-container -am

FROM build AS run
#----------------------------------
#       Nginx/Openresty            
#----------------------------------

# Create directories for openresty
RUN rm -rf /var/lib/apt/lists/* && \
    rm -rf /usr/share/nginx/html/* && \
    mkdir -p /var/log/nginx/ && \
    mkdir -p /etc/nginx/ && \
    mkdir -p /var/log/openresty && \
    mkdir -p /etc/openresty/conf.d  && \  
    mkdir -p /etc/nginx/includes/ && \
    cp -f ${SRC_PATH}/buildtools/config/nginx/includes/onlyoffice*.conf /etc/nginx/includes/ && \
    cp -f ${SRC_PATH}/buildtools/config/nginx/includes/server-*.conf /etc/nginx/includes/ && \
    mkdir -p /etc/nginx/conf.d && \
    cp -f ${SRC_PATH}/buildtools/config/nginx/onlyoffice.conf /etc/nginx/conf.d/ && \
    cp -f ${SRC_PATH}/buildtools/config/nginx/onlyoffice-client.conf /etc/nginx/conf.d/ && \
    cp -f ${SRC_PATH}/buildtools/config/nginx/onlyoffice-management.conf /etc/nginx/conf.d/ && \
    cp -f ${SRC_PATH}/buildtools/config/nginx/onlyoffice-story.conf /etc/nginx/conf.d/

# copy static services files and config values 
COPY --from=build-nodejs-client --chown=onlyoffice:onlyoffice ${SRC_PATH}/publish/web/client ${BUILD_PATH}/client
COPY --from=build-nodejs-client --chown=onlyoffice:onlyoffice ${SRC_PATH}/publish/web/public ${BUILD_PATH}/public
COPY --from=build-nodejs-client --chown=onlyoffice:onlyoffice ${SRC_PATH}/publish/web/management ${BUILD_PATH}/management
COPY --from=build-nodejs-client --chown=onlyoffice:onlyoffice ${SRC_PATH}/publish/web/editor ${SRC_PATH}/publish/web/editor
COPY --from=build-nodejs-client --chown=onlyoffice:onlyoffice ${SRC_PATH}/publish/web/login ${SRC_PATH}/publish/web/login
COPY --from=build-nodejs-client --chown=onlyoffice:onlyoffice ${SRC_PATH}/publish/web/sdk ${SRC_PATH}/publish/web/sdk
COPY --from=build-nodejs-client --chown=onlyoffice:onlyoffice ${SRC_PATH}/publish/web/editor/.next/static/chunks ${BUILD_PATH}/build/doceditor/static/chunks
COPY --from=build-nodejs-client --chown=onlyoffice:onlyoffice ${SRC_PATH}/publish/web/editor/.next/static/css ${BUILD_PATH}/build/doceditor/static/css
COPY --from=build-nodejs-client --chown=onlyoffice:onlyoffice ${SRC_PATH}/publish/web/editor/.next/static/media ${BUILD_PATH}/build/doceditor/static/media
COPY --from=build-nodejs-client --chown=onlyoffice:onlyoffice ${SRC_PATH}/publish/web/login/.next/static/chunks ${BUILD_PATH}/build/login/static/chunks
COPY --from=build-nodejs-client --chown=onlyoffice:onlyoffice ${SRC_PATH}/publish/web/login/.next/static/css ${BUILD_PATH}/build/login/static/css
COPY --from=build-nodejs-client --chown=onlyoffice:onlyoffice ${SRC_PATH}/publish/web/login/.next/static/media ${BUILD_PATH}/build/login/static/media
COPY --from=build-nodejs-client --chown=onlyoffice:onlyoffice ${SRC_PATH}/publish/web/sdk/.next/static/chunks ${BUILD_PATH}/build/sdk/static/chunks
COPY --from=build-nodejs-client --chown=onlyoffice:onlyoffice ${SRC_PATH}/publish/web/sdk/.next/static/css ${BUILD_PATH}/build/sdk/static/css
COPY --from=build-nodejs-client --chown=onlyoffice:onlyoffice ${SRC_PATH}/publish/web/sdk/.next/static/media ${BUILD_PATH}/build/sdk/static/media
COPY --from=build-nodejs-client --chown=onlyoffice:onlyoffice ${SRC_PATH}/plugins/publish/ ${APP_STORAGE_ROOT}/Studio/webplugins

COPY --chown=onlyoffice:onlyoffice ./buildtools/install/docker/config/nginx/docker-entrypoint.d /docker-entrypoint.d
COPY --chown=onlyoffice:onlyoffice ./buildtools/config/elastic.test.json /app/onlyoffice/config/elastic.json
COPY --chown=onlyoffice:onlyoffice ./docker/config/nginx/templates/upstream.conf.template /etc/nginx/templates/upstream.conf.template
COPY --chown=onlyoffice:onlyoffice ./buildtools/install/docker/config/nginx/templates/nginx.conf.template /etc/nginx/nginx.conf.template
COPY --chown=onlyoffice:onlyoffice ./buildtools/config/nginx/html /etc/nginx/html
COPY --chown=onlyoffice:onlyoffice ./buildtools/install/docker/prepare-nginx-router.sh /docker-entrypoint.d/prepare-nginx-router.sh
COPY --chown=onlyoffice:onlyoffice ./buildtools/install/docker/config/nginx/docker-entrypoint.sh /nginx/docker-entrypoint.sh

COPY --from=build-dotnet --chown=onlyoffice:onlyoffice ${SRC_PATH}/publish/services ${SRC_PATH}/publish/services
COPY --from=build-nodejs-server --chown=onlyoffice:onlyoffice ${SRC_PATH}/server/common/ASC.Socket.IO ${SRC_PATH}/publish/services/ASC.Socket.IO
COPY --from=build-nodejs-server --chown=onlyoffice:onlyoffice ${SRC_PATH}/server/common/ASC.SsoAuth ${SRC_PATH}/publish/services/ASC.SsoAuth
COPY --from=java-build --chown=onlyoffice:onlyoffice ${SRC_PATH}/server/common/ASC.Identity/authorization/authorization-container/target/*.jar ${SRC_PATH}/publish/services/ASC.Identity.Authorization/app.jar
COPY --from=java-build --chown=onlyoffice:onlyoffice ${SRC_PATH}/server/common/ASC.Identity/registration/registration-container/target/*.jar ${SRC_PATH}/publish/services/ASC.Identity.Registration/app.jar


RUN sed -i 's/$public_root/\/var\/www\/public\//' /etc/nginx/conf.d/onlyoffice.conf && \
    sed -i 's/http:\/\/172.*/$document_server;/' /etc/nginx/conf.d/onlyoffice.conf && \
    sed -i '/client_body_temp_path/ i \ \ \ \ $MAP_HASH_BUCKET_SIZE' /etc/nginx/nginx.conf.template && \
    sed -i 's/\(worker_connections\).*;/\1 $COUNT_WORKER_CONNECTIONS;/' /etc/nginx/nginx.conf.template && \
    sed -i -e '/^user/s/^/#/' -e 's#/tmp/nginx.pid#nginx.pid#' -e 's#/etc/nginx/mime.types#mime.types#' /etc/nginx/nginx.conf.template && \
    mkdir -p ${BUILD_PATH}/public/campaigns && cp -arf ${SRC_PATH}/campaigns/src/campaigns/* ${BUILD_PATH}/public/campaigns/ && \
    rm -rf ${SRC_PATH}/campaigns

RUN chown -R onlyoffice:onlyoffice /etc/nginx/ && \
    chown -R onlyoffice:onlyoffice /var/log/onlyoffice && \
    chown -R onlyoffice:onlyoffice /var/log/nginx/ ${BUILD_PATH}/public/campaigns && \
    chown -R mysql:mysql /var/log/mysql /var/lib/mysql /var/run/mysqld && \
    chmod 600 /var/lib/rabbitmq/.erlang.cookie && \
    chown rabbitmq:rabbitmq /var/log/rabbitmq
    #chown -R onlyoffice:onlyoffice /var/www  

WORKDIR  /
RUN dos2unix /docker-entrypoint.d/* && chmod +x /docker-entrypoint.d/* && \
    dos2unix /nginx/*.sh && chmod +x *.sh && \ 
    dos2unix *.sh && chmod +x *.sh 

EXPOSE 5032 5010 5027 5012 5007 5009 5005 5004 5006 5000 5003 5033 5099 5013 5011 9899 9834 8092 3306 6379 5672 9200 9600 5601 8080 9090 5157

ENTRYPOINT  [ "/docker-entrypoint.sh" ]
