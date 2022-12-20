@echo off

PUSHD %~dp0..\..
set servicepath=%cd%\common\services\ASC.ApiCache\bin\Debug\ASC.ApiCache.exe urls=http://0.0.0.0:5100 $STORAGE_ROOT=%cd%\Data log:dir=%cd%\Logs log:name=apicache pathToConf=%cd%\config core:products:folder=%cd%\products