namespace ASC.FederatedLogin.Helpers;

public static class XmlExtensions
{
    public static string SelectNodeValue(this XPathNavigator nav, string xpath)
    {
        var node = nav.SelectSingleNode(xpath);

        return node != null ? node.Value : string.Empty;
    }
}
