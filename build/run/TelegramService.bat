@echo off
PUSHD %~dp0..
set servicepath=%cd%\deploy\services\telegram
set servicename=ASC.TelegramService

PUSHD %~dp0..\..
set servicesource=%cd%\common\services\ASC.TelegramService\

if "%1%" == "service" (
  set servicepath=%servicepath%%servicename%.exe --$STORAGE_ROOT=..\..\Data  --log__dir=..\..\Logs --log__name=telegram
) else (
	if NOT "%1%" == "publish" (
		call %servicepath%%servicename%.exe
	)
)