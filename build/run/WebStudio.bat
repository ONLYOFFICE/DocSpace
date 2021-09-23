@echo off
PUSHD %~dp0..
set servicepath=%cd%\deploy\studio\server\
set servicename=ASC.Web.Studio

PUSHD %~dp0..\..
set servicesource=%cd%\web\ASC.Web.Studio\

if "%1%" == "service" (
  set servicepath=%servicepath%%servicename%.exe urls=http://0.0.0.0:5003 $STORAGE_ROOT=..\..\..\..\Data  log:dir=..\..\..\..\Logs log:name=studio pathToConf=..\..\..\..\config core:products:folder=..\..\products core:products:subfolder=server
) else (
	if NOT "%1%" == "publish" (
		call %servicepath%%servicename%.exe
	)
)