@echo off

PUSHD %~dp0..\..
set servicepath=%cd%\common\services\ASC.ApiSystem\bin\Debug\ASC.ApiSystem.exe urls=http://0.0.0.0:5010 $STORAGE_ROOT=%cd%\Data log:dir=%cd%\Logs log:name=apisystem pathToConf=%cd%\config core:products:folder=%cd%\products