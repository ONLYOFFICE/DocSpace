PUSHD %~dp0..
set dir=%~dp0..
echo %dir%

dotnet test common\Tests\Frontend.Translations.Tests\Frontend.Translations.Tests.csproj --filter Name~SpellCheckTest -l:html --environment "BASE_DIR=%dir%" --results-directory "%dir%/TestsResults"