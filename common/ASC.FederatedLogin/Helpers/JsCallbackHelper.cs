namespace ASC.FederatedLogin.Helpers;

public static class JsCallbackHelper
{
    public static string GetCallbackPage()
    {
        using var reader = new StreamReader(Assembly
            .GetExecutingAssembly()
            .GetManifestResourceStream("ASC.FederatedLogin.callback.htm"));

        return reader.ReadToEnd();
    }
}
