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
using ASC.Common.Utils;
using ASC.Core;
using ASC.Core.Users;
using ASC.Web.Core;
using ASC.Web.Core.Users;
using ASC.Web.Studio.Utility;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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
        public List<Contact> Contacts { get; set; }

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
        public string MobilePhone { get; set; }

        [DataMember(Order = 11, EmitDefaultValue = false)]
        public MobilePhoneActivationStatus MobilePhoneActivationStatus { get; set; }

        [DataMember(Order = 20)]
        public bool IsSSO { get; set; }

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

    public class EmployeeWraperFullHelper : EmployeeWraperHelper
    {
        public ApiContext Context { get; }
        public UserManager UserManager { get; }
        public WebItemSecurity WebItemSecurity { get; }
        public TenantManager TenantManager { get; }
        public TimeZoneConverter TimeZoneConverter { get; }

        public EmployeeWraperFullHelper(
            ApiContext context,
            UserManager userManager,
            UserPhotoManager userPhotoManager,
            WebItemSecurity webItemSecurity,
            TenantManager tenantManager,
            CommonLinkUtility commonLinkUtility,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            TimeZoneConverter timeZoneConverter)
        : base(context, displayUserSettingsHelper, userPhotoManager, commonLinkUtility)
        {
            Context = context;
            UserManager = userManager;
            WebItemSecurity = webItemSecurity;
            TenantManager = tenantManager;
            TimeZoneConverter = timeZoneConverter;
        }

        public EmployeeWraperFull GetFull(UserInfo userInfo)
        {
            var result = new EmployeeWraperFull
            {
                UserName = userInfo.UserName,
                FirstName = userInfo.FirstName,
                LastName = userInfo.LastName,
                Birthday = ApiDateTime.FromDate(TenantManager, TimeZoneConverter, userInfo.BirthDate),
                Status = userInfo.Status,
                ActivationStatus = userInfo.ActivationStatus & ~EmployeeActivationStatus.AutoGenerated,
                Terminated = ApiDateTime.FromDate(TenantManager, TimeZoneConverter, userInfo.TerminatedDate),
                WorkFrom = ApiDateTime.FromDate(TenantManager, TimeZoneConverter, userInfo.WorkFromDate),
                Email = userInfo.Email,
                IsVisitor = userInfo.IsVisitor(UserManager),
                IsAdmin = userInfo.IsAdmin(UserManager),
                IsOwner = userInfo.IsOwner(Context.Tenant),
                IsLDAP = userInfo.IsLDAP(),
                IsSSO = userInfo.IsSSO()
            };

            Init(result, userInfo);

            if (userInfo.Sex.HasValue)
            {
                result.Sex = userInfo.Sex.Value ? "male" : "female";
            }

            if (!string.IsNullOrEmpty(userInfo.Location))
            {
                result.Location = userInfo.Location;
            }

            if (!string.IsNullOrEmpty(userInfo.Notes))
            {
                result.Notes = userInfo.Notes;
            }

            if (!string.IsNullOrEmpty(userInfo.MobilePhone))
            {
                result.MobilePhone = userInfo.MobilePhone;
            }

            result.MobilePhoneActivationStatus = userInfo.MobilePhoneActivationStatus;

            if (!string.IsNullOrEmpty(userInfo.CultureName))
            {
                result.CultureName = userInfo.CultureName;
            }

            FillConacts(result, userInfo);

            if (Context.Check("groups") || Context.Check("department"))
            {
                var groups = UserManager.GetUserGroups(userInfo.ID)
                    .Select(x => new GroupWrapperSummary(x, UserManager))
                    .ToList();

                if (groups.Count > 0)
                {
                    result.Groups = groups;
                    result.Department = string.Join(", ", result.Groups.Select(d => d.Name.HtmlEncode()));
                }
                else
                {
                    result.Department = "";
                }
            }

            var userInfoLM = userInfo.LastModified.GetHashCode();

            if (Context.Check("avatarMax"))
            {
                result.AvatarMax = UserPhotoManager.GetMaxPhotoURL(userInfo.ID, out var isdef) + (isdef ? "" : $"?_={userInfoLM}");
            }

            if (Context.Check("avatarMedium"))
            {
                result.AvatarMedium = UserPhotoManager.GetMediumPhotoURL(userInfo.ID, out var isdef) + (isdef ? "" : $"?_={userInfoLM}");
            }

            if (Context.Check("avatar"))
            {
                result.Avatar = UserPhotoManager.GetBigPhotoURL(userInfo.ID, out var isdef) + (isdef ? "" : $"?_={userInfoLM}");
            }

            if (Context.Check("listAdminModules"))
            {
                var listAdminModules = userInfo.GetListAdminModules(WebItemSecurity);

                if (listAdminModules.Any())
                    result.ListAdminModules = listAdminModules;
            }

            return result;
        }

        private void FillConacts(EmployeeWraperFull employeeWraperFull, UserInfo userInfo)
        {
            if (userInfo.ContactsList == null) return;

            var contacts = new List<Contact>();

            for (var i = 0; i < userInfo.ContactsList.Count; i += 2)
            {
                if (i + 1 < userInfo.ContactsList.Count)
                {
                    contacts.Add(new Contact(userInfo.ContactsList[i], userInfo.ContactsList[i + 1]));
                }
            }

            if (contacts.Any())
            {
                employeeWraperFull.Contacts = contacts;
            }
        }
    }

    public static class EmployeeWraperFullExtension
    {
        public static IServiceCollection AddEmployeeWraperFull(this IServiceCollection services)
        {
            services.TryAddScoped<EmployeeWraperFullHelper>();

            return services
                .AddTenantManagerService()
                .AddWebItemSecurity()
                .AddUserManagerService()
                .AddEmployeeWraper();
        }
    }
}