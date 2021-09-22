@echo off
PUSHD %~dp0..
set servicepath=%cd%\deploy\services\socket\service
set servicename=ASC.Socket.IO.Svc

PUSHD %~dp0..\..
set servicesource=%cd%\common\services\ASC.Socket.IO.Svc\

if "%1%" == "service" (
  set servicepath=%servicepath%%servicename%.exe --$STORAGE_ROOT=..\..\Data  --log__dir=..\..\Logs --log__name=socket
) else (
	if NOT "%1%" == "publish" (
		call %servicepath%%servicename%.exe
	)
)