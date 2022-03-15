namespace ASC.Web.Api.ApiModel.ResponseDto;

public class AuthenticationTokenDto
{
    public string Token { get; set; }
    public DateTime Expires { get; set; }
    public bool Sms { get; set; }
    public string PhoneNoise { get; set; }
    public bool Tfa { get; set; }
    public string TfaKey { get; set; }
    public string ConfirmUrl { get; set; }

    public static AuthenticationTokenDto GetSample()
    {
        return new AuthenticationTokenDto
        {
            Expires = DateTime.UtcNow,
            Token = "abcde12345",
            Sms = false,
            PhoneNoise = null,
            Tfa = false,
            TfaKey = null
        };
    }
}
