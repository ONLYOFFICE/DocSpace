@echo off
PUSHD %~dp0..
set servicepath=%cd%\deploy\studio\api
set servicename=ASC.Web.Api

PUSHD %~dp0..\..
set servicesource=%cd%\web\ASC.Web.Api\

if "%1%" == "service" (
  set servicepath=%servicepath%%servicename%.exe --$STORAGE_ROOT=..\..\Data --log__dir=..\..\Logs --log__name=api
) else (
	if NOT "%1%" == "publish" (
		call %servicepath%%servicename%.exe
	)
)