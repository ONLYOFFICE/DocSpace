@echo off
PUSHD %~dp0..
set servicepath=%cd%\deploy\studio\api\
set servicename=ASC.Web.Api

PUSHD %~dp0..\..
set servicesource=%cd%\web\ASC.Web.Api\

if "%1%" == "service" (
  set servicepath=%servicepath%%servicename%.exe urls=http://0.0.0.0:5000 $STORAGE_ROOT=..\..\..\..\Data log:dir=..\..\..\..\Logs log:name=api pathToConf=..\..\..\..\config core:products:folder=..\..\products core:products:subfolder=server
) else (
	if NOT "%1%" == "publish" (
		call %servicepath%%servicename%.exe
	)
)