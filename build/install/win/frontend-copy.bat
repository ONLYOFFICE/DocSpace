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

XCOPY "%PathToRepository%\products\ASC.CRM\Client\dist" "%PathToAppFolder%\products\ASC.CRM\client" /S /Y /B /I
XCOPY "%PathToRepository%\products\ASC.Mail\Client\dist" "%PathToAppFolder%\products\ASC.Mail\client" /S /Y /B /I
XCOPY "%PathToRepository%\products\ASC.Files\Client\dist" "%PathToAppFolder%\products\ASC.Files\client" /S /Y /B /I
XCOPY "%PathToRepository%\products\ASC.People\Client\dist" "%PathToAppFolder%\products\ASC.People\client" /S /Y /B /I
XCOPY "%PathToRepository%\products\ASC.Calendar\Client\dist" "%PathToAppFolder%\products\ASC.Calendar\client" /S /Y /B /I
XCOPY "%PathToRepository%\products\ASC.Projects\Client\dist" "%PathToAppFolder%\products\ASC.Projects\client" /S /Y /B /I
XCOPY "%PathToRepository%\web\ASC.Web.Editor\dist" "%PathToAppFolder%\products\ASC.Files\editor" /S /Y /B /I
XCOPY "%PathToRepository%\web\ASC.Web.Client\dist" "%PathToAppFolder%\studio\client" /S /Y /B /I
XCOPY "%PathToRepository%\web\ASC.Web.Login\dist" "%PathToAppFolder%\studio\login" /S /Y /B /I
XCOPY "%PathToRepository%\build\deploy\public" "%PathToAppFolder%\public" /S /Y /B /I
XCOPY "%PathToRepository%\config\nginx" "%PathToAppFolder%\nginx\conf" /S /Y /B /I
XCOPY "%PathToRepository%\config\*" "%PathToAppFolder%\config" /Y /B /I