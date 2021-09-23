@echo off
PUSHD %~dp0..
set servicepath=%cd%\deploy\services\radicale\service\
set servicename=ASC.Radicale

PUSHD %~dp0..\..
set servicesource=%cd%\common\services\ASC.Radicale\

if "%1%" == "service" (
  set servicepath=%servicepath%%servicename%.exe urls=http://0.0.0.0:5024 $STORAGE_ROOT=..\..\..\..\..\Data  log:dir=..\..\..\..\..\Logs log__name=radicale pathToConf=..\..\..\..\..\config core:products:folder=..\..\..\products core:products:subfolder=server
) else (
	if NOT "%1%" == "publish" (
		call %servicepath%%servicename%.exe
	)
)