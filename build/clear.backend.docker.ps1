$Containers = docker ps -aqf "name=^onlyoffice"
$RootDir = Split-Path -Parent $PSScriptRoot
$DockerDir = ($RootDir + "\build\install\docker")

Write-Host "Clean up containers, volumes or networks" -ForegroundColor Green

if ($Containers) {
  Write-Host "Remove all backend containers" -ForegroundColor Blue

  $Env:DOCUMENT_SERVER_IMAGE_NAME="onlyoffice/documentserver-de:latest"
  $Env:Baseimage_Dotnet_Run="onlyoffice/4testing-docspace-dotnet-runtime:v1.0.0"
  $Env:Baseimage_Nodejs_Run="onlyoffice/4testing-docspace-nodejs-runtime:v1.0.0"
  $Env:Baseimage_Proxy_Run="onlyoffice/4testing-docspace-proxy-runtime:v1.0.0"
  $Env:SERVICE_CLIENT="localhost:5001"
  $Env:BUILD_PATH="/var/www"
  $Env:SRC_PATH="$RootDir\publish\services"
  $Env:ROOT_DIR=$RootDir
  $Env:DATA_DIR="$RootDir\Data"

  docker compose -f "$DockerDir\docspace.profiles.yml" -f "$DockerDir\docspace.overcome.yml" --profile "migration-runner" --profile "backend-local" down --volumes

  Write-Host "Remove docker contatiners 'mysql'" -ForegroundColor Blue
  docker compose -f "$DockerDir\db.yml" down --volumes

  Write-Host "Remove docker volumes" -ForegroundColor Blue
  docker volume prune -f -a

  Write-Host "Remove docker networks" -ForegroundColor Blue
  docker network prune -f
}
else { 
  Write-Host "No containers, images, volumes or networks to clean up" -ForegroundColor Green
}