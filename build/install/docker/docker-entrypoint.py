import json, sys, os
from jsonpath_ng import jsonpath, parse
from os import environ
from multipledispatch import dispatch

filePath = None
saveFilePath = None
jsonValue = None

PRODUCT = os.environ["PRODUCT"] if environ.get("PRODUCT") else "onlyoffice"
BASE_DIR =  os.environ["BASE_DIR"] if environ.get("BASE_DIR") else  "/app/" + PRODUCT
ENV_EXTENSION = os.environ["ENV_EXTENSION"] if environ.get("ENV_EXTENSION") else "none"
PROXY_HOST = os.environ["PROXY_HOST"] if environ.get("PROXY_HOST") else "proxy"
SERVICE_PORT = os.environ["SERVICE_PORT"] if environ.get("SERVICE_PORT") else "5050"
URLS = os.environ["URLS"] if environ.get("URLS") else "http://0.0.0.0:"
PATH_TO_CONF = os.environ["PATH_TO_CONF"] if environ.get("PATH_TO_CONF") else "/app/" + PRODUCT + "/config"
LOG_DIR = os.environ["LOG_DIR"] if environ.get("LOG_DIR") else "/var/log/" + PRODUCT
ROUTER_HOST = os.environ["ROUTER_HOST"] if environ.get("ROUTER_HOST") else "localhost"

MYSQL_HOST = os.environ["MYSQL_HOST"] if environ.get("MYSQL_HOST") else "localhost"
MYSQL_DATABASE = os.environ["MYSQL_DATABASE"] if environ.get("MYSQL_DATABASE") else "onlyoffice"
MYSQL_USER = os.environ["MYSQL_USER"] if environ.get("MYSQL_USER") else "onlyoffice_user"
MYSQL_PASSWORD = os.environ["MYSQL_PASSWORD"] if environ.get("MYSQL_PASSWORD") else "onlyoffice_pass"
DATABASE_MIGRATION = os.environ["DATABASE_MIGRATION"] if environ.get("DATABASE_MIGRATION") else "false"

APP_CORE_BASE_DOMAIN = os.environ["APP_CORE_BASE_DOMAIN"] if environ.get("APP_CORE_BASE_DOMAIN") else "localhost"
APP_CORE_MACHINEKEY = os.environ["APP_CORE_MACHINEKEY"] if environ.get("APP_CORE_MACHINEKEY") else "your_core_machinekey"
APP_URL_PORTAL = os.environ["APP_URL_PORTAL"] if environ.get("APP_URL_PORTAL") else "http://" + ROUTER_HOST + ":8092"
APP_STORAGE_ROOT = os.environ["APP_STORAGE_ROOT"] if environ.get("APP_STORAGE_ROOT") else BASE_DIR + "/data/"

DOCUMENT_SERVER_JWT_SECRET = os.environ["DOCUMENT_SERVER_JWT_SECRET"] if environ.get("DOCUMENT_SERVER_JWT_SECRET") else "your_jwt_secret"
DOCUMENT_SERVER_JWT_HEADER = os.environ["DOCUMENT_SERVER_JWT_HEADER"] if environ.get("DOCUMENT_SERVER_JWT_HEADER") else "AuthorizationJwt"
DOCUMENT_SERVER_URL_PUBLIC = os.environ["DOCUMENT_SERVER_URL_PUBLIC"] if environ.get("DOCUMENT_SERVER_URL_PUBLIC") else "/ds-vpath/"
DOCUMENT_SERVER_URL_INTERNAL = os.environ["DOCUMENT_SERVER_URL_INTERNAL"] if environ.get("DOCUMENT_SERVER_URL_INTERNAL") else "http://onlyoffice-document-server/"

ELK_SHEME = os.environ["ELK_SHEME"] if environ.get("ELK_SHEME") else "http"
ELK_HOST = os.environ["ELK_HOST"] if environ.get("ELK_HOST") else "onlyoffice-elasticsearch"
ELK_PORT = os.environ["ELK_PORT"] if environ.get("ELK_PORT") else "9200"
ELK_THREADS = os.environ["ELK_THREADS"] if environ.get("ELK_THREADS") else "1"

KAFKA_HOST = os.environ["KAFKA_HOST"] if environ.get("KAFKA_HOST") else "kafka:9092"
RUN_FILE = sys.argv[1] if sys.argv[1] else "none"
LOG_FILE = sys.argv[2] if sys.argv[2] else "none"

REDIS_HOST=os.environ["REDIS_HOST"] if environ.get("REDIS_HOST") else "onlyoffice-redis"
RABBIT_HOST=os.environ["RABBIT_HOST"] if environ.get("RABBIT_HOST") else "onlyoffice-rabbitmq"

class RunServices:
    def __init__(self, SERVICE_PORT, PATH_TO_CONF):
        self.SERVICE_PORT = SERVICE_PORT
        self.PATH_TO_CONF = PATH_TO_CONF
    @dispatch(str)    
    def RunService(self, RUN_FILE):
        os.system("node " + RUN_FILE + " --app.port=" + self.SERVICE_PORT +\
             " --app.appsettings=" + self.PATH_TO_CONF)
        return 1
        
    @dispatch(str, str)
    def RunService(self, RUN_FILE, ENV_EXTENSION):
        if ENV_EXTENSION == "none":
            self.RunService(RUN_FILE)
        os.system("node " + RUN_FILE + " --app.port=" + self.SERVICE_PORT +\
             " --app.appsettings=" + self.PATH_TO_CONF +\
                " --app.environment=" + ENV_EXTENSION)
        return 1

    @dispatch(str, str, str)
    def RunService(self, RUN_FILE, ENV_EXTENSION, LOG_FILE):
        data = RUN_FILE.split(".")
        if data[-1] != "dll":
            self.RunService(RUN_FILE, ENV_EXTENSION)
        elif  ENV_EXTENSION == "none":
            os.system("dotnet " + RUN_FILE + " --urls=" + URLS + self.SERVICE_PORT +\
                " --\'$STORAGE_ROOT\'=" + APP_STORAGE_ROOT +\
                    " --pathToConf=" + self.PATH_TO_CONF +\
                        " --log:dir=" + LOG_DIR +\
                            " --log:name=" + LOG_FILE +\
                                " core:products:folder=/var/www/products/" +\
                                    " core:products:subfolder=server")
        else:
            os.system("dotnet " + RUN_FILE + " --urls=" + URLS + self.SERVICE_PORT +\
                 " --\'$STORAGE_ROOT\'=" + APP_STORAGE_ROOT +\
                    " --pathToConf=" + self.PATH_TO_CONF +\
                        " --log:dir=" + LOG_DIR +\
                            " --log:name=" + LOG_FILE +\
                                " --ENVIRONMENT=" + ENV_EXTENSION +\
                                    " core:products:folder=/var/www/products/" +\
                                        " core:products:subfolder=server")

def openJsonFile(filePath):
    try:
        with open(filePath, 'r') as f:
            return json.load(f)
    except FileNotFoundError as e:
        return False
    except IOError as e:
        return False

def parseJsonValue(jsonValue):
    data = jsonValue.split("=")
    data[0] = "$." + data[0].strip()
    data[1] = data[1].replace(" ", "")
    
    return data

def updateJsonData(jsonData, jsonKey, jsonUpdateValue):
    jsonpath_expr = parse(jsonKey)
    jsonpath_expr.find(jsonData)
    jsonpath_expr.update(jsonData, jsonUpdateValue)
    
    return jsonData

def writeJsonFile(jsonFile, jsonData, indent=4):
    with open(jsonFile, 'w') as f:
        f.write(json.dumps(jsonData, ensure_ascii=False, indent=indent))
    
    return 1

#filePath = sys.argv[1]
saveFilePath = filePath
#jsonValue = sys.argv[2]

filePath = "/app/onlyoffice/config/appsettings.json"
jsonData = openJsonFile(filePath)
#jsonUpdateValue = parseJsonValue(jsonValue)
updateJsonData(jsonData, "$.ConnectionStrings.default.connectionString", "Server="+ MYSQL_HOST +";Port=3306;Database="+ MYSQL_DATABASE +";User ID="+ MYSQL_USER +";Password="+ MYSQL_PASSWORD +";Pooling=true;Character Set=utf8;AutoEnlist=false;SSL Mode=none;ConnectionReset=false",)
updateJsonData(jsonData,"$.core.base-domain", APP_CORE_BASE_DOMAIN)
updateJsonData(jsonData,"$.core.machinekey", APP_CORE_MACHINEKEY)
updateJsonData(jsonData,"$.core.products.subfolder", "server")
updateJsonData(jsonData,"$.web.hub.internal", "http://onlyoffice-socket:" + SERVICE_PORT + "/")
updateJsonData(jsonData,"$.web.url-shortener.internal", "http://onlyoffice-urlshortener:" + SERVICE_PORT + "/")
updateJsonData(jsonData,"$.files.docservice.url.portal", APP_URL_PORTAL)
updateJsonData(jsonData,"$.files.docservice.url.public", DOCUMENT_SERVER_URL_PUBLIC)
updateJsonData(jsonData,"$.files.docservice.url.internal", DOCUMENT_SERVER_URL_INTERNAL)
updateJsonData(jsonData,"$.files.docservice.secret.value", DOCUMENT_SERVER_JWT_SECRET)
updateJsonData(jsonData,"$.files.docservice.secret.header", DOCUMENT_SERVER_JWT_HEADER)
updateJsonData(jsonData,"$.migration.enabled", DATABASE_MIGRATION)
writeJsonFile(filePath, jsonData)

filePath = "/app/onlyoffice/config/elastic.json"
jsonData = openJsonFile(filePath)
updateJsonData(jsonData,"$.elastic.Scheme", ELK_SHEME)
updateJsonData(jsonData,"$.elastic.Host", ELK_HOST)
updateJsonData(jsonData,"$.elastic.Port", ELK_PORT)
updateJsonData(jsonData,"$.elastic.Threads", ELK_THREADS)
writeJsonFile(filePath, jsonData)

filePath = "/app/onlyoffice/config/kafka.json"
jsonData = openJsonFile(filePath)
jsonData.update({"kafka": {"BootstrapServers": KAFKA_HOST}})
writeJsonFile(filePath, jsonData)

filePath = "/app/onlyoffice/config/socket.json"
jsonData = openJsonFile(filePath)
updateJsonData(jsonData,"$.socket.port", SERVICE_PORT)
writeJsonFile(filePath, jsonData)

filePath = "/app/onlyoffice/config/ssoauth.json"
jsonData = openJsonFile(filePath)
updateJsonData(jsonData,"$.ssoauth.port", SERVICE_PORT)
writeJsonFile(filePath, jsonData)

filePath = "/app/onlyoffice/config/rabbitmq.json"
jsonData = openJsonFile(filePath)
updateJsonData(jsonData,"$.RabbitMQ.Hostname", RABBIT_HOST)
writeJsonFile(filePath, jsonData)

filePath = "/app/onlyoffice/config/redis.json"
jsonData = openJsonFile(filePath)
updateJsonData(jsonData,"$.Redis.Hosts.[0].Host", REDIS_HOST)
writeJsonFile(filePath, jsonData)

run = RunServices(SERVICE_PORT, PATH_TO_CONF)
run.RunService(RUN_FILE, ENV_EXTENSION, LOG_FILE)