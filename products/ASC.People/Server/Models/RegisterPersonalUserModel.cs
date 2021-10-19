namespace ASC.People.Models
{
    public class RegisterPersonalUserModel
    {
        public string Email { get; set; }
        public string Lang { get; set; }
        public bool Spam { get; set; }
        public string RecaptchaResponse { get; set; }
    }
}
