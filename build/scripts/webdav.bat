PUSHD %~dp0..

cd %~dp0../../common/ASC.WebDav/

call yarn install --frozen-lockfile

POPD