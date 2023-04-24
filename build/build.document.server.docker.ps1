$PSversionMajor = $PSVersionTable.PSVersion | sort-object major | ForEach-Object { $_.major }
$PSversionMinor = $PSVersionTable.PSVersion | sort-object minor | ForEach-Object { $_.minor }
  
if ($PSversionMajor -lt 7 -or $PSversionMinor -lt 2) {
  Write-Error "Powershell version must be greater than or equal to 7.2."
  exit
}

$RootDir = Split-Path -Parent $PSScriptRoot

Write-Host "Run Document server" -ForegroundColor Green
$DOCUMENT_SERVER_IMAGE_NAME = "onlyoffice/documentserver-de:latest"


docker run -i -t -d -p 8085:80 -e JWT_ENABLED=true -e JWT_SECRET=secret -e JWT_HEADER=AuthorizationJwt --restart=always -v $RootDir/Data:/var/www/onlyoffice/Data $DOCUMENT_SERVER_IMAGE_NAME