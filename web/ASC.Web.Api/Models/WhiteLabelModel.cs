using System.Collections.Generic;

using ASC.Api.Collections;

using Microsoft.AspNetCore.Http;

namespace ASC.Web.Api.Models
{
    public class WhiteLabelModel
    {
        public IEnumerable<IFormFile> Attachments { get; set; }
        public string LogoText { get; set; }
        public IEnumerable<ItemKeyValuePair<string, string>> Logo { get; set; }
    }
    public class WhiteLabelQuery
    {
        public bool IsDefault { get; set; }
        public bool IsRetina { get; set; }
    }
}
