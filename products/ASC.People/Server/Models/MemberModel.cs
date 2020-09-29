using System;
using System.Collections.Generic;

using ASC.Api.Core;
using ASC.Web.Api.Models;

namespace ASC.People.Models
{
    public class MemberModel
    {
        public bool IsVisitor { get; set; }
        public string Email { get; set; }
        public string Firstname { get; set; }

        public string Lastname { get; set; }
        public Guid[] Department { get; set; }
        public string Title { get; set; }
        public string Location { get; set; }
        public string Sex { get; set; }
        public ApiDateTime Birthday { get; set; }
        public ApiDateTime Worksfrom { get; set; }
        public string Comment { get; set; }
        public IEnumerable<Contact> Contacts { get; set; }
        public string Files { get; set; }
        public string Password { get; set; }
        public string PasswordHash { get; set; }
    }

    public class UpdateMemberModel : MemberModel
    {
        public string UserId { get; set; }
        public bool? Disable { get; set; }
        public string CultureName { get; set; }
    }
}
