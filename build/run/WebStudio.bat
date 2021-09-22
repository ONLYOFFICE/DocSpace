@echo off
PUSHD %~dp0..
set servicepath=%cd%\deploy\studio\server\
set servicename=ASC.Web.Studio

PUSHD %~dp0..\..
set servicesource=%cd%\web\ASC.Web.Studio\

if "%1%" == "service" (
  set servicepath=%servicepath%%servicename%.exe --$STORAGE_ROOT=..\..\Data  --log__dir=..\..\Logs --log__name=studio
) else (
	if NOT "%1%" == "publish" (
		call %servicepath%%servicename%.exe
	)
)