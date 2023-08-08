$PSversionMajor = $PSVersionTable.PSVersion | sort-object major | ForEach-Object { $_.major }
$PSversionMinor = $PSVersionTable.PSVersion | sort-object minor | ForEach-Object { $_.minor }

if ($PSversionMajor -lt 7 -or $PSversionMinor -lt 2) {
  Write-Error "Powershell version must be greater than or equal to 7.2."
  exit
}

$RootDir = Split-Path (Split-Path -Parent $PSScriptRoot) -Parent
$DockerDir = ($RootDir + "\build\install\docker")
$LocalIp = (Get-CimInstance -ClassName Win32_NetworkAdapterConfiguration | Where-Object { $_.DHCPEnabled -ne $null -and $_.DefaultIPGateway -ne $null }).IPAddress | Select-Object -First 1

$Doceditor = ($LocalIp + ":5013")
$Login = ($LocalIp + ":5011")
$Client = ($LocalIp + ":5001")

Set-Location -Path $DockerDir

Write-Host "Start all services (containers)" -ForegroundColor Green
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
docker compose -f docspace.profiles.yml -f docspace.overcome.yml --profile migration-runner --profile backend-local start