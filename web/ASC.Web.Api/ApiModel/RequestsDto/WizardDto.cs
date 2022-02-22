namespace ASC.Web.Api.ApiModel.RequestsDto;

public class WizardDto
{
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Lng { get; set; }
    public string TimeZone { get; set; }
    public string Promocode { get; set; }
    public string AmiId { get; set; }
    public bool SubscribeFromSite { get; set; }

    public void Deconstruct(out string email, out string passwordHash, out string lng, out string timeZone, out string promocode, out string amiid, out bool subscribeFromSite)
    {
        (email, passwordHash, lng, timeZone, promocode, amiid, subscribeFromSite) = (Email, PasswordHash, Lng, TimeZone, Promocode, AmiId, SubscribeFromSite);
    }
}