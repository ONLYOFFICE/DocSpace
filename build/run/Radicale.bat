@echo off
PUSHD %~dp0..
set servicepath=%cd%\deploy\services\radicale\service
set servicename=ASC.Radicale

PUSHD %~dp0..\..
set servicesource=%cd%\common\services\ASC.Radicale\

if "%1%" == "service" (
  set servicepath=%servicepath%%servicename%.exe --$STORAGE_ROOT=..\..\Data  --log__dir=..\..\Logs --log__name=radicale
) else (
	if NOT "%1%" == "publish" (
		call %servicepath%%servicename%.exe
	)
)