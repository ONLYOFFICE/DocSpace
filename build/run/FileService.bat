@echo off

PUSHD %~dp0..\..
set servicepath=%cd%\products\ASC.Files\Service\bin\Debug\ASC.Files.Service.exe urls=http://0.0.0.0:5009 STORAGE_ROOT=%cd%\Data log:dir=%cd%\Logs log:name=files.service pathToConf=%cd%\config core:products:folder=%cd%\products
