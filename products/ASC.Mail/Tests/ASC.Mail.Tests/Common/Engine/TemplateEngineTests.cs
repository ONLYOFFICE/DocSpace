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
//using System.Linq;
//using ASC.Core;
//using ASC.Core.Users;
//using ASC.Mail.Core;
//using ASC.Mail.Models;
//using ASC.Mail.Enums;
//using ASC.Mail.Utils;
//using NUnit.Framework;
//using System.IO;
//using ASC.Mail.Aggregator.Tests.Common.Utils;

//namespace ASC.Mail.Aggregator.Tests.Common.Engine
//{
//    [TestFixture]
//    internal class TemplateEngineTests
//    {
//        private const int CURRENT_TENANT = 0;
//        public const string PASSWORD = "123456";
//        public const string DOMAIN = "gmail.com";

//        private static readonly string TestFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
//           @"..\..\Data\");
//        private const string EML1_FILE_NAME = @"bad_encoding.eml";
//        private static readonly string Eml1Path = TestFolderPath + EML1_FILE_NAME;

//        public UserInfo TestUser { get; private set; }
//        private EngineFactory _engineFactory;
//        private MailBoxData _testMailbox;

//        [SetUp]
//        public void SetUp()
//        {
//            CoreContext.TenantManager.SetCurrentTenant(CURRENT_TENANT);
//            SecurityContext.AuthenticateMe(ASC.Core.Configuration.Constants.CoreSystem);

//            TestUser = TestHelper.CreateNewRandomEmployee();

//            _engineFactory = new EngineFactory(CURRENT_TENANT, TestUser.ID.ToString());

//            var mailboxSettings = _engineFactory.MailBoxSettingEngine.GetMailBoxSettings(DOMAIN);

//            var testMailboxes = mailboxSettings.ToMailboxList(TestUser.Email, PASSWORD, CURRENT_TENANT, TestUser.ID.ToString());

//            _testMailbox = testMailboxes.FirstOrDefault();

//            if (_testMailbox == null || !_engineFactory.MailboxEngine.SaveMailBox(_testMailbox))
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
//        public void CreateTemplate()
//        {
//            var folders = _engineFactory.FolderEngine.GetFolders();

//            Assert.IsNotEmpty(folders);

//            var templateItem = new MailTemplateData(0, _testMailbox, "test@gmail.com", new List<string>(), 
//                new List<string>(), new List<string>(), "subject", MailUtil.CreateMessageId(), null, false, 
//                null, "Test body", MailUtil.CreateStreamId(), new List<MailAttachmentData>());

//            var data = _engineFactory.TemplateEngine.Save(templateItem);

//            Assert.Greater(data.Id, 0);

//            folders = _engineFactory.FolderEngine.GetFolders();

//            var templateFolder = folders.FirstOrDefault(f => f.id == FolderType.Templates);

//            Assert.IsNotNull(templateFolder);
//            Assert.AreEqual(1, templateFolder.totalMessages);
//            Assert.AreEqual(0, templateFolder.unreadMessages);
//            Assert.AreEqual(0, templateFolder.total);
//            Assert.AreEqual(0, templateFolder.unread);

//            var savedTemplateData = _engineFactory.MessageEngine.GetMessage(data.Id, new MailMessageData.Options());

//            Assert.AreEqual("subject", savedTemplateData.Subject);
//            Assert.AreEqual("test@gmail.com", savedTemplateData.From);
//        }
//    }
//}
