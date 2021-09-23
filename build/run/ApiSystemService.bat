@echo off
PUSHD %~dp0..
set servicepath=%cd%\deploy\services\apisystem\
set servicename=ASC.ApiSystem

PUSHD %~dp0..\..
set servicesource=%cd%\common\services\ASC.ApiSystem\

if "%1%" == "service" (
  set servicepath=%servicepath%%servicename%.exe urls=http://0.0.0.0:5010 $STORAGE_ROOT=..\..\..\..\Data log:dir=..\..\..\..\Logs log:name=apisystem pathToConf=..\..\..\..\config core:products:folder=..\..\products core:products:subfolder=server
) else (
	if NOT "%1%" == "publish" (
		call %servicepath%%servicename%.exe
	)
)