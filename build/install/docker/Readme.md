# Getting Started

* Download App run:	git clone git clone https://github.com/ONLYOFFICE/AppServer.git
* cd ./AppServer/build/install/docker/

# Installation

* Build Appserver docker microservices run: docker-compose -f build.yml build
* In file .env (for checking values run: docker exec -it onlyoffice-community-server env)
  - write value DOCUMENT_SERVER_JWT_SECRET
  - write value APP_CORE_MACHINEKEY
* For running Appserver with Community Server run: docker-compose -f app-server.yml up -d
* For running standlone Appserver run: 
  - docker-compose -f db.yml up -d
  - docker-compose -f document-server.yml -f app-server.yml up -d
