PUSHD %~dp0..

cd %~dp0../../common/ASC.SsoAuth/

call yarn install --frozen-lockfile

POPD