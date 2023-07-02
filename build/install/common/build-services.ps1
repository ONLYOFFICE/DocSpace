$PROJECT_REPOSITORY_NAME="DocSpace"
$SRC_PATH=(get-item $PSScriptRoot ).parent.parent.parent.FullName
$BUILD_PATH="$SRC_PATH\publish"
$BUILD_DOTNET_CORE_ARGS="false"
$PROPERTY_BUILD="backend-publish"

$BACKEND_NODEJS_SERVICES="ASC.Socket.IO","ASC.SsoAuth"
$BACKEND_DOTNETCORE_SERVICES="ASC.Files", "ASC.People", "ASC.Data.Backup", "ASC.Files.Service", "ASC.Notify", "ASC.Studio.Notify", "ASC.Web.Api", "ASC.Web.Studio", "ASC.Data.Backup.BackgroundTasks", "ASC.ClearEvents", "ASC.ApiSystem", "ASC.Web.HealthChecks.UI"
$SELF_CONTAINED="false"
$PUBLISH_BACKEND_ARGS="false"
$PUBLISH_CNF="Debug"

$FRONTEND_BUILD_ARGS="build"
$FRONTEND_DEPLOY_ARGS="deploy"
$DEBUG_INFO_CHECK=""
$MIGRATION_CHECK="true"
$DOCKER_ENTRYPOINT="$SRC_PATH\build\install\docker\docker-entrypoint.py"

#$ARRAY_NAME_SERVICES=()

Remove-Item "$BUILD_PATH\services" -Recurse

foreach ($SERVICE in $BACKEND_NODEJS_SERVICES)
{
  Write-Host "== Build $SERVICE project =="
  yarn install --cwd "$SRC_PATH\common\$SERVICE" --frozen-lockfile

  $DST = "$BUILD_PATH\services\$SERVICE\service\"
  New-Item -ItemType "directory" -Path $DST 
  Copy-Item -Path "$SRC_PATH\common\$SERVICE\*" -Destination $DST -Recurse
  Copy-Item $DOCKER_ENTRYPOINT -Destination $DST
}

dotnet build "$SRC_PATH\ASC.Web.slnf"
dotnet publish "$SRC_PATH\ASC.Web.slnf" -p "PublishProfile=FolderProfileWindows"

Set-Location -Path "$SRC_PATH\web\ASC.Web.Api"
dotnet publish -c "Debug" --self-contained "false" -o "$BUILD_PATH\services\ASC.Web.Api\service\"

foreach ($SERVICE in $BACKEND_DOTNETCORE_SERVICES)
{
  $DST = "$BUILD_PATH\services\$SERVICE\service\"
  Copy-Item $DOCKER_ENTRYPOINT -Destination $DST
}

dotnet build "$SRC_PATH\ASC.Migrations.sln" -o "$BUILD_PATH\services\ASC.Migration.Runner\service\"

$FilePath = "$BUILD_PATH\services\ASC.Migration.Runner\service\docker-migration-entrypoint.sh"
Get-Content "$SRC_PATH\build\install\docker\docker-migration-entrypoint.sh" -raw | % {$_ -replace "`r", ""} | Set-Content -NoNewline $FilePath

Set-Location -Path $PSScriptRoot