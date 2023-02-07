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
    using System.Diagnostics;
    using System.Threading;

    using ASC.Core.Users;

    using Microsoft.Extensions.DependencyInjection;

    using NUnit.Framework;

    [TestFixture]
    public class UserManagerTest
    {
        IServiceProvider ServiceProvider { get; set; }

        [Test]
        public void SearchUsers()
        {
            using var scope = ServiceProvider.CreateScope();
            var userManager = scope.ServiceProvider.GetService<UserManager>();
            var tenantManager = scope.ServiceProvider.GetService<TenantManager>();
            var tenant = tenantManager.SetCurrentTenant(0);

            var users = userManager.Search(null, EmployeeStatus.Active);
            Assert.AreEqual(0, users.Length);

            users = userManager.Search("", EmployeeStatus.Active);
            Assert.AreEqual(0, users.Length);

            users = userManager.Search("  ", EmployeeStatus.Active);
            Assert.AreEqual(0, users.Length);

            users = userManager.Search("АбРаМсКй", EmployeeStatus.Active);
            Assert.AreEqual(0, users.Length);

            users = userManager.Search("АбРаМсКий", EmployeeStatus.Active);
            Assert.AreEqual(0, users.Length);//Абрамский уволился

            users = userManager.Search("АбРаМсКий", EmployeeStatus.All);
            Assert.AreNotEqual(0, users.Length);

            users = userManager.Search("иванов николай", EmployeeStatus.Active);
            Assert.AreNotEqual(0, users.Length);

            users = userManager.Search("ведущий програм", EmployeeStatus.Active);
            Assert.AreNotEqual(0, users.Length);

            users = userManager.Search("баннов лев", EmployeeStatus.Active, new Guid("613fc896-3ddd-4de1-a567-edbbc6cf1fc8"));
            Assert.AreNotEqual(0, users.Length);

            users = userManager.Search("иванов николай", EmployeeStatus.Active, new Guid("613fc896-3ddd-4de1-a567-edbbc6cf1fc8"));
            Assert.AreEqual(0, users);
        }

        [Test]
        public void DepartmentManagers()
        {
            using var scope = ServiceProvider.CreateScope();
            var scopeClass = scope.ServiceProvider.GetService<UserManagerTestScope>();
            var (userManager, tenantManager) = scopeClass;
            var tenant = tenantManager.SetCurrentTenant(1024);

            var deps = userManager.GetDepartments();
            var users = userManager.GetUsers();

            var g1 = deps[0];
            var ceo = users[0];
            var u1 = users[1];
            var u2 = users[2];
            userManager.GetCompanyCEO();
            userManager.SetCompanyCEO(ceo.Id);
            var ceoTemp = userManager.GetCompanyCEO();
            Assert.AreEqual(ceo, ceoTemp);

            Thread.Sleep(TimeSpan.FromSeconds(6));
            ceoTemp = userManager.GetCompanyCEO();
            Assert.AreEqual(ceo, ceoTemp);

            userManager.SetDepartmentManager(g1.ID, u1.Id);

            userManager.SetDepartmentManager(g1.ID, u2.Id);
        }

        [Test]
        public void UserGroupsPerformanceTest()
        {
            using var scope = ServiceProvider.CreateScope();
            var scopeClass = scope.ServiceProvider.GetService<UserManagerTestScope>();
            (var userManager, var tenantManager) = scopeClass;
            var tenant = tenantManager.SetCurrentTenant(0);

            foreach (var u in userManager.GetUsers())
            {
                var groups = userManager.GetGroups(Guid.Empty);
                Assert.IsNotNull(groups);
                foreach (var g in userManager.GetUserGroups(u.Id))
                {
                    var manager = userManager.GetUsers(userManager.GetDepartmentManager(g.ID)).UserName;
                }
            }
            var stopwatch = Stopwatch.StartNew();
            foreach (var u in userManager.GetUsers())
            {
                var groups = userManager.GetGroups(Guid.Empty);
                Assert.IsNotNull(groups);
                foreach (var g in userManager.GetUserGroups(u.Id))
                {
                    var manager = userManager.GetUsers(userManager.GetDepartmentManager(g.ID)).UserName;
                }
            }
            stopwatch.Stop();

            stopwatch.Restart();
            var users = userManager.GetUsersByGroup(Constants.GroupUser.ID);
            var visitors = userManager.GetUsersByGroup(Constants.GroupVisitor.ID);
            var all = userManager.GetUsers();
            Assert.IsNotNull(users);
            Assert.IsNotNull(visitors);
            Assert.IsNotNull(all);
            stopwatch.Stop();
        }
    }

    public class UserManagerTestScope
    {
        private UserManager UserManager { get; }
        private TenantManager TenantManager { get; }

        public UserManagerTestScope(UserManager userManager, TenantManager tenantManager)
        {
            UserManager = userManager;
            TenantManager = tenantManager;
        }

        public void Deconstruct(out UserManager userManager, out TenantManager tenantManager)
        {
            userManager = UserManager;
            tenantManager = TenantManager;
        }
    }
}
#endif
