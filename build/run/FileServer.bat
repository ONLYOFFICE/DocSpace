@echo off
PUSHD %~dp0..
set servicepath=%cd%\deploy\products\ASC.Files\server\
set servicename=ASC.Files

PUSHD %~dp0..\..
set servicesource=%cd%\products\ASC.Files\Server\

if "%1%" == "service" (
  set servicepath=%servicepath%%servicename%.exe --$STORAGE_ROOT=..\..\..\Data --log__dir=..\..\..\Logs --log__name=files
) else (
	if NOT "%1%" == "publish" (
		call %servicepath%%servicename%.exe
	)
)