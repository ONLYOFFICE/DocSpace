@echo off
echo 
echo #####################
echo #   frontend copy   #
echo #####################

set FirstArg=%~s1

set SecondArg=%~s2

if defined SecondArg (
	set PathToRepository=%FirstArg%
	set PathToAppFolder=%SecondArg%
) else (
	set PathToRepository=%FirstArg%
	set PathToAppFolder=%FirstArg%\publish
)

xcopy "%PathToRepository%\build\deploy\public" "%PathToAppFolder%\public" /s /y /b /i
xcopy "%PathToRepository%\build\deploy\studio\client" "%PathToAppFolder%\studio\client" /s /y /b /i
xcopy "%PathToRepository%\build\deploy\studio\login" "%PathToAppFolder%\studio\login" /s /y /b /i
xcopy "%PathToRepository%\build\deploy\products\ASC.Files\client" "%PathToAppFolder%\products\ASC.Files\client" /s /y /b /i
xcopy "%PathToRepository%\build\deploy\products\ASC.Files\editor" "%PathToAppFolder%\editor" /s /y /b /i
xcopy "%PathToRepository%\build\deploy\products\ASC.People\client" "%PathToAppFolder%\products\ASC.People\client" /s /y /b /i
xcopy "%PathToRepository%\config\nginx" "%PathToAppFolder%\nginx\conf" /s /y /b /i
xcopy "%PathToRepository%\config\*" "%PathToAppFolder%\config" /y /b /i
