namespace ASC.People.ApiModels.RequestDto;

public class RegisterPersonalUserRequestDto
{
    public string Email { get; set; }
    public string Lang { get; set; }
    public bool Spam { get; set; }
    public string RecaptchaResponse { get; set; }
}
