@echo off

PUSHD %~dp0..\..
set servicepath=%cd%\common\ASC.Migration\bin\Debug\ASC.Migration.exe urls=http://0.0.0.0:5034 STORAGE_ROOT=%cd%\Data pathToConf=%cd%\config log:dir=%cd%\Logs log:name=migration core:products:folder=%cd%\products
