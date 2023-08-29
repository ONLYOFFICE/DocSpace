$Containers = docker ps -aqf "name=^onlyoffice"
$Images = docker images onlyoffice/4testing-docspace* -q
$RootDir = Split-Path -Parent $PSScriptRoot
$DockerDir = ($RootDir + "\build\install\docker")

Write-Host "Clean up containers, volumes or networks" -ForegroundColor Green

if ($Containers -or $Images) {
  Write-Host "Remove all backend containers" -ForegroundColor Blue

  $Env:DOCUMENT_SERVER_IMAGE_NAME="onlyoffice/documentserver-de:latest"
  $Env:Baseimage_Dotnet_Run="onlyoffice/4testing-docspace-dotnet-runtime:dev"
  $Env:Baseimage_Nodejs_Run="onlyoffice/4testing-docspace-nodejs-runtime:dev"
  $Env:Baseimage_Proxy_Run="onlyoffice/4testing-docspace-proxy-runtime:dev"
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

  Write-Host "Remove docker base images (onlyoffice/4testing-docspace)" -ForegroundColor Blue
  docker rmi -f $Images

  Write-Host "Remove docker networks" -ForegroundColor Blue
  docker network prune -f
}
else { 
  Write-Host "No containers, images, volumes or networks to clean up" -ForegroundColor Green
}