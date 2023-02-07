PUSHD %~dp0..

cd %~dp0../../common/ASC.WebPlugins/

call yarn install --immutable

call yarn build

POPD