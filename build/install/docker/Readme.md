# Getting Started

* Download App run:	git clone git clone https://github.com/ONLYOFFICE/AppServer.git
* cd ./AppServer/build/install/docker/

# Installation

* Build Appserver docker microservices run: docker-compose -f build.yml build

* Check JSON Web Token validation
  - In file .env:
    - write value DOCUMENT_SERVER_JWT_SECRET
    - write value APP_CORE_MACHINEKEY

* Run Appserver with Community Server:  
  - check file app-server.yml before running: 
    - app_data:/app/onlyoffice/data should be commented,
    - /app/onlyoffice CommunityServer/data:/app/onlyoffice/data should be uncommented
  - docker-compose -f app-server.yml up -d

* Run standlone Appserver: 
  - check file app-server.yml before running: 
    - app_data:/app/onlyoffice/data should be uncommented,
    - /app/onlyoffice CommunityServer/data:/app/onlyoffice/data should be commented
  - docker-compose -f db.yml up -d
  - docker-compose -f document-server.yml -f app-server.yml up -d
