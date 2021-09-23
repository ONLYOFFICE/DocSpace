@echo off
PUSHD %~dp0..
set servicepath=%cd%\deploy\services\encryption\
set servicename=ASC.Data.Storage.Encryption

PUSHD %~dp0..\..
set servicesource=%cd%\common\services\ASC.Data.Storage.Encryption\

if "%1%" == "service" (
  set servicepath=%servicepath%%servicename%.exe urls=http://0.0.0.0:5019 $STORAGE_ROOT=..\..\..\..\Data log:dir=..\..\..\..\Logs log:name=encryption  pathToConf=..\..\..\..\config core:products:folder=..\..\products core:products:subfolder=server
) else (
	if NOT "%1%" == "publish" (
		call %servicepath%%servicename%.exe
	)
)