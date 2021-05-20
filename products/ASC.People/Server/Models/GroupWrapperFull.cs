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


using ASC.Common;
using ASC.Core;
using ASC.Core.Users;

namespace ASC.Web.Api.Models
{
    public class GroupWrapperFull
    {
        public string Description { get; set; }

        public string Name { get; set; }

        public Guid? Parent { get; set; }

        public Guid Category { get; set; }

        public Guid Id { get; set; }

        public EmployeeWraper Manager { get; set; }

        public List<EmployeeWraper> Members { get; set; }

        public static GroupWrapperFull GetSample()
        {
            return new GroupWrapperFull
            {
                Id = Guid.NewGuid(),
                Manager = EmployeeWraper.GetSample(),
                Category = Guid.NewGuid(),
                Name = "Sample group",
                Parent = Guid.NewGuid(),
                Members = new List<EmployeeWraper> { EmployeeWraper.GetSample() }
            };
        }
    }

    [Scope]
    public class GroupWraperFullHelper
    {
        private UserManager UserManager { get; }
        private EmployeeWraperHelper EmployeeWraperHelper { get; }

        public GroupWraperFullHelper(UserManager userManager, EmployeeWraperHelper employeeWraperHelper)
        {
            UserManager = userManager;
            EmployeeWraperHelper = employeeWraperHelper;
        }

        public GroupWrapperFull Get(GroupInfo group, bool includeMembers)
        {
            var result = new GroupWrapperFull
            {
                Id = group.ID,
                Category = group.CategoryID,
                Parent = group.Parent != null ? group.Parent.ID : Guid.Empty,
                Name = group.Name,
                Manager = EmployeeWraperHelper.Get(UserManager.GetUsers(UserManager.GetDepartmentManager(group.ID)))
            };

            if (includeMembers)
            {
                result.Members = new List<EmployeeWraper>(UserManager.GetUsersByGroup(group.ID).Select(EmployeeWraperHelper.Get));
            }

            return result;
        }
    }
}