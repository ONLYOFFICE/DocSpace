@echo off
PUSHD %~dp0..
set servicepath=%cd%\deploy\services\urlshortener\service
set servicename=ASC.UrlShortener.Svc

PUSHD %~dp0..\..
set servicesource=%cd%\common\services\ASC.UrlShortener.Svc\

if "%1%" == "service" (
  set servicepath=%servicepath%%servicename%.exe  --$STORAGE_ROOT=..\..\..\Data --log__dir=..\..\..\Logs --log__name=urlshortener
) else (
	if NOT "%1%" == "publish" (
		call %servicepath%%servicename%.exe
	)
)