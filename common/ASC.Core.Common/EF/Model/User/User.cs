using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using ASC.Core.Users;

using Microsoft.EntityFrameworkCore;

namespace ASC.Core.Common.EF
{
    [Table("core_user")]
    public class User : BaseEntity
    {
        public int Tenant { get; set; }

        public string UserName { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Guid Id { get; set; }

        public bool? Sex { get; set; }

        [Column("bithdate")]
        public DateTime? Birthdate { get; set; }

        public EmployeeStatus Status { get; set; }

        [Column("activation_status")]
        public EmployeeActivationStatus ActivationStatus { get; set; }
        public string Email { get; set; }

        public DateTime? WorkFromDate { get; set; }

        public DateTime? TerminatedDate { get; set; }

        public string Title { get; set; }

        public string Culture { get; set; }

        public string Contacts { get; set; }

        public string Phone { get; set; }

        [Column("phone_activation")]
        public MobilePhoneActivationStatus PhoneActivation { get; set; }

        public string Location { get; set; }

        public string Notes { get; set; }

        public string Sid { get; set; }

        [Column("sso_name_id")]
        public string SsoNameId { get; set; }

        [Column("sso_session_id")]
        public string SsoSessionId { get; set; }

        public bool Removed { get; set; }

        [Column("create_on")]
        public DateTime CreateOn { get; set; }

        [Column("last_modified")]
        public DateTime LastModified { get; set; }

        public UserSecurity UserSecurity { get; set; }

        public List<UserGroup> Groups { get; set; }

        internal override object[] GetKeys()
        {
            return new object[] { Id };
        }
    }

    public static class DbUserExtension
    {
        public static void AddUser(this ModelBuilder modelBuilder)
        {
            modelBuilder.AddUserGroup();
        }
    }
}
