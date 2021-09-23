@echo off
PUSHD %~dp0..
set servicepath=%cd%\deploy\services\thumb\service\
set servicename=ASC.Thumbnails.Svc

PUSHD %~dp0..\..
set servicesource=%cd%\common\services\ASC.Thumbnails.Svc\

if "%1%" == "service" (
  set servicepath=%servicepath%%servicename%.exe urls=http://0.0.0.0:5030 $STORAGE_ROOT=..\..\..\..\..\Data  log:dir=..\..\..\..\..\Logs --log__name=thumbnails pathToConf=..\..\..\..\..\config core:products:folder=..\..\..\products core:products:subfolder=server
) else (
	if NOT "%1%" == "publish" (
		call %servicepath%%servicename%.exe
	)
)