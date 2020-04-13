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
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using ASC.Core;
//using ASC.Core.Users;
//using ASC.Mail.Aggregator.Tests.Common.Utils;
//using ASC.Mail.Core;
//using ASC.Mail.Models;
//using ASC.Mail.Enums;
//using ASC.Mail.Utils;
//using NUnit.Framework;

//namespace ASC.Mail.Aggregator.Tests.Common.UserFolders
//{
//    [TestFixture]
//    internal class CrudUserFoldersXMailTests
//    {
//        private const int CURRENT_TENANT = 0;
//        public const string PASSWORD = "123456";
//        public const string DOMAIN = "gmail.com";

//        private static readonly string TestFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
//            @"..\..\Data\");

//        private const string EML1_FILE_NAME = @"bad_encoding.eml";
//        private const string EML2_FILE_NAME = @"embed_image.eml";
//        private static readonly string Eml1Path = TestFolderPath + EML1_FILE_NAME;
//        private static readonly string Eml2Path = TestFolderPath + EML2_FILE_NAME;

//        public UserInfo TestUser { get; private set; }
//        private MailBoxData _mbox;

//        private EngineFactory _engineFactory;

//        [SetUp]
//        public void SetUp()
//        {
//            CoreContext.TenantManager.SetCurrentTenant(CURRENT_TENANT);
//            SecurityContext.AuthenticateMe(ASC.Core.Configuration.Constants.CoreSystem);

//            TestUser = TestHelper.CreateNewRandomEmployee();

//            _engineFactory = new EngineFactory(CURRENT_TENANT, TestUser.ID.ToString());

//            var mailboxSettings = _engineFactory.MailBoxSettingEngine.GetMailBoxSettings(DOMAIN);

//            var testMailboxes = mailboxSettings.ToMailboxList(TestUser.Email, PASSWORD, CURRENT_TENANT, TestUser.ID.ToString());

//            _mbox = testMailboxes.FirstOrDefault();

//            if (!_engineFactory.MailboxEngine.SaveMailBox(_mbox))
//            {
//                throw new Exception(string.Format("Can't create mailbox with email: {0}", TestUser.Email));
//            }

//            SecurityContext.AuthenticateMe(TestUser.ID);
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
//        public void LoadMessagesToUserFolderTest()
//        {
//            var baseFolder = _engineFactory.UserFolderEngine.Create("Folder 1");

//            Assert.Greater(baseFolder.Id, 0);

//            var resultBaseFolder = _engineFactory.UserFolderEngine.Get(baseFolder.Id);

//            Assert.IsNotNull(resultBaseFolder);
//            Assert.AreEqual(0, resultBaseFolder.ParentId);
//            Assert.AreEqual(0, resultBaseFolder.FolderCount);
//            Assert.AreEqual(0, resultBaseFolder.UnreadCount);
//            Assert.AreEqual(0, resultBaseFolder.TotalCount);
//            Assert.AreEqual(0, resultBaseFolder.UnreadChainCount);
//            Assert.AreEqual(0, resultBaseFolder.TotalChainCount);

//            int mailId1;

//            using (var fs = new FileStream(Eml1Path, FileMode.Open, FileAccess.Read))
//            {
//                mailId1 = _engineFactory.TestEngine.LoadSampleMessage((int) FolderType.UserFolder, baseFolder.Id,
//                    _mbox.MailBoxId, true, fs);
//            }

//            Assert.Greater(mailId1, 0);

//            resultBaseFolder = _engineFactory.UserFolderEngine.Get(baseFolder.Id);

//            Assert.IsNotNull(resultBaseFolder);
//            Assert.AreEqual(0, resultBaseFolder.ParentId);
//            Assert.AreEqual(0, resultBaseFolder.FolderCount);
//            Assert.AreEqual(1, resultBaseFolder.UnreadCount);
//            Assert.AreEqual(1, resultBaseFolder.TotalCount);
//            Assert.AreEqual(1, resultBaseFolder.UnreadChainCount);
//            Assert.AreEqual(1, resultBaseFolder.TotalChainCount);

//            int mailId2;

//            using (var fs = new FileStream(Eml2Path, FileMode.Open, FileAccess.Read))
//            {
//                mailId2 = _engineFactory.TestEngine.LoadSampleMessage((int) FolderType.UserFolder, baseFolder.Id,
//                    _mbox.MailBoxId, false, fs);
//            }

//            Assert.Greater(mailId2, 0);

//            resultBaseFolder = _engineFactory.UserFolderEngine.Get(baseFolder.Id);

//            Assert.IsNotNull(resultBaseFolder);
//            Assert.AreEqual(0, resultBaseFolder.ParentId);
//            Assert.AreEqual(0, resultBaseFolder.FolderCount);
//            Assert.AreEqual(1, resultBaseFolder.UnreadCount);
//            Assert.AreEqual(2, resultBaseFolder.TotalCount);
//            Assert.AreEqual(1, resultBaseFolder.UnreadChainCount);
//            Assert.AreEqual(2, resultBaseFolder.TotalChainCount);
//        }

//        [Test]
//        public void MoveMessagesFromDefaulFolderToUserFolderTest()
//        {
//            var baseFolder = _engineFactory.UserFolderEngine.Create("Folder 1");

//            Assert.Greater(baseFolder.Id, 0);

//            var resultBaseFolder = _engineFactory.UserFolderEngine.Get(baseFolder.Id);

//            Assert.IsNotNull(resultBaseFolder);
//            Assert.AreEqual(0, resultBaseFolder.ParentId);
//            Assert.AreEqual(0, resultBaseFolder.FolderCount);
//            Assert.AreEqual(0, resultBaseFolder.UnreadCount);
//            Assert.AreEqual(0, resultBaseFolder.TotalCount);
//            Assert.AreEqual(0, resultBaseFolder.UnreadChainCount);
//            Assert.AreEqual(0, resultBaseFolder.TotalChainCount);

//            int mailId1;
//            int mailId2;

//            using (var fs = new FileStream(Eml1Path, FileMode.Open, FileAccess.Read))
//            {
//                mailId1 = _engineFactory.TestEngine.LoadSampleMessage((int) FolderType.Inbox, null,
//                    _mbox.MailBoxId, true, fs);
//            }

//            using (var fs = new FileStream(Eml2Path, FileMode.Open, FileAccess.Read))
//            {
//                mailId2 = _engineFactory.TestEngine.LoadSampleMessage((int) FolderType.Inbox, null,
//                    _mbox.MailBoxId, false, fs);
//            }

//            Assert.Greater(mailId1, 0);
//            Assert.Greater(mailId2, 0);

//            _engineFactory.MessageEngine.SetFolder(new List<int> {mailId1, mailId2}, FolderType.UserFolder,
//                baseFolder.Id);

//            resultBaseFolder = _engineFactory.UserFolderEngine.Get(baseFolder.Id);

//            Assert.IsNotNull(resultBaseFolder);
//            Assert.AreEqual(0, resultBaseFolder.ParentId);
//            Assert.AreEqual(0, resultBaseFolder.FolderCount);
//            Assert.AreEqual(1, resultBaseFolder.UnreadCount);
//            Assert.AreEqual(2, resultBaseFolder.TotalCount);
//            Assert.AreEqual(1, resultBaseFolder.UnreadChainCount);
//            Assert.AreEqual(2, resultBaseFolder.TotalChainCount);
//        }

//        [Test]
//        public void MoveMessagesFromUserFolderToDefaulFolderTest()
//        {
//            var baseFolder = _engineFactory.UserFolderEngine.Create("Folder 1");

//            Assert.Greater(baseFolder.Id, 0);

//            var resultBaseFolder = _engineFactory.UserFolderEngine.Get(baseFolder.Id);

//            Assert.IsNotNull(resultBaseFolder);
//            Assert.AreEqual(0, resultBaseFolder.ParentId);
//            Assert.AreEqual(0, resultBaseFolder.FolderCount);
//            Assert.AreEqual(0, resultBaseFolder.UnreadCount);
//            Assert.AreEqual(0, resultBaseFolder.TotalCount);
//            Assert.AreEqual(0, resultBaseFolder.UnreadChainCount);
//            Assert.AreEqual(0, resultBaseFolder.TotalChainCount);

//            int mailId1;
//            int mailId2;

//            using (var fs = new FileStream(Eml1Path, FileMode.Open, FileAccess.Read))
//            {
//                mailId1 = _engineFactory.TestEngine.LoadSampleMessage((int) FolderType.UserFolder, baseFolder.Id,
//                    _mbox.MailBoxId, true, fs);
//            }

//            using (var fs = new FileStream(Eml2Path, FileMode.Open, FileAccess.Read))
//            {
//                mailId2 = _engineFactory.TestEngine.LoadSampleMessage((int) FolderType.UserFolder, baseFolder.Id,
//                    _mbox.MailBoxId, false, fs);
//            }

//            Assert.Greater(mailId1, 0);
//            Assert.Greater(mailId2, 0);

//            resultBaseFolder = _engineFactory.UserFolderEngine.Get(baseFolder.Id);

//            Assert.IsNotNull(resultBaseFolder);
//            Assert.AreEqual(0, resultBaseFolder.ParentId);
//            Assert.AreEqual(0, resultBaseFolder.FolderCount);
//            Assert.AreEqual(1, resultBaseFolder.UnreadCount);
//            Assert.AreEqual(2, resultBaseFolder.TotalCount);
//            Assert.AreEqual(1, resultBaseFolder.UnreadChainCount);
//            Assert.AreEqual(2, resultBaseFolder.TotalChainCount);

//            _engineFactory.MessageEngine.SetFolder(new List<int> {mailId1, mailId2}, FolderType.Inbox);

//            resultBaseFolder = _engineFactory.UserFolderEngine.Get(baseFolder.Id);

//            Assert.IsNotNull(resultBaseFolder);
//            Assert.AreEqual(0, resultBaseFolder.ParentId);
//            Assert.AreEqual(0, resultBaseFolder.FolderCount);
//            Assert.AreEqual(0, resultBaseFolder.UnreadCount);
//            Assert.AreEqual(0, resultBaseFolder.TotalCount);
//            Assert.AreEqual(0, resultBaseFolder.UnreadChainCount);
//            Assert.AreEqual(0, resultBaseFolder.TotalChainCount);
//        }

//        [Test]
//        public void MoveMessagesFromUserFolderToAnotherUserFolderTest()
//        {
//            var folder1 = _engineFactory.UserFolderEngine.Create("Folder 1");

//            Assert.Greater(folder1.Id, 0);

//            var resultFolder1 = _engineFactory.UserFolderEngine.Get(folder1.Id);

//            Assert.IsNotNull(resultFolder1);
//            Assert.AreEqual(0, resultFolder1.ParentId);
//            Assert.AreEqual(0, resultFolder1.FolderCount);
//            Assert.AreEqual(0, resultFolder1.UnreadCount);
//            Assert.AreEqual(0, resultFolder1.TotalCount);
//            Assert.AreEqual(0, resultFolder1.UnreadChainCount);
//            Assert.AreEqual(0, resultFolder1.TotalChainCount);

//            var folder2 = _engineFactory.UserFolderEngine.Create("Folder 2");

//            Assert.Greater(folder2.Id, 0);

//            var resultFolder2 = _engineFactory.UserFolderEngine.Get(folder2.Id);

//            Assert.IsNotNull(resultFolder2);
//            Assert.AreEqual(0, resultFolder2.ParentId);
//            Assert.AreEqual(0, resultFolder2.FolderCount);
//            Assert.AreEqual(0, resultFolder2.UnreadCount);
//            Assert.AreEqual(0, resultFolder2.TotalCount);
//            Assert.AreEqual(0, resultFolder2.UnreadChainCount);
//            Assert.AreEqual(0, resultFolder2.TotalChainCount);

//            int mailId1;
//            int mailId2;

//            using (var fs = new FileStream(Eml1Path, FileMode.Open, FileAccess.Read))
//            {
//                mailId1 = _engineFactory.TestEngine.LoadSampleMessage((int)FolderType.UserFolder, folder1.Id,
//                    _mbox.MailBoxId, true, fs);
//            }

//            using (var fs = new FileStream(Eml2Path, FileMode.Open, FileAccess.Read))
//            {
//                mailId2 = _engineFactory.TestEngine.LoadSampleMessage((int)FolderType.UserFolder, folder1.Id,
//                    _mbox.MailBoxId, false, fs);
//            }

//            Assert.Greater(mailId1, 0);
//            Assert.Greater(mailId2, 0);

//            resultFolder1 = _engineFactory.UserFolderEngine.Get(folder1.Id);

//            Assert.IsNotNull(resultFolder1);
//            Assert.AreEqual(0, resultFolder1.ParentId);
//            Assert.AreEqual(0, resultFolder1.FolderCount);
//            Assert.AreEqual(1, resultFolder1.UnreadCount);
//            Assert.AreEqual(2, resultFolder1.TotalCount);
//            Assert.AreEqual(1, resultFolder1.UnreadChainCount);
//            Assert.AreEqual(2, resultFolder1.TotalChainCount);

//            _engineFactory.MessageEngine.SetFolder(new List<int> { mailId1, mailId2 }, FolderType.UserFolder, folder2.Id);

//            resultFolder1 = _engineFactory.UserFolderEngine.Get(folder1.Id);

//            Assert.IsNotNull(resultFolder1);
//            Assert.AreEqual(0, resultFolder1.ParentId);
//            Assert.AreEqual(0, resultFolder1.FolderCount);
//            Assert.AreEqual(0, resultFolder1.UnreadCount);
//            Assert.AreEqual(0, resultFolder1.TotalCount);
//            Assert.AreEqual(0, resultFolder1.UnreadChainCount);
//            Assert.AreEqual(0, resultFolder1.TotalChainCount);

//            resultFolder2 = _engineFactory.UserFolderEngine.Get(folder2.Id);

//            Assert.IsNotNull(resultFolder2);
//            Assert.AreEqual(0, resultFolder2.ParentId);
//            Assert.AreEqual(0, resultFolder2.FolderCount);
//            Assert.AreEqual(1, resultFolder2.UnreadCount);
//            Assert.AreEqual(2, resultFolder2.TotalCount);
//            Assert.AreEqual(1, resultFolder2.UnreadChainCount);
//            Assert.AreEqual(2, resultFolder2.TotalChainCount);
//        }
//    }
//}
