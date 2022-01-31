namespace ASC.Web.Api.Models
{
    public class SecurityModel
    {
        public Guid ProductId { get; set; }

        public Guid UserId { get; set; }

        public bool Administrator { get; set; }
    }
}
