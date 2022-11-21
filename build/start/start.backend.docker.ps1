$RootDir = Split-Path (Split-Path -Parent $PSScriptRoot) -Parent
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

Write-Host "Start all backend services (containers)"

$Env:DOCKERFILE = $DockerFile
$Env:ROOT_DIR = $RootDir
$Env:RELEASE_DATE = $BuildDate
$Env:GIT_BRANCH = $Branch
$Env:SERVICE_DOCEDITOR = $Doceditor
$Env:SERVICE_LOGIN = $Login
$Env:SERVICE_CLIENT = $Client
$Env:APP_CORE_BASE_DOMAIN = $CoreBaseDomain
$Env:APP_URL_PORTAL = ("http://" + $LocalIp + ":8092")
$Env:ENV_EXTENSION = $EnvExtension

docker compose -f ($DockerDir + "\docspace.dev.yml") up -d