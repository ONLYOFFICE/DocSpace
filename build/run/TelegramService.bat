@echo off

PUSHD %~dp0..\..
set servicepath=%cd%\common\services\ASC.TelegramService\bin\Debug\ASC.TelegramService.exe urls=http://0.0.0.0:51702 STORAGE_ROOT=%cd%\Data log:dir=%cd%\Logs log:name=telegram pathToConf=%cd%\config