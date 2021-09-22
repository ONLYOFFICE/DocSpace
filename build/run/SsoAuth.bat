@echo off
PUSHD %~dp0..
set servicepath=%cd%\deploy\services\ssoauth\service
set servicename=ASC.SsoAuth.Svc

PUSHD %~dp0..\..
set servicesource=%cd%\common\services\ASC.SsoAuth.Svc\

if "%1%" == "service" (
  set servicepath=%servicepath%%servicename%.exe --$STORAGE_ROOT=..\..\Data  --log__dir=..\..\Logs --log__name=ssoauth
) else (
	if NOT "%1%" == "publish" (
		call %servicepath%%servicename%.exe
	)
)