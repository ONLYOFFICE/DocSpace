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
xcopy "%PathToRepository%\build\deploy\client" "%PathToAppFolder%\client" /s /y /b /i
xcopy "%PathToRepository%\build\deploy\login" "%PathToAppFolder%\login" /s /y /b /i
xcopy "%PathToRepository%\build\deploy\editor" "%PathToAppFolder%\products\ASC.Files\editor" /s /y /b /i
xcopy "%PathToRepository%\config\nginx" "%PathToAppFolder%\nginx\conf" /s /y /b /i
xcopy "%PathToRepository%\config\*" "%PathToAppFolder%\config" /y /b /i
