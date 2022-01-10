PUSHD %~dp0..
dotnet test common\Tests\Frontend.Translations.Tests\Frontend.Translations.Tests.csproj --filter "TestCategory=FastRunning" -l:html -r TestsResults