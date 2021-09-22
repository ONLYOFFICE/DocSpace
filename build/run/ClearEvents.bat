@echo off
PUSHD %~dp0..
set servicepath=%cd%\deploy\services\clearevents
set servicename=ASC.ClearEvents

PUSHD %~dp0..\..
set servicesource=%cd%\common\services\ASC.ClearEvents\

if "%1%" == "service" (
  set servicepath=%servicepath%%servicename%.exe --$STORAGE_ROOT=..\..\Data  --log__dir=..\..\Logs --log__name=clearEvents
) else (
	if NOT "%1%" == "publish" (
		call %servicepath%%servicename%.exe
	)
)