@echo off
PUSHD %~dp0..
set servicepath=%cd%\deploy\services\encryption
set servicename=ASC.Data.Storage.Encryption

PUSHD %~dp0..\..
set servicesource=%cd%\common\services\ASC.Data.Storage.Encryption\

if "%1%" == "service" (
  set servicepath=%servicepath%%servicename%.exe --$STORAGE_ROOT=..\..\..\Data --log__dir=..\..\..\Logs --log__name=encryption
) else (
	if NOT "%1%" == "publish" (
		call %servicepath%%servicename%.exe
	)
)