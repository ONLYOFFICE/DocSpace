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


#if DEBUG
namespace ASC.Core.Common.Tests
{
    using System;
    using System.Threading;
    using ASC.Core.Users;
    using NUnit.Framework;
    using System.Diagnostics;

    [TestFixture]
    public class UserManagerTest
    {
        [Test]
        public void SearchUsers()
        {
            var tenant = CoreContext.TenantManager.SetCurrentTenant(0);
            var users = CoreContext.UserManager.Search(tenant, null, EmployeeStatus.Active);
            Assert.AreEqual(0, users.Length);

            users = CoreContext.UserManager.Search(tenant, "", EmployeeStatus.Active);
            Assert.AreEqual(0, users.Length);

            users = CoreContext.UserManager.Search(tenant, "  ", EmployeeStatus.Active);
            Assert.AreEqual(0, users.Length);

            users = CoreContext.UserManager.Search(tenant, "АбРаМсКй", EmployeeStatus.Active);
            Assert.AreEqual(0, users.Length);

            users = CoreContext.UserManager.Search(tenant, "АбРаМсКий", EmployeeStatus.Active);
            Assert.AreEqual(0, users.Length);//Абрамский уволился

            users = CoreContext.UserManager.Search(tenant, "АбРаМсКий", EmployeeStatus.All);
            Assert.AreNotEqual(0, users.Length);

            users = CoreContext.UserManager.Search(tenant, "иванов николай", EmployeeStatus.Active);
            Assert.AreNotEqual(0, users.Length);

            users = CoreContext.UserManager.Search(tenant, "ведущий програм", EmployeeStatus.Active);
            Assert.AreNotEqual(0, users.Length);

            users = CoreContext.UserManager.Search(tenant, "баннов лев", EmployeeStatus.Active, new Guid("613fc896-3ddd-4de1-a567-edbbc6cf1fc8"));
            Assert.AreNotEqual(0, users.Length);

            users = CoreContext.UserManager.Search(tenant, "иванов николай", EmployeeStatus.Active, new Guid("613fc896-3ddd-4de1-a567-edbbc6cf1fc8"));
            Assert.AreEqual(0, users);
        }

        [Test]
        public void DepartmentManagers()
        {
            var tenant = CoreContext.TenantManager.SetCurrentTenant(1024);

            var deps = CoreContext.UserManager.GetDepartments(tenant.TenantId);
            var users = CoreContext.UserManager.GetUsers(tenant);

            var g1 = deps[0];
            var ceo = users[0];
            var u1 = users[1];
            var u2 = users[2];

            var ceoTemp = CoreContext.UserManager.GetCompanyCEO(tenant.TenantId);
            CoreContext.UserManager.SetCompanyCEO(tenant.TenantId, ceo.ID);
            ceoTemp = CoreContext.UserManager.GetCompanyCEO(tenant.TenantId);
            Assert.AreEqual(ceo, ceoTemp);

            Thread.Sleep(TimeSpan.FromSeconds(6));
            ceoTemp = CoreContext.UserManager.GetCompanyCEO(tenant.TenantId);
            Assert.AreEqual(ceo, ceoTemp);

            CoreContext.UserManager.SetDepartmentManager(tenant.TenantId, g1.ID, u1.ID);

            CoreContext.UserManager.SetDepartmentManager(tenant.TenantId, g1.ID, u2.ID);
        }

        [Test]
        public void UserGroupsPerformanceTest()
        {
            var tenant = CoreContext.TenantManager.SetCurrentTenant(0);

            foreach (var u in CoreContext.UserManager.GetUsers(tenant))
            {
                var groups = CoreContext.UserManager.GetGroups(tenant.TenantId, Guid.Empty);
                Assert.IsNotNull(groups);
                foreach (var g in CoreContext.UserManager.GetUserGroups(tenant, u.ID))
                {
                    var manager = CoreContext.UserManager.GetUsers(tenant.TenantId, CoreContext.UserManager.GetDepartmentManager(tenant.TenantId, g.ID)).UserName;
                }
            }
            var stopwatch = Stopwatch.StartNew();
            foreach (var u in CoreContext.UserManager.GetUsers(tenant))
            {
                var groups = CoreContext.UserManager.GetGroups(tenant.TenantId, Guid.Empty);
                Assert.IsNotNull(groups);
                foreach (var g in CoreContext.UserManager.GetUserGroups(tenant, u.ID))
                {
                    var manager = CoreContext.UserManager.GetUsers(tenant.TenantId, CoreContext.UserManager.GetDepartmentManager(tenant.TenantId, g.ID)).UserName;
                }
            }
            stopwatch.Stop();

            stopwatch.Restart();
            var users = CoreContext.UserManager.GetUsersByGroup(tenant, Constants.GroupUser.ID);
            var visitors = CoreContext.UserManager.GetUsersByGroup(tenant, Constants.GroupVisitor.ID);
            var all = CoreContext.UserManager.GetUsers(tenant);
            Assert.IsNotNull(users);
            Assert.IsNotNull(visitors);
            Assert.IsNotNull(all);
            stopwatch.Stop();
        }
    }
}
#endif
