@echo off
PUSHD %~dp0..
set servicepath=%cd%\deploy\services\telegram\
set servicename=ASC.TelegramService

PUSHD %~dp0..\..
set servicesource=%cd%\common\services\ASC.TelegramService\

if "%1%" == "service" (
  set servicepath=%servicepath%%servicename%.exe urls=http://0.0.0.0:51702 $STORAGE_ROOT=..\..\..\..\Data  log:dir=..\..\..\..\Logs log:name=telegram pathToConf=..\..\..\..\config core:products:folder=..\..\products core:products:subfolder=server
) else (
	if NOT "%1%" == "publish" (
		call %servicepath%%servicename%.exe
	)
)