PUSHD %~dp0\..
set dir="%cd%"
echo %dir%
dotnet test %dir%\common\Tests\Frontend.Translations.Tests\Frontend.Translations.Tests.csproj --filter "TestCategory=Locales" -l:html --environment "BASE_DIR=%dir%" --results-directory "%dir%\TestsResults"