@echo off

PUSHD %~dp0..\..
set servicepath=%cd%\common\services\ASC.Data.Storage.Encryption\bin\Debug\ASC.Data.Storage.Encryption.exe urls=http://0.0.0.0:5019 $STORAGE_ROOT=%cd%\Data log:dir=%cd%\Logs log:name=studio.notify pathToConf=%cd%\config core:products:folder=%cd%\products
