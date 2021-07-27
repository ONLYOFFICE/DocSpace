
using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF.Model.Mail
{
    [Keyless]
    public class GreyListingWhiteList
    {
        public string Comment { get; set; }
        public string Source { get; set; }
    }
}
