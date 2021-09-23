@echo off
PUSHD %~dp0..
set servicepath=%cd%\deploy\services\socket\service\
set servicename=ASC.Socket.IO.Svc

PUSHD %~dp0..\..
set servicesource=%cd%\common\services\ASC.Socket.IO.Svc\

if "%1%" == "service" (
  set servicepath=%servicepath%%servicename%.exe urls=http://0.0.0.0:5028 $STORAGE_ROOT=..\..\..\..\..\Data  log:dir=..\..\..\..\..\Logs log:name=socket pathToConf=..\..\..\..\..\config core:products:folder=..\..\..\products core:products:subfolder=server
) else (
	if NOT "%1%" == "publish" (
		call %servicepath%%servicename%.exe
	)
)