$Containers = docker ps -aqf "name=^onlyoffice"
$Images = docker images onlyoffice/docspace* -q

if ($Containers) {
  Write-Host "Stop all backend containers" -ForegroundColor Blue
  docker stop $Containers

  Write-Host "Remove all backend containers" -ForegroundColor Blue
  docker rm -f $Containers
}

if ($Images) {
  Write-Host "Remove all docker images except 'mysql, rabbitmq, redis, elasticsearch, documentserver'" -ForegroundColor Blue
  docker rmi -f $Images
}

Write-Host "Remove unused volumes." -ForegroundColor Blue
docker volume prune -f

Write-Host "Remove unused networks." -ForegroundColor Blue
docker network prune -f