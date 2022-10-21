@echo off

PUSHD %~dp0..\..
set servicepath=%cd%\products\ASC.People\Server\bin\Debug\ASC.People.exe urls=http://0.0.0.0:5004 STORAGE_ROOT=%cd%\Data log:dir=%cd%\Logs log:name=people pathToConf=%cd%\config core:products:folder=%cd%\products