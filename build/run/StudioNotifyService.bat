@echo off
PUSHD %~dp0..
set servicepath=%cd%\deploy\services\studio.notify
set servicename=ASC.Studio.Notify

PUSHD %~dp0..\..
set servicesource=%cd%\common\services\ASC.Studio.Notify\

if "%1%" == "service" (
  set servicepath=%servicepath%%servicename%.exe  --$STORAGE_ROOT=..\..\..\Data --log__dir=..\..\..\Logs --log__name=studio.notify
) else (
	if NOT "%1%" == "publish" (
		call %servicepath%%servicename%.exe
	)
)