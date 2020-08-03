namespace ASC.Web.Api.Models
{
    public class WizardModel
    {
        public string Email { get; set; }
        public string Pwd { get; set; }
        public string Lng { get; set; }
        public string TimeZone { get; set; }
        public string Promocode { get; set; }
        public string AmiId { get; set; }
        public bool Analytics { get; set; }

        public void Deconstruct(out string email, out string pwd, out string lng, out string timeZone, out string promocode, out string amiid, out bool analytics)
            => (email, pwd, lng, timeZone, promocode, amiid, analytics) = (Email, Pwd, Lng, TimeZone, Promocode, AmiId, Analytics);
    }
}
