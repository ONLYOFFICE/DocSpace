@echo off
echo 
echo #####################
echo #   build backend   #
echo #####################

set SRC_PATH=%~s2

pushd %~1

  call dotnet build ASC.Web.slnf
  call dotnet build ASC.Migrations.sln --property:OutputPath=%SRC_PATH%\services\ASC.Migration.Runner\service

  echo "== Build ASC.Socket.IO =="
  pushd common\ASC.Socket.IO
    call yarn install --frozen-lockfile
  popd

  echo "== Build ASC.SsoAuth =="
  pushd common\ASC.SsoAuth
    call yarn install --frozen-lockfile
  popd

popd
