@echo off
PUSHD %~dp0..
set servicepath=%cd%\deploy\services\backup\
set servicename=ASC.Data.Backup

PUSHD %~dp0..\..
set servicesource=%cd%\common\services\ASC.Data.Backup\

if "%1%" == "service" (
  set servicepath=%servicepath%%servicename%.exe urls=http://0.0.0.0:5012 $STORAGE_ROOT=..\..\..\..\Data log:dir=..\..\..\..\Logs log:name=backup  pathToConf=..\..\..\..\config core:products:folder=..\..\products core:products:subfolder=server
) else (
	if NOT "%1%" == "publish" (
		call %servicepath%%servicename%.exe
	)
)