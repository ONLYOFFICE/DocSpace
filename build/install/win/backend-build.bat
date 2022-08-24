@echo off
echo 
echo #####################
echo #   build backend   #
echo #####################

pushd %~1

  call dotnet build ASC.Web.sln

  echo "== Build ASC.Thumbnails =="
  pushd common\ASC.Thumbnails
    call yarn install --frozen-lockfile
  popd

  echo "== Build ASC.UrlShortener =="
  pushd common\ASC.UrlShortener
    call yarn install --frozen-lockfile
  popd

  echo "== Build ASC.Socket.IO =="
  pushd common\ASC.Socket.IO
    call yarn install --frozen-lockfile
  popd

  echo "== Build ASC.SsoAuth =="
  pushd common\ASC.SsoAuth
    call yarn install --frozen-lockfile
  popd

popd
