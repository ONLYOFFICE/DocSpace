/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
 *
 * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
 * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
 * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
 * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
 *
 * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
 * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
 *
 * You can contact Ascensio System SIA by email at sales@onlyoffice.com
 *
 * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
 * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
 *
 * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
 * relevant author attributions when distributing the software. If the display of the logo in its graphic 
 * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
 * in every copy of the program you distribute. 
 * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
 *
*/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using ASC.Api.Core;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Core;
using ASC.Web.Core.Users;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Api.Models
{
    [DataContract(Name = "person", Namespace = "")]
    public class EmployeeWraperFull : EmployeeWraper
    {
        [DataMember(Order = 10)]
        public string FirstName { get; set; }

        [DataMember(Order = 10)]
        public string LastName { get; set; }

        [DataMember(Order = 2)]
        public string UserName { get; set; }

        [DataMember(Order = 10)]
        public string Email { get; set; }

        [DataMember(Order = 12, EmitDefaultValue = false)]
        protected List<Contact> Contacts { get; set; }

        [DataMember(Order = 10, EmitDefaultValue = false)]
        public ApiDateTime Birthday { get; set; }

        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string Sex { get; set; }

        [DataMember(Order = 10)]
        public EmployeeStatus Status { get; set; }

        [DataMember(Order = 10)]
        public EmployeeActivationStatus ActivationStatus { get; set; }

        [DataMember(Order = 10)]
        public ApiDateTime Terminated { get; set; }

        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string Department { get; set; }

        [DataMember(Order = 10, EmitDefaultValue = false)]
        public ApiDateTime WorkFrom { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public List<GroupWrapperSummary> Groups { get; set; }

        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string Location { get; set; }

        [DataMember(Order = 10, EmitDefaultValue = false)]
        public string Notes { get; set; }

        [DataMember(Order = 20)]
        public string AvatarMax { get; set; }

        [DataMember(Order = 20)]
        public string AvatarDefault { get; set; }
        
        [DataMember(Order = 20)]
        public string AvatarMedium { get; set; }

        [DataMember(Order = 20)]
        public string Avatar { get; set; }

        [DataMember(Order = 20)]
        public bool IsAdmin { get; set; }

        [DataMember(Order = 20)]
        public bool IsLDAP { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public List<string> ListAdminModules { get; set; }

        [DataMember(Order = 20)]
        public bool IsOwner { get; set; }

        [DataMember(Order = 2)]
        public bool IsVisitor { get; set; }

        [DataMember(Order = 20, EmitDefaultValue = false)]
        public string CultureName { get; set; }


        [DataMember(Order = 11, EmitDefaultValue = false)]
        protected string MobilePhone { get; set; }

        [DataMember(Order = 11, EmitDefaultValue = false)]
        protected MobilePhoneActivationStatus MobilePhoneActivationStatus { get; set; }

        [DataMember(Order = 20)]
        public bool IsSSO { get; set; }
        public TenantManager TenantManager { get; }

        public EmployeeWraperFull()
        {
        }

        public EmployeeWraperFull(
            UserInfo userInfo, 
            ApiContext context, 
            UserManager userManager, 
            UserPhotoManager userphotomanager, 
            WebItemSecurity webItemSecurity,
            TenantManager tenantManager, 
            CommonLinkUtility commonLinkUtility,
            DisplayUserSettings displayUserSettings)
            : base(userInfo, context, displayUserSettings, userphotomanager, commonLinkUtility)
        {
            UserName = userInfo.UserName;
            FirstName = userInfo.FirstName;
            LastName = userInfo.LastName;
            Birthday = ApiDateTime.FromDate(tenantManager, userInfo.BirthDate);

            if (userInfo.Sex.HasValue)
                Sex = userInfo.Sex.Value ? "male" : "female";

            Status = userInfo.Status;
            ActivationStatus = userInfo.ActivationStatus & ~EmployeeActivationStatus.AutoGenerated;
            Terminated = ApiDateTime.FromDate(tenantManager, userInfo.TerminatedDate);
            WorkFrom = ApiDateTime.FromDate(tenantManager, userInfo.WorkFromDate);

            Email = userInfo.Email;

            if (!string.IsNullOrEmpty(userInfo.Location))
            {
                Location = userInfo.Location;
            }

            if (!string.IsNullOrEmpty(userInfo.Notes))
            {
                Notes = userInfo.Notes;
            }

            if (!string.IsNullOrEmpty(userInfo.MobilePhone))
            {
                MobilePhone = userInfo.MobilePhone;
            }

            MobilePhoneActivationStatus = userInfo.MobilePhoneActivationStatus;

            if (!string.IsNullOrEmpty(userInfo.CultureName))
            {
                CultureName = userInfo.CultureName;
            }

            FillConacts(userInfo);

            if (context.Check("groups") || context.Check("department"))
            {
                var groups = userManager.GetUserGroups(userInfo.ID)
                    .Select(x => new GroupWrapperSummary(x, userManager))
                    .ToList();

                if (groups.Count > 0)
                {
                    Groups = groups;
                    Department = string.Join(", ", Groups.Select(d => d.Name.HtmlEncode()));
                }
                else
                {
                    Department = "";
                }
            }

            var userInfoLM = userInfo.LastModified.GetHashCode();

            if (context.Check("avatarDefault"))
            {
                AvatarDefault = Convert.ToBase64String(userManager.GetUserPhoto(userInfo.ID));
            }

            if (context.Check("avatarMax"))
            {
                AvatarMax = userphotomanager.GetMaxPhotoURL(userInfo.ID, out var isdef) + (isdef ? "" : $"?_={userInfoLM}");
            }

            if (context.Check("avatarMedium"))
            {
                AvatarMedium = userphotomanager.GetMediumPhotoURL(userInfo.ID, out var isdef) + (isdef ? "" : $"?_={userInfoLM}");
            }

            if (context.Check("avatar"))
            {
                Avatar = userphotomanager.GetBigPhotoURL(userInfo.ID, out var isdef) + (isdef ? "" : $"?_={userInfoLM}");
            }

            if (context.Check("listAdminModules"))
            {
                var listAdminModules = userInfo.GetListAdminModules(webItemSecurity);

                if (listAdminModules.Any())
                    ListAdminModules = listAdminModules;
            }

            IsVisitor = userInfo.IsVisitor(userManager);
            IsAdmin = userInfo.IsAdmin(userManager);
            IsOwner = userInfo.IsOwner(context.Tenant);
            IsLDAP = userInfo.IsLDAP();
            IsSSO = userInfo.IsSSO();
            TenantManager = tenantManager;
        }

        private void FillConacts(UserInfo userInfo)
        {
            if (userInfo.Contacts == null) return;

            var contacts = new List<Contact>();

            for (var i = 0; i < userInfo.Contacts.Count; i += 2)
            {
                if (i + 1 < userInfo.Contacts.Count)
                {
                    contacts.Add(new Contact(userInfo.Contacts[i], userInfo.Contacts[i + 1]));
                }
            }

            if (contacts.Any())
            {
                Contacts = contacts;
            }
        }

        public static EmployeeWraperFull GetFull(
            Guid userId, 
            ApiContext context, 
            UserManager userManager, 
            UserPhotoManager userPhotoManager, 
            WebItemSecurity webItemSecurity,
            TenantManager tenantManager, 
            CommonLinkUtility commonLinkUtility,
            DisplayUserSettings displayUserSettings)
        {
            try
            {
                return GetFull(userManager.GetUsers(userId), context, userManager, userPhotoManager, webItemSecurity, tenantManager, commonLinkUtility, displayUserSettings);

            }
            catch (Exception)
            {
                return GetFull(Constants.LostUser, context, userManager, userPhotoManager, webItemSecurity, tenantManager, commonLinkUtility, displayUserSettings);
            }
        }

        public static EmployeeWraperFull GetFull(
            UserInfo userInfo, 
            ApiContext context, 
            UserManager userManager, 
            UserPhotoManager userPhotoManager,
            WebItemSecurity webItemSecurity,
            TenantManager tenantManager, 
            CommonLinkUtility commonLinkUtility,
            DisplayUserSettings displayUserSettings)
        {
            return new EmployeeWraperFull(userInfo, context, userManager, userPhotoManager, webItemSecurity, tenantManager, commonLinkUtility, displayUserSettings);
        }

        public new static EmployeeWraperFull GetSample()
        {
            return new EmployeeWraperFull
            {
                Avatar = "url to big avatar",
                AvatarSmall = "url to small avatar",
                AvatarMax = "url to max avatar",
                Contacts = new List<Contact> { Contact.GetSample() },
                Email = "my@gmail.com",
                FirstName = "Mike",
                Id = Guid.Empty,
                IsAdmin = false,
                ListAdminModules = new List<string> { "projects", "crm" },
                UserName = "Mike.Zanyatski",
                LastName = "Zanyatski",
                Title = "Manager",
                Groups = new List<GroupWrapperSummary> { GroupWrapperSummary.GetSample() },
                AvatarMedium = "url to medium avatar",
                Birthday = ApiDateTime.GetSample(),
                Department = "Marketing",
                Location = "Palo Alto",
                Notes = "Notes to worker",
                Sex = "male",
                Status = EmployeeStatus.Active,
                WorkFrom = ApiDateTime.GetSample(),
                Terminated = ApiDateTime.GetSample(),
                CultureName = "en-EN",
                IsLDAP = false,
                IsSSO = false
            };
        }
    }
}