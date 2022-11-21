$RootDir = Split-Path -Parent $PSScriptRoot
$Branch = git branch --show-current
$DockerDir = ($RootDir + "\build\install\docker")
$BuildDate = Get-Date -Format "yyyy-MM-dd"
$LocalIp = (Get-WmiObject -Class Win32_NetworkAdapterConfiguration | Where-Object { $_.DHCPEnabled -ne $null -and $_.DefaultIPGateway -ne $null }).IPAddress | Select-Object -First 1

$Doceditor = ($LocalIp + ":5013")
$Login = ($LocalIp + ":5011")
$Client = ($LocalIp + ":5001")

$DockerFile = "Dockerfile.dev"
$EnvExtension = "dev"
$CoreBaseDomain = "localhost"

# Stop all backend services"
& "$PSScriptRoot\start\stop.backend.docker.ps1"

$Containers = docker ps -a -f "name=^onlyoffice" --format="{{.ID}} {{.Names}}" | Select-String -Pattern ("mysql|rabbitmq|redis|elasticsearch|documentserver") -NotMatch | ConvertFrom-String | ForEach-Object P1
$Images = docker images onlyoffice/docspace*

if ($Containers) {
  Write-Host "Remove all backend containers"
  docker rm -f $Containers
}

if ($Images) {
  Write-Host "Remove all backend images"
  docker rmi -f $Images
    
  Write-Host "Remove all docker images except 'mysql, rabbitmq, redis, elasticsearch, documentserver'"
  docker image rm -f $Images
}

Write-Host "Run MySQL"
docker compose -f  ($DockerDir + "\db.yml") up -d

Write-Host "Run environments (redis, rabbitmq)"
$env:DOCKERFILE = $DockerFile
docker compose -f ($DockerDir + "\redis.yml") -f ($DockerDir + "\rabbitmq.yml") up -d

if ($args[0] -eq "--no_ds") {
  Write-Host "SKIP Document server"
}
else { 
  Write-Host "Run Document server"
  $Env:DOCUMENT_SERVER_IMAGE_NAME = "onlyoffice/documentserver-de:latest"
  $Env:ROOT_DIR = $RootDir
  docker compose -f ($DockerDir + "\ds.dev.yml") up -d
}

Write-Host "Build all backend services"
$Env:DOCKERFILE = $DockerFile
$Env:RELEASE_DATE = $BuildDate
$Env:GIT_BRANCH = $Branch
$Env:SERVICE_DOCEDITOR = $Doceditor
$Env:SERVICE_LOGIN = $Login
$Env:SERVICE_CLIENT = $Client
$Env:APP_CORE_BASE_DOMAIN = $CoreBaseDomain
$Env:ENV_EXTENSION = $EnvExtension
docker compose -f ($DockerDir + "\build.dev.yml") build --build-arg GIT_BRANCH=$Branch --build-arg RELEASE_DATE=$BuildDate

Write-Host "Run DB migration"
$Env:DOCKERFILE = $DockerFile
docker compose -f ($DockerDir + "\migration-runner.yml") up -d

# Start all backend services"
& "$PSScriptRoot\start\start.backend.docker.ps1"