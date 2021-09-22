@echo off
PUSHD %~dp0..
set servicepath=%cd%\deploy\products\ASC.Calendar\server
set servicename=ASC.Calendar

PUSHD %~dp0..\..
set servicesource=%cd%\products\ASC.Calendar\Server\

if "%1%" == "service" (
  set servicepath=%servicepath%%servicename%.exe  --$STORAGE_ROOT=..\..\..\Data --log__dir=..\..\..\Logs --log__name=calendar
) else (
	if NOT "%1%" == "publish" (
		call %servicepath%%servicename%.exe
	)
)