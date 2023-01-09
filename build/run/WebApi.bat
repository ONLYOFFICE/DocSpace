@echo off

PUSHD %~dp0..\..
set servicepath=%cd%\web\ASC.Web.Api\bin\Debug\ASC.Web.Api.exe urls=http://0.0.0.0:5000 $STORAGE_ROOT=%cd%\Data log:dir=%cd%\Logs log:name=web.api pathToConf=%cd%\config core:products:folder=%cd%\products