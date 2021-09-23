@echo off
PUSHD %~dp0..
set servicepath=%cd%\deploy\services\urlshortener\service
set servicename=ASC.UrlShortener.Svc

PUSHD %~dp0..\..
set servicesource=%cd%\common\services\ASC.UrlShortener.Svc\

if "%1%" == "service" (
  set servicepath=%servicepath%%servicename%.exe urls=http://0.0.0.0:5029 $STORAGE_ROOT=..\..\..\..\..\Data log:dir=..\..\..\..\..\Logs log:name=urlshortener pathToConf=..\..\..\..\..\config core:products:folder=..\..\..\products core:products:subfolder=server
) else (
	if NOT "%1%" == "publish" (
		call %servicepath%%servicename%.exe
	)
)