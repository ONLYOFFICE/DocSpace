PUSHD %~dp0..

REM call yarn wipe
call yarn install

REM call yarn build
call yarn build

REM call yarn wipe
call yarn deploy

pause