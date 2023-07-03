$Containers = docker ps -aqf "name=^onlyoffice"
$Images = docker images onlyoffice/docspace* -q
$RootDir = Split-Path -Parent $PSScriptRoot
$DockerDir = ($RootDir + "\build\install\docker")
$LocalIp = (Get-CimInstance -ClassName Win32_NetworkAdapterConfiguration | Where-Object { $_.DHCPEnabled -ne $null -and $_.DefaultIPGateway -ne $null }).IPAddress | Select-Object -First 1

Write-Host "Clean up containers, images, volumes or networks" -ForegroundColor Green

if ($Containers -or $Images) {
  Write-Host "Remove all backend containers" -ForegroundColor Blue

  $Env:DOCUMENT_SERVER_IMAGE_NAME="onlyoffice/documentserver-de:latest"
  $Env:Baseimage_Dotnet_Run="onlyoffice/4testing-docspace-dotnet-runtime:v1.0.0"
  $Env:Baseimage_Nodejs_Run="onlyoffice/4testing-docspace-nodejs-runtime:v1.0.0"
  $Env:Baseimage_Proxy_Run="onlyoffice/4testing-docspace-proxy-runtime:v1.0.0"
  $Env:SERVICE_CLIENT=($LocalIp + ":5001")
  $Env:BUILD_PATH="/var/www"
  $Env:SRC_PATH="$RootDir\publish\services"
  $Env:ROOT_DIR=$RootDir
  $Env:DATA_DIR="$RootDir\Data"

  docker compose -f "$DockerDir\docspace.profiles.yml" -f "$DockerDir\docspace.overcome.yml" --profile "migration-runner" down --volumes
  docker compose -f "$DockerDir\docspace.profiles.yml" -f "$DockerDir\docspace.overcome.yml" --profile "backend-local" down --volumes

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