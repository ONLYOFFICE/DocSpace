@echo off
PUSHD %~dp0..
set servicepath=%cd%\deploy\products\ASC.Mail\server
set servicename=ASC.Mail

PUSHD %~dp0..\..
set servicesource=%cd%\products\ASC.Mail\Server\

if "%1%" == "service" (
  set servicepath=%servicepath%%servicename%.exe  --$STORAGE_ROOT=..\..\..\Data --log__dir=..\..\..\Logs --log__name=mail
) else (
	if NOT "%1%" == "publish" (
		call %servicepath%%servicename%.exe
	)
)