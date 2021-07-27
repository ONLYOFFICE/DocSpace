@echo off
echo 
echo ######################
echo #   build frontend   #
echo ######################

pushd %~1

call yarn install
call yarn build

popd