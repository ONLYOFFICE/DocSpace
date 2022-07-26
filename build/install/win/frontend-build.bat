@echo off
echo 
echo ######################
echo #   build frontend   #
echo ######################

set DEBUG_INFO=%~2

pushd %~s1

  call yarn install
  if "%DEBUG_INFO%"=="true" yarn debug-info
  call yarn build
  call yarn deploy

popd
