PUSHD %~dp0..
set dir=%~dp0..
echo %dir%

set save=false

 if /I "%1" == "-s" set save=%2 & shift
 shift

dotnet test common\Tests\Frontend.Translations.Tests\Frontend.Translations.Tests.csproj --filter Name~SpellCheckTest -l:html --environment "BASE_DIR=%dir%" --environment "SAVE=%save%" --results-directory "%dir%/TestsResults"