namespace ASC.People.Api;

public class ThirdpartyController : ApiControllerBase
{
    private readonly ThirdpartyControllerEngine _thirdpartyControllerEngine;

    public ThirdpartyController(ThirdpartyControllerEngine thirdpartyControllerEngine)
    {
        _thirdpartyControllerEngine = thirdpartyControllerEngine;
    }

    [AllowAnonymous]
    [Read("thirdparty/providers")]
    public ICollection<AccountInfoDto> GetAuthProviders(bool inviteView, bool settingsView, string clientCallback, string fromOnly)
    {
        return _thirdpartyControllerEngine.GetAuthProviders(inviteView, settingsView, clientCallback, fromOnly, Request);
    }

    [Update("thirdparty/linkaccount")]
    public void LinkAccountFromBody([FromBody] LinkAccountRequestDto model)
    {
        _thirdpartyControllerEngine.LinkAccount(model);
    }

    [Update("thirdparty/linkaccount")]
    [Consumes("application/x-www-form-urlencoded")]
    public void LinkAccountFromForm([FromForm] LinkAccountRequestDto model)
    {
        _thirdpartyControllerEngine.LinkAccount(model);
    }

    [AllowAnonymous]
    [Create("thirdparty/signup")]
    public void SignupAccountFromBody([FromBody] SignupAccountRequestDto model)
    {
        _thirdpartyControllerEngine.SignupAccount(model);
    }

    [AllowAnonymous]
    [Create("thirdparty/signup")]
    [Consumes("application/x-www-form-urlencoded")]
    public void SignupAccountFromForm([FromForm] SignupAccountRequestDto model)
    {
        _thirdpartyControllerEngine.SignupAccount(model);
    }

    [Delete("thirdparty/unlinkaccount")]
    public void UnlinkAccount(string provider)
    {
        _thirdpartyControllerEngine.UnlinkAccount(provider);
    }
}
