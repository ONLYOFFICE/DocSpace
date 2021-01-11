# Getting Started

* Download App run:	git clone git clone https://github.com/ONLYOFFICE/AppServer.git
* cd ./AppServer/build/install/docker/

# Installation

* Build Appserver docker microservices run: docker-compose -f build.yml build

*  In file .env check values and if it needs modify for JSON Web Token validation:
  - list of values:
    - DOCUMENT_SERVER_JWT_SECRET
    - APP_CORE_MACHINEKEY
    - DOCUMENT_SERVER_JWT_HEADER

* Run Appserver with Community Server:  
  - check file appserver.yml before running: 
    - app_data:/app/onlyoffice/data should be commented,
    - /app/onlyoffice CommunityServer/data:/app/onlyoffice/data should be uncommented
  - docker-compose -f appserver.yml up -d

* Run standlone Appserver: 
  - check file appserver.yml before running: 
    - app_data:/app/onlyoffice/data should be uncommented,
    - /app/onlyoffice CommunityServer/data:/app/onlyoffice/data should be commented
  - docker-compose -f db.yml up -d
  - docker-compose -f ds.yml -f appserver.yml up -d
