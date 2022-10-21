@echo off

PUSHD %~dp0..\..
set servicepath=%cd%\common\services\ASC.Studio.Notify\bin\Debug\ASC.Studio.Notify.exe urls=http://0.0.0.0:5006 STORAGE_ROOT=%cd%\Data log:dir=%cd%\Logs log:name=studio.notify pathToConf=%cd%\config core:products:folder=%cd%\products