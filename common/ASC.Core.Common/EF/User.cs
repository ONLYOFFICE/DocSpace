using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using ASC.Core.Users;

namespace ASC.Core.Common.EF
{
    [Table("core_user")]
    public class User
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

        public static implicit operator UserInfo(User user)
        {
            return new UserInfo
            {
                ActivationStatus = user.ActivationStatus,
                BirthDate = user.Birthdate,
                CreateDate = user.CreateOn,
                CultureName = user.Culture,
                Email = user.Email,
                FirstName = user.FirstName,
                ID = user.Id,
                LastModified = user.LastModified,
                LastName = user.LastName,
                Location = user.Location,
                MobilePhone = user.Phone,
                MobilePhoneActivationStatus = user.PhoneActivation,
                Notes = user.Notes,
                Removed = user.Removed,
                Sex = user.Sex,
                Sid = user.Sid,
                SsoNameId = user.SsoNameId,
                SsoSessionId = user.SsoSessionId,
                Status = user.Status,
                Tenant = user.Tenant,
                TerminatedDate = user.TerminatedDate,
                Title = user.Title,
                UserName = user.UserName,
                WorkFromDate = user.WorkFromDate,
                Contacts = user.Contacts
            };
        }

        public static implicit operator User(UserInfo user)
        {
            return new User
            {
                ActivationStatus = user.ActivationStatus,
                Birthdate = user.BirthDate,
                CreateOn = user.CreateDate,
                Culture = user.CultureName,
                Email = user.Email,
                FirstName = user.FirstName,
                Id = user.ID,
                LastModified = user.LastModified,
                LastName = user.LastName,
                Location = user.Location,
                Phone = user.MobilePhone,
                PhoneActivation = user.MobilePhoneActivationStatus,
                Notes = user.Notes,
                Removed = user.Removed,
                Sex = user.Sex,
                Sid = user.Sid,
                SsoNameId = user.SsoNameId,
                SsoSessionId = user.SsoSessionId,
                Status = user.Status,
                Tenant = user.Tenant,
                TerminatedDate = user.TerminatedDate,
                Title = user.Title,
                UserName = user.UserName,
                WorkFromDate = user.WorkFromDate,
                Contacts = user.Contacts
            };
        }
    }
}
