///*
// *
// * (c) Copyright Ascensio System Limited 2010-2020
// *
// * This program is freeware. You can redistribute it and/or modify it under the terms of the GNU 
// * General Public License (GPL) version 3 as published by the Free Software Foundation (https://www.gnu.org/copyleft/gpl.html). 
// * In accordance with Section 7(a) of the GNU GPL its Section 15 shall be amended to the effect that 
// * Ascensio System SIA expressly excludes the warranty of non-infringement of any third-party rights.
// *
// * THIS PROGRAM IS DISTRIBUTED WITHOUT ANY WARRANTY; WITHOUT EVEN THE IMPLIED WARRANTY OF MERCHANTABILITY OR
// * FITNESS FOR A PARTICULAR PURPOSE. For more details, see GNU GPL at https://www.gnu.org/copyleft/gpl.html
// *
// * You can contact Ascensio System SIA by email at sales@onlyoffice.com
// *
// * The interactive user interfaces in modified source and object code versions of ONLYOFFICE must display 
// * Appropriate Legal Notices, as required under Section 5 of the GNU GPL version 3.
// *
// * Pursuant to Section 7 § 3(b) of the GNU GPL you must retain the original ONLYOFFICE logo which contains 
// * relevant author attributions when distributing the software. If the display of the logo in its graphic 
// * form is not reasonably feasible for technical reasons, you must include the words "Powered by ONLYOFFICE" 
// * in every copy of the program you distribute. 
// * Pursuant to Section 7 § 3(e) we decline to grant you any rights under trademark law for use of our trademarks.
// *
//*/


//using System;
//using ASC.Core;
//using ASC.Core.Users;
//using ASC.Mail.Aggregator.Tests.Common.Utils;
//using ASC.Mail.Core;
//using ASC.Mail.Exceptions;
//using NUnit.Framework;

//namespace ASC.Mail.Aggregator.Tests.Common.UserFolders
//{
//    [TestFixture]
//    internal class CrudUserFoldersTests
//    {
//        private const int CURRENT_TENANT = 0;
//        public const string PASSWORD = "123456";
//        public const string DOMAIN = "gmail.com";

//        public UserInfo TestUser { get; private set; }

//        private EngineFactory _engineFactory;

//        [SetUp]
//        public void SetUp()
//        {
//            CoreContext.TenantManager.SetCurrentTenant(CURRENT_TENANT);
//            SecurityContext.AuthenticateMe(ASC.Core.Configuration.Constants.CoreSystem);

//            TestUser = TestHelper.CreateNewRandomEmployee();

//            _engineFactory = new EngineFactory(CURRENT_TENANT, TestUser.ID.ToString());
//        }

//        [TearDown]
//        public void CleanUp()
//        {
//            if (TestUser == null || TestUser.ID == Guid.Empty)
//                return;

//            CoreContext.TenantManager.SetCurrentTenant(CURRENT_TENANT);

//            SecurityContext.AuthenticateMe(ASC.Core.Configuration.Constants.CoreSystem);

//            CoreContext.UserManager.DeleteUser(TestUser.ID);

//            // Clear TestUser1 mail data
//            var eraser = _engineFactory.MailGarbageEngine;

//            eraser.ClearUserMail(TestUser.ID, CoreContext.TenantManager.GetCurrentTenant());
//        }

//        [Test]
//        public void CreateFolderTest()
//        {
//            var engine = _engineFactory.UserFolderEngine;

//            var folder = engine.Create("Test folder");

//            Assert.Greater(folder.Id, 0);
//        }

//        [Test]
//        public void CreateFolderWithAlreadyExistingNameTest()
//        {
//            var engine = _engineFactory.UserFolderEngine;

//            const string name = "Test folder";

//            var folder = engine.Create(name);

//            Assert.Greater(folder.Id, 0);

//            Assert.Throws<AlreadyExistsFolderException>(() =>
//            {
//                engine.Create(name);
//            });
//        }

//        [Test]
//        public void CreateFolderWithoutParentTest()
//        {
//            var engine = _engineFactory.UserFolderEngine;

//            Assert.Throws<ArgumentException>(() =>
//            {
//                engine.Create("Test folder", 777);
//            });
//        }

//        [Test]
//        public void CreateFolderWithoutNameTest()
//        {
//            var engine = _engineFactory.UserFolderEngine;

//            Assert.Throws<EmptyFolderException>(() =>
//            {
//                engine.Create("");
//            });
//        }

//        [Test]
//        public void CreateSubFolderTest()
//        {
//            var engine = _engineFactory.UserFolderEngine;

//            var folder = engine.Create("Test folder");

//            Assert.Greater(folder.Id, 0);

//            var subFolder = engine.Create("Test sub folder", folder.Id);

//            Assert.Greater(subFolder.Id, 0);

//            var rootFolder = engine.Get(folder.Id);

//            Assert.IsNotNull(rootFolder);

//            Assert.AreEqual(1, rootFolder.FolderCount);
//        }

//        [Test]
//        public void RemoveSubFolderTest()
//        {
//            var engine = _engineFactory.UserFolderEngine;

//            var folder = engine.Create("Test folder");

//            Assert.Greater(folder.Id, 0);

//            var subFolder = engine.Create("Test sub folder", folder.Id);

//            Assert.Greater(subFolder.Id, 0);

//            var rootFolder = engine.Get(folder.Id);

//            Assert.IsNotNull(rootFolder);

//            Assert.AreEqual(1, rootFolder.FolderCount);

//            engine.Delete(subFolder.Id);

//            rootFolder = engine.Get(rootFolder.Id);

//            Assert.IsNotNull(rootFolder);

//            Assert.AreEqual(0, rootFolder.FolderCount);
//        }

//        [Test]
//        public void ChangeNameTest()
//        {
//            var engine = _engineFactory.UserFolderEngine;

//            const string name = "Folder Name";

//            var folder = engine.Create(name);

//            Assert.Greater(folder.Id, 0);

//            Assert.AreEqual(name, folder.Name);

//            const string new_name = "New Folder Name";

//            var resultFolder = engine.Update(folder.Id, new_name);

//            Assert.IsNotNull(resultFolder);

//            Assert.Greater(resultFolder.Id, 0);

//            Assert.AreEqual(0, resultFolder.FolderCount);

//            Assert.AreEqual(new_name, resultFolder.Name);

//            Assert.AreNotEqual(folder.Name, resultFolder.Name);
//        }

//        [Test]
//        public void ChangeNameToExistingTest()
//        {
//            var engine = _engineFactory.UserFolderEngine;

//            const string name1 = "Folder Name 1";

//            var folder1 = engine.Create(name1);

//            Assert.Greater(folder1.Id, 0);

//            Assert.AreEqual(name1, folder1.Name);

//            const string name2 = "New Folder Name";

//            var folder2 = engine.Create(name2);

//            Assert.Greater(folder2.Id, 0);

//            Assert.AreEqual(name2, folder2.Name);

//            Assert.Throws<AlreadyExistsFolderException>(() =>
//            {
//                engine.Update(folder2.Id, name1);
//            });
//        }

//        [Test]
//        public void MoveToBaseFolderTest()
//        {
//            var engine = _engineFactory.UserFolderEngine;

//            var baseFolder = engine.Create("Folder 1");

//            Assert.Greater(baseFolder.Id, 0);

//            var folder = engine.Create("Folder 1.1");

//            Assert.Greater(folder.Id, 0);

//            engine.Update(folder.Id, folder.Name, baseFolder.Id);

//            var resultBaseFolder = engine.Get(baseFolder.Id);

//            Assert.IsNotNull(resultBaseFolder);

//            Assert.AreEqual(0, resultBaseFolder.ParentId);

//            Assert.AreEqual(1, resultBaseFolder.FolderCount);

//            var resultFolder = engine.Get(folder.Id);

//            Assert.Greater(resultFolder.Id, 0);

//            Assert.AreEqual(baseFolder.Id, resultFolder.ParentId);
//        }

//        [Test]
//        public void MoveFromBaseFolderTest()
//        {
//            var engine = _engineFactory.UserFolderEngine;

//            var baseFolder = engine.Create("Folder 1");

//            Assert.Greater(baseFolder.Id, 0);

//            var folder = engine.Create("Folder 1.1", baseFolder.Id);

//            Assert.Greater(folder.Id, 0);

//            Assert.AreEqual(baseFolder.Id, folder.ParentId);

//            baseFolder = engine.Get(baseFolder.Id);

//            Assert.AreEqual(1, baseFolder.FolderCount);

//            engine.Update(folder.Id, folder.Name, 0);

//            var resultBaseFolder = engine.Get(baseFolder.Id);

//            Assert.IsNotNull(resultBaseFolder);

//            Assert.AreEqual(0, resultBaseFolder.FolderCount);

//            var resultFolder = engine.Get(folder.Id);

//            Assert.Greater(resultFolder.Id, 0);
//        }

//        [Test]
//        public void WrongMoveFolderTest()
//        {
//            var engine = _engineFactory.UserFolderEngine;

//            var baseFolder = engine.Create("Folder 1");

//            Assert.Greater(baseFolder.Id, 0);

//            var folder = engine.Create("Folder 1.1", baseFolder.Id);

//            Assert.Greater(folder.Id, 0);

//            Assert.AreEqual(baseFolder.Id, folder.ParentId);

//            Assert.Throws<MoveFolderException>(() =>
//            {
//                engine.Update(baseFolder.Id, baseFolder.Name, folder.Id);
//            });
//        }

//        [Test]
//        public void WrongChangeParentToCurrentTest()
//        {
//            var engine = _engineFactory.UserFolderEngine;

//            var baseFolder = engine.Create("Folder 1");

//            Assert.Greater(baseFolder.Id, 0);

//            var folder = engine.Create("Folder 1.1", baseFolder.Id);

//            Assert.Greater(folder.Id, 0);

//            Assert.AreEqual(baseFolder.Id, folder.ParentId);

//            Assert.Throws<ArgumentException>(() =>
//            {
//                engine.Update(folder.Id, folder.Name, folder.Id);
//            });
//        }

//        [Test]
//        public void WrongChangeParentToChildTest()
//        {
//            var engine = _engineFactory.UserFolderEngine;

//            var root = engine.Create("Root");

//            Assert.Greater(root.Id, 0);

//            var f1 = engine.Create("1", root.Id);

//            Assert.Greater(f1.Id, 0);

//            Assert.AreEqual(root.Id, f1.ParentId);

//            root = engine.Get(root.Id);

//            Assert.AreEqual(1, root.FolderCount);

//            var f11 = engine.Create("1.1", f1.Id);

//            Assert.Greater(f11.Id, 0);

//            Assert.AreEqual(f1.Id, f11.ParentId);

//            f1 = engine.Get(f1.Id);

//            Assert.AreEqual(1, f1.FolderCount);

//            var f111 = engine.Create("1.1.1", f11.Id);

//            Assert.Greater(f111.Id, 0);

//            Assert.AreEqual(f11.Id, f111.ParentId);

//            Assert.Throws<MoveFolderException>(() =>
//            {
//                engine.Update(f11.Id, f11.Name, f111.Id);
//            });
//        }

//        [Test]
//        public void DeleteFolderInTheMiddleTest()
//        {
//            var engine = _engineFactory.UserFolderEngine;

//            var root = engine.Create("Root");

//            Assert.Greater(root.Id, 0);

//            var f1 = engine.Create("1", root.Id);

//            Assert.Greater(f1.Id, 0);

//            Assert.AreEqual(root.Id, f1.ParentId);

//            root = engine.Get(root.Id);

//            Assert.AreEqual(1, root.FolderCount);

//            var f11 = engine.Create("1.1", f1.Id);

//            Assert.Greater(f11.Id, 0);

//            Assert.AreEqual(f1.Id, f11.ParentId);

//            f1 = engine.Get(f1.Id);

//            Assert.AreEqual(1, f1.FolderCount);

//            var f111 = engine.Create("1.1.1", f11.Id);

//            Assert.Greater(f111.Id, 0);

//            Assert.AreEqual(f11.Id, f111.ParentId);

//            f11 = engine.Get(f11.Id);

//            Assert.AreEqual(1, f11.FolderCount);

//            var f2 = engine.Create("2", root.Id);

//            Assert.Greater(f2.Id, 0);

//            Assert.AreEqual(root.Id, f2.ParentId);

//            f1 = engine.Get(root.Id);

//            Assert.AreEqual(4, f1.FolderCount);

//            engine.Delete(f11.Id);

//            f1 = engine.Get(root.Id);

//            Assert.AreEqual(2, f1.FolderCount);

//            engine.Delete(root.Id);

//            var list = engine.GetList();

//            Assert.IsEmpty(list);
//        }
//    }
//}
