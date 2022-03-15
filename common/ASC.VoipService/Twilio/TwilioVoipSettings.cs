using Uri = System.Uri;

namespace ASC.VoipService.Twilio;

public class TwilioVoipSettings : VoipSettings
{
    public TwilioVoipSettings(
        AuthContext authContext,
        TenantUtil tenantUtil,
        SecurityContext securityContext,
        BaseCommonLinkUtility baseCommonLinkUtility) :
        base(authContext, tenantUtil, securityContext, baseCommonLinkUtility)
    { }

    public TwilioVoipSettings(
        Uri voiceUrl,
        AuthContext authContext,
        TenantUtil tenantUtil,
        SecurityContext securityContext,
        BaseCommonLinkUtility baseCommonLinkUtility) :
        this(authContext, tenantUtil, securityContext, baseCommonLinkUtility)
    {
        if (string.IsNullOrEmpty(voiceUrl.Query)) return;

        JsonSettings = Encoding.UTF8.GetString(Convert.FromBase64String(HttpUtility.UrlDecode(HttpUtility.ParseQueryString(voiceUrl.Query)["settings"])));
    }

    public TwilioVoipSettings(string settings, AuthContext authContext) : base(settings, authContext)
    {
    }

    public override string Connect(bool user = true, string contactId = null)
    {
        var result = GetEcho("", user);
        if (!string.IsNullOrEmpty(contactId))
        {
            result += "&ContactId=" + contactId;
        }
        return result;
    }

    public override string Redirect(string to)
    {
        return GetEcho("redirect") + "&RedirectTo=" + to;
    }

    public override string Dequeue(bool reject)
    {
        return GetEcho("dequeue") + "&Reject=" + reject;
    }

    private string GetEcho(string method, bool user = true)
    {
        return new TwilioResponseHelper(this, BaseCommonLinkUtility.GetFullAbsolutePath(""), AuthContext, TenantUtil, SecurityContext).GetEcho(method, user);
    }
}
