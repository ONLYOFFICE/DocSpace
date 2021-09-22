@echo off
PUSHD %~dp0..
set servicepath=%cd%\deploy\services\thumb\service
set servicename=ASC.Thumbnails.Svc

PUSHD %~dp0..\..
set servicesource=%cd%\common\services\ASC.Thumbnails.Svc\

if "%1%" == "service" (
  set servicepath=%servicepath%%servicename%.exe  --$STORAGE_ROOT=..\..\Data  --log__dir=..\..\Logs --log__name=thumbnails
) else (
	if NOT "%1%" == "publish" (
		call %servicepath%%servicename%.exe
	)
)