Write-Host "Stop all backend services (containers)"

$Containers = docker ps -a -f "name=^onlyoffice" --format="{{.ID}} {{.Names}}" | Select-String -Pattern ("mysql|rabbitmq|redis|elasticsearch|documentserver") -NotMatch | ConvertFrom-String | ForEach-Object P1

docker stop $Containers