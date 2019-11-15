FROM ubuntu:18.04

ARG RELEASE_DATE="2016-06-21"
ARG RELEASE_DATE_SIGN=""
ARG VERSION="8.9.0.190"
ARG SOURCE_REPO_URL="deb http://static.teamlab.com.s3.amazonaws.com/repo/debian squeeze main"
ARG DEBIAN_FRONTEND=noninteractive

LABEL onlyoffice.community.release-date="${RELEASE_DATE}" \
      onlyoffice.community.version="${VERSION}" \
      maintainer="Ascensio System SIA <support@onlyoffice.com>"

ENV LANG=en_US.UTF-8 \
    LANGUAGE=en_US:en \
    LC_ALL=en_US.UTF-8

RUN apt-get -y update && \
    apt-get -yq install gnupg2 ca-certificates && \
    apt-get install -yq sudo locales && \
    addgroup --system --gid 107 onlyoffice && \
    adduser -uid 104 --quiet --home /var/www/onlyoffice --system --gid 107 onlyoffice && \
    addgroup --system --gid 104 elasticsearch && \
    adduser -uid 103 --quiet --home /nonexistent --system --gid 104 elasticsearch && \
    locale-gen en_US.UTF-8 && \
    apt-get -y update && \
    apt-get install -yq software-properties-common wget curl cron rsyslog && \
    wget http://nginx.org/keys/nginx_signing.key && \
    apt-key add nginx_signing.key && \
    echo "deb http://nginx.org/packages/mainline/ubuntu/ bionic nginx" >> /etc/apt/sources.list.d/nginx.list && \
    echo "deb-src http://nginx.org/packages/mainline/ubuntu/ bionic nginx" >> /etc/apt/sources.list.d/nginx.list && \
    apt-get install -yq openjdk-8-jre-headless && \
    wget -qO - https://artifacts.elastic.co/GPG-KEY-elasticsearch | apt-key add - && \
    apt-get install -yq apt-transport-https && \
    apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF && \
    echo "deb https://download.mono-project.com/repo/ubuntu stable-bionic main" | tee /etc/apt/sources.list.d/mono-official.list && \
    echo "deb https://artifacts.elastic.co/packages/6.x/apt stable main" | tee -a /etc/apt/sources.list.d/elastic-6.x.list && \
    apt-get -y update && \
    apt-get install -yq elasticsearch=6.5.0 && \
    add-apt-repository -y ppa:certbot/certbot && \
    add-apt-repository -y ppa:jonathonf/ffmpeg-4 && \
    curl -sL https://deb.nodesource.com/setup_12.x | sudo -E bash - && \
    apt-get install -y nodejs && \
    curl -sS https://dl.yarnpkg.com/debian/pubkey.gpg | sudo apt-key add - && \
    echo "deb https://dl.yarnpkg.com/debian/ stable main" | sudo tee /etc/apt/sources.list.d/yarn.list && \
    wget -q https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb && \
    dpkg -i packages-microsoft-prod.deb && \
    apt-get -y update && \
    apt-get install -yq nginx && \
    cd ~ && \
    wget http://www-us.apache.org/dist/kafka/2.2.1/kafka_2.12-2.2.1.tgz && \
    tar xzf kafka_2.12-2.2.1.tgz && \
    rm kafka_2.12-2.2.1.tgz && \
    echo "#!/bin/sh\nexit 0" > /usr/sbin/policy-rc.d && \
    apt-get install -yq libgdiplus \
                        python-certbot-nginx \
                        htop \
                        nano \
                        dnsutils \
                        python3-pip \
                        multiarch-support \ 
                        iproute2 \
                        ffmpeg \
                        jq \ 
                        git \ 
                        yarn \
                        dotnet-sdk-3.0 \
                        supervisor \
                        mysql-client \
                        mysql-server

RUN git clone https://github.com/ONLYOFFICE/AppServer.git /app/onlyoffice/src/

RUN cd /app/onlyoffice/src/ && \
    yarn install --cwd web/ASC.Web.Components --frozen-lockfile > build/ASC.Web.Components.log && \
    npm run build --prefix web/ASC.Web.Components && \
    yarn pack --cwd web/ASC.Web.Components

RUN cd /app/onlyoffice/src/ && \
    npm run build:storybook --prefix web/ASC.Web.Components && \
    mkdir -p /var/www/story/ && \
    cp -Rf web/ASC.Web.Components/storybook-static/* /var/www/story/

RUN cd /app/onlyoffice/src/ && \
    component=$(ls web/ASC.Web.Components/asc-web-components-v1.*.tgz) && \
    yarn add ../../$component --cwd web/ASC.Web.Client --cache-folder ../../yarn && \
    yarn install --cwd web/ASC.Web.Client --frozen-lockfile || (cd web/ASC.Web.Client && npm i && cd ../../) && \
    npm run build --prefix web/ASC.Web.Client && \
    rm -rf /var/www/studio/client/* && \
    mkdir -p /var/www/studio/client && \
    cp -rf web/ASC.Web.Client/build/* /var/www/studio/client

RUN cd /app/onlyoffice/src/ && \
    component=$(ls  web/ASC.Web.Components/asc-web-components-v1.*.tgz) && \
    yarn add ../../../$component --cwd products/ASC.People/Client --cache-folder ../../../yarn && \
    yarn install --cwd products/ASC.People/Client --frozen-lockfile || (cd products/ASC.People/Client && npm i && cd ../../../) && \
    npm run build --prefix products/ASC.People/Client && \
    mkdir -p /var/www/products/ASC.People/client && \
    cp -Rf products/ASC.People/Client/build/* /var/www/products/ASC.People/client && \
    mkdir -p /var/www/products/ASC.People/client/products/people

RUN cd /app/onlyoffice/src/ && \
    rm -f /etc/nginx/conf.d/* && \
    cp -rf config/nginx/onlyoffice*.conf /etc/nginx/conf.d/ && \
    mkdir -p /app/onlyoffice/config/ && cp -rf config/* /app/onlyoffice/config/ && \
    sed -e 's/#//' -i /etc/nginx/conf.d/onlyoffice.conf && \
    cd products/ASC.People/Server && \
    dotnet -d publish -o /var/www/products/ASC.People/server && \
    cd ../../../ && \
    cd web/ASC.Web.Api && \
    dotnet -d publish -o /var/www/studio/api && \
    cd ../../ && \
    cd web/ASC.Web.Studio && \
    dotnet -d publish -o /var/www/studio/server && \
    cd ../../ && \
    cd common/services/ASC.Notify && \
    dotnet -d publish -o /var/www/services/notify && \
    cd ../../../ && \
    cd common/services/ASC.Studio.Notify && \
    dotnet -d publish -o /var/www/services/studio.notify

COPY config/mysql/conf.d/mysql.cnf /etc/mysql/conf.d/mysql.cnf
COPY config/supervisor/supervisord.conf /etc/supervisor/conf.d/supervisord.conf

RUN sed -i 's/172.18.0.5/localhost/' /app/onlyoffice/config/appsettings.test.json

RUN mkdir -p /var/mysqld/ && \
    chown -R mysql:mysql /var/lib/mysql /var/run/mysqld /var/mysqld/ && \
    service mysql start && \
    mysql -e "CREATE DATABASE IF NOT EXISTS onlyoffice CHARACTER SET utf8 COLLATE 'utf8_general_ci'" && \
    mysql -D "onlyoffice" < /app/onlyoffice/src/sql/app.sql && \
    mysql -D "onlyoffice" < /app/onlyoffice/src/sql/app.data.sql && \
    mysql -D "onlyoffice" -e 'CREATE USER IF NOT EXISTS "onlyoffice_user"@"localhost" IDENTIFIED WITH mysql_native_password BY "onlyoffice_pass";' && \
    mysql -D "onlyoffice" -e 'GRANT ALL PRIVILEGES ON *.* TO 'onlyoffice_user'@'localhost' IDENTIFIED BY "onlyoffice_pass";' && \
    mysql -D "onlyoffice" -e 'UPDATE core_user SET email = "paul.bannov@gmail.com";' && \
    mysql -D "onlyoffice" -e 'UPDATE core_usersecurity SET pwdhash = "vLFfghR5tNV3K9DKhmwArV+SbjWAcgZZzIDTnJ0JgCo=", pwdhashsha512 = "USubvPlB+ogq0Q1trcSupg==";' && \
    service mysql stop

RUN rm -rf /var/lib/apt/lists/*

VOLUME /var/lib/mysql

EXPOSE 80 443 8092 8081

ENTRYPOINT ["/usr/bin/supervisord", "--"]
