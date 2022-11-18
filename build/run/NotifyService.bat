@echo off

PUSHD %~dp0..\..
set servicepath=%cd%\common\services\ASC.Notify\bin\Debug\ASC.Notify.exe urls=http://0.0.0.0:5005 $STORAGE_ROOT=%cd%\Data log:dir=%cd%\Logs log:name=notify pathToConf=%cd%\config core:products:folder=%cd%\products core:eventBus:subscriptionClientName=asc_event_bus_notify_queue