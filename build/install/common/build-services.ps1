$SRC_PATH=(get-item $PSScriptRoot ).parent.parent.parent.FullName
$BUILD_PATH="$SRC_PATH\publish"

$BACKEND_NODEJS_SERVICES="ASC.Socket.IO","ASC.SsoAuth"
$BACKEND_DOTNETCORE_SERVICES="ASC.Files", "ASC.People", "ASC.Data.Backup", "ASC.Files.Service", "ASC.Notify", "ASC.Studio.Notify", "ASC.Web.Api", "ASC.Web.Studio", "ASC.Data.Backup.BackgroundTasks", "ASC.ClearEvents", "ASC.ApiSystem", "ASC.Web.HealthChecks.UI"
$SELF_CONTAINED="false"
$PUBLISH_CNF="Debug"

$FRONTEND_BUILD_ARGS="build"
$FRONTEND_DEPLOY_ARGS="deploy"
$DEBUG_INFO_CHECK=""
$MIGRATION_CHECK="true"
$DOCKER_ENTRYPOINT="$SRC_PATH\build\install\docker\docker-entrypoint.py"

if(Test-Path -Path "$BUILD_PATH\services" ){
  Write-Host "== Clean up services ==" -ForegroundColor Green
  Remove-Item "$BUILD_PATH\services" -Recurse
}

Write-Host "== Build ASC.Web.slnf ==" -ForegroundColor Green
dotnet build "$SRC_PATH\ASC.Web.slnf"

Write-Host "== Build ASC.Migrations.sln ==" -ForegroundColor Green
dotnet build "$SRC_PATH\ASC.Migrations.sln" -o "$BUILD_PATH\services\ASC.Migration.Runner\service\"

Write-Host "== Add docker-migration-entrypoint.sh to ASC.Migration.Runner ==" -ForegroundColor Green
$FilePath = "$BUILD_PATH\services\ASC.Migration.Runner\service\docker-migration-entrypoint.sh"
Get-Content "$SRC_PATH\build\install\docker\docker-migration-entrypoint.sh" -raw | % {$_ -replace "`r", ""} | Set-Content -NoNewline $FilePath

foreach ($SERVICE in $BACKEND_NODEJS_SERVICES)
{
  Write-Host "== Build $SERVICE project ==" -ForegroundColor Green
  yarn install --cwd "$SRC_PATH\common\$SERVICE" --frozen-lockfile

  $DST = "$BUILD_PATH\services\$SERVICE\service\"

  if(!(Test-Path -Path $DST )){
    New-Item -ItemType "directory" -Path $DST 
  }

  Write-Host "== Copy service data to `publish\services\${SERVICE}\service`  ==" -ForegroundColor Green
  Copy-Item -Path "$SRC_PATH\common\$SERVICE\*" -Destination $DST -Recurse
  Write-Host "== Add docker-entrypoint.py to $SERVICE ==" -ForegroundColor Green
  Copy-Item $DOCKER_ENTRYPOINT -Destination $DST
}

Write-Host "== Publish ASC.Web.slnf ==" -ForegroundColor Green
dotnet publish "$SRC_PATH\ASC.Web.slnf" -p "PublishProfile=FolderProfile"

Set-Location -Path $PSScriptRoot

foreach ($SERVICE in $BACKEND_DOTNETCORE_SERVICES)
{
  Write-Host "== Add docker-entrypoint.py to $SERVICE ==" -ForegroundColor Green
  $DST = "$BUILD_PATH\services\$SERVICE\service\"
  Copy-Item $DOCKER_ENTRYPOINT -Destination $DST
}