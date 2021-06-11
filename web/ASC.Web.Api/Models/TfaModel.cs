using System;

namespace ASC.Web.Api.Models
{
    public class TfaModel
    {
        public string Type { get; set; }
        public Guid? Id { get; set; }
    }

    public class TfaValidateModel
    {
        public string Code { get; set; }
    }
}
