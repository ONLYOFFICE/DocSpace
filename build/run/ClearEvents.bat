@echo off

PUSHD %~dp0..\..
set servicepath=%cd%\common\services\ASC.ClearEvents\bin\Debug\ASC.ClearEvents.exe urls=http://0.0.0.0:5027 STORAGE_ROOT=%cd%\Data pathToConf=%cd%\config log:dir=%cd%\Logs log:name=clearEvents core:products:folder=%cd%\products