@echo off
PUSHD %~dp0..
set servicepath=%cd%\deploy\services\clearevents\
set servicename=ASC.ClearEvents

PUSHD %~dp0..\..
set servicesource=%cd%\common\services\ASC.ClearEvents\

if "%1%" == "service" (
  set servicepath=%servicepath%%servicename%.exe urls=http://0.0.0.0:5027 $STORAGE_ROOT=..\..\..\..\Data  --log__dir=..\..\..\..\Logs --log__name=clearEvents pathToConf=..\..\..\..\config core:products:folder=..\..\products core:products:subfolder=server
) else (
	if NOT "%1%" == "publish" (
		call %servicepath%%servicename%.exe
	)
)