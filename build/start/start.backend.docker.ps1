$PSversionMajor = $PSVersionTable.PSVersion | sort-object major | ForEach-Object { $_.major }
$PSversionMinor = $PSVersionTable.PSVersion | sort-object minor | ForEach-Object { $_.minor }

if ($PSversionMajor -lt 7 -or $PSversionMinor -lt 2) {
  Write-Error "Powershell version must be greater than or equal to 7.2."
  exit
}

$Branch = git branch --show-current
$BranchExistRemote = git ls-remote --heads origin $Branch

if (-not $BranchExistRemote) {
  Write-Error "The current branch does not exist in the remote repository. Please push changes."
  exit
}

$RootDir = Split-Path (Split-Path -Parent $PSScriptRoot) -Parent
$DockerDir = ($RootDir + "\build\install\docker")
$BuildDate = Get-Date -Format "yyyy-MM-dd"
$LocalIp = (Get-CimInstance -ClassName Win32_NetworkAdapterConfiguration | Where-Object { $_.DHCPEnabled -ne $null -and $_.DefaultIPGateway -ne $null }).IPAddress | Select-Object -First 1

$Doceditor = ($LocalIp + ":5013")
$Login = ($LocalIp + ":5011")
$Client = ($LocalIp + ":5001")

$DockerFile = "Dockerfile.dev"
$EnvExtension = "dev"
$CoreBaseDomain = "localhost"

Write-Host "Start all backend services (containers)" -ForegroundColor Green
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