@echo off

PUSHD %~dp0..\..
set servicepath=%cd%\common\services\ASC.Webhooks.Service\bin\Debug\ASC.Webhooks.Service.exe urls=http://0.0.0.0:5031 STORAGE_ROOT=%cd%\Data  log:dir=%cd%\Logs log:name=webhooks pathToConf=%cd%\config core:products:folder=%cd%\products