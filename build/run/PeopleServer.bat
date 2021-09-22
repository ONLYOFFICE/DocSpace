@echo off
PUSHD %~dp0..
set servicepath=%cd%\deploy\products\ASC.People\server\
set servicename=ASC.People

PUSHD %~dp0..\..
set servicesource=%cd%\products\ASC.People\Server\

if "%1%" == "service" (
  set servicepath=%servicepath%%servicename%.exe --$STORAGE_ROOT=..\..\..\Data --log__dir=..\..\..\Logs --log__name=people
) else (
	if NOT "%1%" == "publish" (
		call %servicepath%%servicename%.exe
	)
)