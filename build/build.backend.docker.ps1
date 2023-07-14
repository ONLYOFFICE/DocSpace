$PSversionMajor = $PSVersionTable.PSVersion | sort-object major | ForEach-Object { $_.major }
$PSversionMinor = $PSVersionTable.PSVersion | sort-object minor | ForEach-Object { $_.minor }

if ($PSversionMajor -lt 7 -or $PSversionMinor -lt 2) {
  Write-Error "Powershell version must be greater than or equal to 7.2."
  exit
}

$RootDir = Split-Path -Parent $PSScriptRoot
$DockerDir = "$RootDir\build\install\docker"
$LocalIp = (Get-CimInstance -ClassName Win32_NetworkAdapterConfiguration | Where-Object { $_.DHCPEnabled -ne $null -and $_.DefaultIPGateway -ne $null }).IPAddress | Select-Object -First 1

$Doceditor = ($LocalIp + ":5013")
$Login = ($LocalIp + ":5011")
$Client = ($LocalIp + ":5001")
$PortalUrl = ("http://" + $LocalIp + ":8092")


# Stop all backend services"
& "$PSScriptRoot\start\stop.backend.docker.ps1"

$Env:COMPOSE_IGNORE_ORPHANS = "True"

Write-Host "Run MySQL" -ForegroundColor Green
docker compose -f "$DockerDir\db.yml" up -d

Write-Host "Build backend services (to `publish/` folder)" -ForegroundColor Green
& "$PSScriptRoot\install\common\build-services.ps1"

Set-Location -Path $DockerDir

Write-Host "Run migration and services" -ForegroundColor Green
$Env:ENV_EXTENSION="dev"
$Env:Baseimage_Dotnet_Run="onlyoffice/4testing-docspace-dotnet-runtime:v1.0.0"
$Env:Baseimage_Nodejs_Run="onlyoffice/4testing-docspace-nodejs-runtime:v1.0.0"
$Env:Baseimage_Proxy_Run="onlyoffice/4testing-docspace-proxy-runtime:v1.0.0"
$Env:DOCUMENT_SERVER_IMAGE_NAME="onlyoffice/documentserver-de:latest"
$Env:SERVICE_DOCEDITOR=$Doceditor
$Env:SERVICE_LOGIN=$Login
$Env:SERVICE_CLIENT=$Client
$Env:ROOT_DIR=$RootDir
$Env:BUILD_PATH="/var/www"
$Env:SRC_PATH="$RootDir\publish\services"
$Env:DATA_DIR="$RootDir\Data"
$Env:APP_URL_PORTAL=$PortalUrl
docker compose -f docspace.profiles.yml -f docspace.overcome.yml --profile migration-runner --profile backend-local up -d

Set-Location -Path $PSScriptRoot