@echo off
PUSHD %~dp0..
set servicepath=%cd%\deploy\services\ssoauth\service\
set servicename=ASC.SsoAuth.Svc

PUSHD %~dp0..\..
set servicesource=%cd%\common\services\ASC.SsoAuth.Svc\

if "%1%" == "service" (
  set servicepath=%servicepath%%servicename%.exe urls=http://0.0.0.0:5031 $STORAGE_ROOT=..\..\..\..\..\Data  log:dir=..\..\..\..\..\Logs log:name=ssoauth pathToConf=..\..\..\..\..\config core:products:folder=..\..\..\products core:products:subfolder=server ssoauth:path=..\client
) else (
	if NOT "%1%" == "publish" (
		call %servicepath%%servicename%.exe
	)
)