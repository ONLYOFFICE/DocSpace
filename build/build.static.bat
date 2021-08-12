PUSHD %~dp0..

REM call yarn build
call yarn build

REM call yarn wipe
call yarn deploy

pause