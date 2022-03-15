namespace ASC.Web.Api.Models;

public class Contact
{
    public string Type { get; set; }
    public string Value { get; set; }

    //For binder
    public Contact() { }

    public Contact(string type, string value)
    {
        Type = type;
        Value = value;
    }

    public static Contact GetSample()
    {
        return new Contact("GTalk", "my@gmail.com");
    }
}