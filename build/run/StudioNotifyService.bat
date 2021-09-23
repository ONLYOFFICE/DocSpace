@echo off
PUSHD %~dp0..
set servicepath=%cd%\deploy\services\studio.notify\
set servicename=ASC.Studio.Notify

PUSHD %~dp0..\..
set servicesource=%cd%\common\services\ASC.Studio.Notify\

if "%1%" == "service" (
  set servicepath=%servicepath%%servicename%.exe urls=http://0.0.0.0:5006 $STORAGE_ROOT=..\..\..\..\Data log:dir=..\..\..\..\Logs log:name=studio.notify pathToConf=..\..\..\..\config core:products:folder=..\..\products core:products:subfolder=server
) else (
	if NOT "%1%" == "publish" (
		call %servicepath%%servicename%.exe
	)
)