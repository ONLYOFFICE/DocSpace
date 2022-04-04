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
using System.Linq.Expressions;


using ASC.Api.Core;
using ASC.Common;
using ASC.Core;
using ASC.Core.Common.EF;
using ASC.Core.Users;
using ASC.Web.Core;
using ASC.Web.Core.Users;
using ASC.Web.Studio.Utility;

namespace ASC.Web.Api.Models
{
    public class EmployeeWraperFull : EmployeeWraper
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string UserName { get; set; }

        public string Email { get; set; }

        public List<Contact> Contacts { get; set; }

        public ApiDateTime Birthday { get; set; }

        public string Sex { get; set; }

        public EmployeeStatus Status { get; set; }

        public EmployeeActivationStatus ActivationStatus { get; set; }

        public ApiDateTime Terminated { get; set; }

        public string Department { get; set; }

        public ApiDateTime WorkFrom { get; set; }

        public List<GroupWrapperSummary> Groups { get; set; }

        public string Location { get; set; }

        public string Notes { get; set; }

        public string AvatarMax { get; set; }

        public string AvatarMedium { get; set; }

        public string Avatar { get; set; }

        public bool IsAdmin { get; set; }

        public bool IsLDAP { get; set; }

        public List<string> ListAdminModules { get; set; }

        public bool IsOwner { get; set; }

        public bool IsVisitor { get; set; }

        public string CultureName { get; set; }


        public string MobilePhone { get; set; }

        public MobilePhoneActivationStatus MobilePhoneActivationStatus { get; set; }

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

    [Scope]
    public class EmployeeWraperFullHelper : EmployeeWraperHelper
    {
        private ApiContext Context { get; }
        private WebItemSecurity WebItemSecurity { get; }
        private ApiDateTimeHelper ApiDateTimeHelper { get; }

        public EmployeeWraperFullHelper(
            ApiContext context,
            UserManager userManager,
            UserPhotoManager userPhotoManager,
            WebItemSecurity webItemSecurity,
            CommonLinkUtility commonLinkUtility,
            DisplayUserSettingsHelper displayUserSettingsHelper,
            ApiDateTimeHelper apiDateTimeHelper)
        : base(context, displayUserSettingsHelper, userPhotoManager, commonLinkUtility, userManager)
        {
            Context = context;
            WebItemSecurity = webItemSecurity;
            ApiDateTimeHelper = apiDateTimeHelper;
        }

        public static Expression<Func<User, UserInfo>> GetExpression(ApiContext apiContext)
        {
            if (apiContext?.Fields == null) return null;
            var newExpr = Expression.New(typeof(UserInfo));

            //i => new UserInfo { ID = i.id } 
            var parameter = Expression.Parameter(typeof(User), "i");
            var bindExprs = new List<MemberAssignment>();

            if (apiContext.Check("Id"))
            {
                bindExprs.Add(Expression.Bind(typeof(UserInfo).GetProperty("ID"), Expression.Property(parameter, typeof(User).GetProperty("Id"))));
            }

            var body = Expression.MemberInit(newExpr, bindExprs);
            var lambda = Expression.Lambda<Func<User, UserInfo>>(body, parameter);

            return lambda;
        }

        public EmployeeWraperFull GetSimple(UserInfo userInfo)
        {
            var result = new EmployeeWraperFull
            {
                FirstName = userInfo.FirstName,
                LastName = userInfo.LastName,
            };

            FillGroups(result, userInfo);

            var photoData = UserPhotoManager.GetUserPhotoData(userInfo.ID, UserPhotoManager.BigFotoSize);

            if (photoData != null)
            {
                result.Avatar = "data:image/png;base64," + Convert.ToBase64String(photoData);
            }

            return result;
        }

        public EmployeeWraperFull GetFull(UserInfo userInfo)
        {
            var result = new EmployeeWraperFull
            {
                UserName = userInfo.UserName,
                FirstName = userInfo.FirstName,
                LastName = userInfo.LastName,
                Birthday = ApiDateTimeHelper.Get(userInfo.BirthDate),
                Status = userInfo.Status,
                ActivationStatus = userInfo.ActivationStatus & ~EmployeeActivationStatus.AutoGenerated,
                Terminated = ApiDateTimeHelper.Get(userInfo.TerminatedDate),
                WorkFrom = ApiDateTimeHelper.Get(userInfo.WorkFromDate),
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
            FillGroups(result, userInfo);

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

                if (listAdminModules.Count > 0)
                    result.ListAdminModules = listAdminModules;
            }

            return result;
        }

        private void FillGroups(EmployeeWraperFull result, UserInfo userInfo)
        {
            if (!Context.Check("groups") && !Context.Check("department"))
            {
                return;
            }

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

            if (contacts.Count > 0)
            {
                employeeWraperFull.Contacts = contacts;
            }
        }
    }
}