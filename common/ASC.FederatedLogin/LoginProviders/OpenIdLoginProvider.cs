namespace ASC.FederatedLogin.LoginProviders;

class OpenIdLoginProvider : ILoginProvider
{
    public string Scopes => string.Empty;
    public string CodeUrl => string.Empty;
    public string AccessTokenUrl => string.Empty;
    public string RedirectUri => string.Empty;
    public string ClientID => string.Empty;
    public string ClientSecret => string.Empty;
    public bool IsEnabled => _consumerFactory.Get<GoogleLoginProvider>().IsEnabled;

    private static readonly OpenIdRelyingParty _openId = new OpenIdRelyingParty();
    private readonly Signature _signature;
    private readonly InstanceCrypto _instanceCrypto;
    private readonly ConsumerFactory _consumerFactory;

    public OpenIdLoginProvider(Signature signature, InstanceCrypto instanceCrypto, ConsumerFactory consumerFactory)
    {
        _signature = signature;
        _instanceCrypto = instanceCrypto;
        _consumerFactory = consumerFactory;
    }

    public LoginProfile ProcessAuthoriztion(HttpContext context, IDictionary<string, string> @params, IDictionary<string, string> additionalStateArgs)
    {
        var response = _openId.GetResponse();
        if (response == null)
        {
            if (Identifier.TryParse(@params["oid"], out var id))
            {
                try
                {
                    IAuthenticationRequest request;

                    var realmUrlString = string.Empty;

                    @params.TryGetValue("realmUrl", out realmUrlString);

                    if (!string.IsNullOrEmpty(realmUrlString))
                    {
                        request = _openId.CreateRequest(id, new Realm(realmUrlString));
                    }
                    else
                    {
                        request = _openId.CreateRequest(id);
                    }

                    request.AddExtension(new ClaimsRequest
                    {
                        Email = DemandLevel.Require,
                        Nickname = DemandLevel.Require,
                        Country = DemandLevel.Request,
                        Gender = DemandLevel.Request,
                        PostalCode = DemandLevel.Request,
                        TimeZone = DemandLevel.Request,
                        FullName = DemandLevel.Request,


                    });
                    var fetch = new FetchRequest();
                    fetch.Attributes.AddRequired(WellKnownAttributes.Contact.Email);
                    //Duplicating attributes
                    fetch.Attributes.AddRequired("http://schema.openid.net/contact/email");//Add two more
                    fetch.Attributes.AddRequired("http://openid.net/schema/contact/email");
                    fetch.Attributes.AddRequired(WellKnownAttributes.Name.Alias);
                    fetch.Attributes.AddRequired(WellKnownAttributes.Name.First);
                    fetch.Attributes.AddRequired(WellKnownAttributes.Media.Images.Default);
                    fetch.Attributes.AddRequired(WellKnownAttributes.Name.Last);
                    fetch.Attributes.AddRequired(WellKnownAttributes.Name.Middle);
                    fetch.Attributes.AddRequired(WellKnownAttributes.Person.Gender);
                    fetch.Attributes.AddRequired(WellKnownAttributes.BirthDate.WholeBirthDate);
                    request.AddExtension(fetch);
                    request.RedirectToProvider();
                    //context.Response.End();//TODO This will throw thread abort

                }
                catch (ProtocolException ex)
                {
                    return LoginProfile.FromError(_signature, _instanceCrypto, ex);
                }
            }
            else
            {
                return LoginProfile.FromError(_signature, _instanceCrypto, new Exception("invalid OpenID identifier"));
            }
        }
        else
        {
            // Stage 3: OpenID Provider sending assertion response
            switch (response.Status)
            {
                case AuthenticationStatus.Authenticated:
                    var spprofile = response.GetExtension<ClaimsResponse>();
                    var fetchprofile = response.GetExtension<FetchResponse>();

                    var realmUrlString = string.Empty;
                    @params.TryGetValue("realmUrl", out realmUrlString);

                    var profile = ProfileFromOpenId(spprofile, fetchprofile, response.ClaimedIdentifier.ToString(), realmUrlString);
                    return profile;
                case AuthenticationStatus.Canceled:
                    return LoginProfile.FromError(_signature, _instanceCrypto, new Exception("Canceled at provider"));
                case AuthenticationStatus.Failed:
                    return LoginProfile.FromError(_signature, _instanceCrypto, response.Exception);
            }
        }
        return null;
    }

    public LoginProfile GetLoginProfile(string accessToken)
    {
        throw new NotImplementedException();
    }

    internal LoginProfile ProfileFromOpenId(ClaimsResponse spprofile, FetchResponse fetchprofile, string claimedId, string realmUrlString)
    {
        var profile = new LoginProfile(_signature, _instanceCrypto)
        {
            Link = claimedId,
            Id = claimedId,
            Provider = ProviderConstants.OpenId,
        };
        if (spprofile != null)
        {
            //Fill
            profile.BirthDay = spprofile.BirthDateRaw;
            profile.DisplayName = spprofile.FullName;
            profile.EMail = spprofile.Email;
            profile.Name = spprofile.Nickname;
            profile.Gender = spprofile.Gender.HasValue ? spprofile.Gender.Value.ToString() : "";
            profile.TimeZone = spprofile.TimeZone;
            profile.Locale = spprofile.Language;
        }
        if (fetchprofile != null)
        {
            profile.Name = fetchprofile.GetAttributeValue(WellKnownAttributes.Name.Alias);
            profile.LastName = fetchprofile.GetAttributeValue(WellKnownAttributes.Name.Last);
            profile.FirstName = fetchprofile.GetAttributeValue(WellKnownAttributes.Name.First);
            profile.DisplayName = fetchprofile.GetAttributeValue(WellKnownAttributes.Name.FullName);
            profile.MiddleName = fetchprofile.GetAttributeValue(WellKnownAttributes.Name.Middle);
            profile.Salutation = fetchprofile.GetAttributeValue(WellKnownAttributes.Name.Prefix);
            profile.Avatar = fetchprofile.GetAttributeValue(WellKnownAttributes.Media.Images.Default);
            profile.EMail = fetchprofile.GetAttributeValue(WellKnownAttributes.Contact.Email);
            profile.Gender = fetchprofile.GetAttributeValue(WellKnownAttributes.Person.Gender);
            profile.BirthDay = fetchprofile.GetAttributeValue(WellKnownAttributes.BirthDate.WholeBirthDate);
        }
        profile.RealmUrl = realmUrlString;

        return profile;
    }
}
