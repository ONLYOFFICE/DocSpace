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
            var scopeClass = scope.ServiceProvider.GetService<Scope>();

            var users = scopeClass.UserManager.Search(null, EmployeeStatus.Active);
            Assert.AreEqual(0, users.Length);

            users = scopeClass.UserManager.Search("", EmployeeStatus.Active);
            Assert.AreEqual(0, users.Length);

            users = scopeClass.UserManager.Search("  ", EmployeeStatus.Active);
            Assert.AreEqual(0, users.Length);

            users = scopeClass.UserManager.Search("АбРаМсКй", EmployeeStatus.Active);
            Assert.AreEqual(0, users.Length);

            users = scopeClass.UserManager.Search("АбРаМсКий", EmployeeStatus.Active);
            Assert.AreEqual(0, users.Length);//Абрамский уволился

            users = scopeClass.UserManager.Search("АбРаМсКий", EmployeeStatus.All);
            Assert.AreNotEqual(0, users.Length);

            users = scopeClass.UserManager.Search("иванов николай", EmployeeStatus.Active);
            Assert.AreNotEqual(0, users.Length);

            users = scopeClass.UserManager.Search("ведущий програм", EmployeeStatus.Active);
            Assert.AreNotEqual(0, users.Length);

            users = scopeClass.UserManager.Search("баннов лев", EmployeeStatus.Active, new Guid("613fc896-3ddd-4de1-a567-edbbc6cf1fc8"));
            Assert.AreNotEqual(0, users.Length);

            users = scopeClass.UserManager.Search("иванов николай", EmployeeStatus.Active, new Guid("613fc896-3ddd-4de1-a567-edbbc6cf1fc8"));
            Assert.AreEqual(0, users);
        }

        [Test]
        public void DepartmentManagers()
        {
            using var scope = ServiceProvider.CreateScope();
            var scopeClass = scope.ServiceProvider.GetService<Scope>();
            var tenant = scopeClass.TenantManager.SetCurrentTenant(1024);

            var deps = scopeClass.UserManager.GetDepartments();
            var users = scopeClass.UserManager.GetUsers();

            var g1 = deps[0];
            var ceo = users[0];
            var u1 = users[1];
            var u2 = users[2];
            _ = scopeClass.UserManager.GetCompanyCEO();
            scopeClass.UserManager.SetCompanyCEO(ceo.ID);
            var ceoTemp = scopeClass.UserManager.GetCompanyCEO();
            Assert.AreEqual(ceo, ceoTemp);

            Thread.Sleep(TimeSpan.FromSeconds(6));
            ceoTemp = scopeClass.UserManager.GetCompanyCEO();
            Assert.AreEqual(ceo, ceoTemp);

            scopeClass.UserManager.SetDepartmentManager(g1.ID, u1.ID);

            scopeClass.UserManager.SetDepartmentManager(g1.ID, u2.ID);
        }

        [Test]
        public void UserGroupsPerformanceTest()
        {
            using var scope = ServiceProvider.CreateScope();
            var scopeClass = scope.ServiceProvider.GetService<Scope>();
            var tenant = scopeClass.TenantManager.SetCurrentTenant(0);

            foreach (var u in scopeClass.UserManager.GetUsers())
            {
                var groups = scopeClass.UserManager.GetGroups(Guid.Empty);
                Assert.IsNotNull(groups);
                foreach (var g in scopeClass.UserManager.GetUserGroups(u.ID))
                {
                    var manager = scopeClass.UserManager.GetUsers(scopeClass.UserManager.GetDepartmentManager(g.ID)).UserName;
                }
            }
            var stopwatch = Stopwatch.StartNew();
            foreach (var u in scopeClass.UserManager.GetUsers())
            {
                var groups = scopeClass.UserManager.GetGroups(Guid.Empty);
                Assert.IsNotNull(groups);
                foreach (var g in scopeClass.UserManager.GetUserGroups(u.ID))
                {
                    var manager = scopeClass.UserManager.GetUsers(scopeClass.UserManager.GetDepartmentManager(g.ID)).UserName;
                }
            }
            stopwatch.Stop();

            stopwatch.Restart();
            var users = scopeClass.UserManager.GetUsersByGroup(Constants.GroupUser.ID);
            var visitors = scopeClass.UserManager.GetUsersByGroup(Constants.GroupVisitor.ID);
            var all = scopeClass.UserManager.GetUsers();
            Assert.IsNotNull(users);
            Assert.IsNotNull(visitors);
            Assert.IsNotNull(all);
            stopwatch.Stop();
        }
    }

    class Scope
    {
        internal UserManager UserManager { get; }
        internal TenantManager TenantManager { get; }

        public Scope(UserManager userManager, TenantManager tenantManager)
        {
            UserManager = userManager;
            TenantManager = tenantManager;
        }
    }
}
#endif
