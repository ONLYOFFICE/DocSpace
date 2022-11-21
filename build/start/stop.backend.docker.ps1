$PSversionMajor = $PSVersionTable.PSVersion | sort-object major | ForEach-Object { $_.major }
$PSversionMinor = $PSVersionTable.PSVersion | sort-object minor | ForEach-Object { $_.minor }

if ($PSversionMajor -lt 7 -or $PSversionMinor -lt 2) {
  Write-Error "Powershell version must be greater than or equal to 7.2."
  exit
}

$Containers = docker ps -a -f "name=^onlyoffice" --format="{{.ID}} {{.Names}}" | Select-String -Pattern ("mysql|rabbitmq|redis|elasticsearch|documentserver") -NotMatch | ConvertFrom-String | ForEach-Object P1

if (-not $Containers) {
  Write-Host "No containers to stop" -ForegroundColor Blue
  exit
}

Write-Host "Stop all backend services (containers)" -ForegroundColor Green
docker stop $Containers