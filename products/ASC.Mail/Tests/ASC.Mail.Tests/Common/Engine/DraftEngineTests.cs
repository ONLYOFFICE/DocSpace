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
//using System.Text;
//using ASC.Core;
//using ASC.Core.Users;
//using ASC.Mail.Aggregator.Tests.Common.Utils;
//using ASC.Mail.Core;
//using ASC.Mail.Models
//using ASC.Mail.Enums;
//using ASC.Mail.Exceptions;
//using ASC.Mail.Utils;
//using NUnit.Framework;

//namespace ASC.Mail.Aggregator.Tests.Common.Engine
//{
//    [TestFixture]
//    internal class DraftEngineTests
//    {
//        private const int CURRENT_TENANT = 0;
//        public const string PASSWORD = "123456";
//        public const string DOMAIN = "gmail.com";

//        public UserInfo TestUser { get; private set; }
//        private EngineFactory _engineFactory;
//        private MailBoxData _testMailbox;

//        private static readonly string TestFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
//           @"..\..\Data\");
//        private const string EML1_FILE_NAME = @"bad_encoding.eml";
//        private static readonly string Eml1Path = TestFolderPath + EML1_FILE_NAME;

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
//        public void CreateDraftTest()
//        {
//            var folders = _engineFactory.FolderEngine.GetFolders();

//            Assert.IsNotEmpty(folders);

//            Assert.AreEqual(true,
//                folders.Any(f => f.total == 0 && f.unread == 0 && f.totalMessages == 0 && f.unreadMessages == 0));

//            var draftItem = new MailDraftData(0, _testMailbox, "test@gmail.com", new List<string>(), new List<string>(), new List<string>(), "subject", MailUtil.CreateMessageId(), null, false, null, "Test body", MailUtil.CreateStreamId(), new List<MailAttachmentData>());

//            var data = _engineFactory.DraftEngine.Save(draftItem);

//            Assert.Greater(data.Id, 0);

//            folders = _engineFactory.FolderEngine.GetFolders();

//            var draft = folders.FirstOrDefault(f => f.id == FolderType.Draft);

//            Assert.IsNotNull(draft);
//            Assert.AreEqual(1, draft.totalMessages);
//            Assert.AreEqual(0, draft.unreadMessages);
//            Assert.AreEqual(0, draft.total);
//            Assert.AreEqual(0, draft.unread);

//            var savedDraftData = _engineFactory.MessageEngine.GetMessage(data.Id, new MailMessageData.Options());

//            Assert.AreEqual("subject", savedDraftData.Subject);
//            Assert.AreEqual("test@gmail.com", savedDraftData.From);
//        }

//        [Test]
//        public void CreateForwardDraftTest()
//        {
//            var folders = _engineFactory.FolderEngine.GetFolders();

//            Assert.IsNotEmpty(folders);

//            Assert.AreEqual(true,
//                folders.Any(f => f.total == 0 && f.unread == 0 && f.totalMessages == 0 && f.unreadMessages == 0));

//            var id1 = _engineFactory.TestEngine.CreateSampleMessage((int?)FolderType.Inbox, _testMailbox.MailBoxId,
//                new List<string> { _testMailbox.EMailView }, new List<string>(), new List<string>(), false, true, "Test subject",
//                "Test body");

//            Assert.Greater(id1, 0);

//            MailAttachmentData attachData;

//            using (var fs = new FileStream(Eml1Path, FileMode.Open, FileAccess.Read))
//            {
//                attachData = _engineFactory.TestEngine.AppendAttachmentsToSampleMessage(id1, EML1_FILE_NAME, fs, "message/eml");
//            }

//            Assert.IsNotNull(attachData);
//            Assert.Greater(attachData.fileId, 0);

//            var message = _engineFactory.MessageEngine.GetMessage(id1, new MailMessageData.Options());

//            Assert.AreEqual(1, message.Attachments.Count);

//            var draftItem = new MailDraftData(0, _testMailbox, "test@gmail.com", new List<string>(), new List<string>(), new List<string>(), "subject", MailUtil.CreateMessageId(), null, false, null, "Test body", MailUtil.CreateStreamId(), message.Attachments);

//            var data = _engineFactory.DraftEngine.Save(draftItem);

//            Assert.Greater(data.Id, 0);

//            folders = _engineFactory.FolderEngine.GetFolders();

//            var inbox = folders.FirstOrDefault(f => f.id == FolderType.Inbox);

//            Assert.IsNotNull(inbox);
//            Assert.AreEqual(1, inbox.totalMessages);
//            Assert.AreEqual(1, inbox.unreadMessages);
//            Assert.AreEqual(1, inbox.total);
//            Assert.AreEqual(1, inbox.unread);

//            var draft = folders.FirstOrDefault(f => f.id == FolderType.Draft);

//            Assert.IsNotNull(draft);
//            Assert.AreEqual(1, draft.totalMessages);
//            Assert.AreEqual(0, draft.unreadMessages);
//            Assert.AreEqual(0, draft.total);
//            Assert.AreEqual(0, draft.unread);

//            var savedDraftData = _engineFactory.MessageEngine.GetMessage(data.Id, new MailMessageData.Options());

//            Assert.AreEqual("subject", savedDraftData.Subject);
//            Assert.AreEqual("test@gmail.com", savedDraftData.From);
//            Assert.AreEqual(1, savedDraftData.Attachments.Count);

//            message = _engineFactory.MessageEngine.GetMessage(id1, new MailMessageData.Options());

//            Assert.AreEqual(1, message.Attachments.Count);
//        }

//        [Test]
//        public void CreateDraftWithClonedAttachmentTest()
//        {
//            var folders = _engineFactory.FolderEngine.GetFolders();

//            Assert.IsNotEmpty(folders);

//            Assert.AreEqual(true,
//                folders.Any(f => f.total == 0 && f.unread == 0 && f.totalMessages == 0 && f.unreadMessages == 0));

//            var id1 = _engineFactory.TestEngine.CreateSampleMessage((int?) FolderType.Inbox, _testMailbox.MailBoxId,
//                new List<string> {_testMailbox.EMailView}, new List<string>(), new List<string>(), false, true,
//                "Test subject",
//                "Test body");

//            Assert.Greater(id1, 0);

//            MailAttachmentData attachData;

//            using (var fs = new FileStream(Eml1Path, FileMode.Open, FileAccess.Read))
//            {
//                attachData = _engineFactory.TestEngine.AppendAttachmentsToSampleMessage(id1, EML1_FILE_NAME, fs,
//                    "message/eml");
//            }

//            Assert.IsNotNull(attachData);
//            Assert.Greater(attachData.fileId, 0);

//            var message = _engineFactory.MessageEngine.GetMessage(id1, new MailMessageData.Options());

//            Assert.AreEqual(1, message.Attachments.Count);

//            var clonedAttachments = new List<MailAttachmentData>();
//            do
//            {
//                var clone = attachData.Clone() as MailAttachmentData;
//                if (clone == null)
//                    break;

//                clonedAttachments.Add(clone);

//            } while (clonedAttachments.Sum(a => a.size) < Defines.ATTACHMENTS_TOTAL_SIZE_LIMIT);

//            Assert.Greater(clonedAttachments.Count, 1);

//            var draftItem = new MailDraftData(0, _testMailbox, "test@gmail.com", new List<string>(), new List<string>(),
//                new List<string>(), "subject", MailUtil.CreateMessageId(), null, false, null, "Test body",
//                MailUtil.CreateStreamId(), clonedAttachments);

//            Assert.AreEqual(1, draftItem.Attachments.Count);

//            var data = _engineFactory.DraftEngine.Save(draftItem);

//            Assert.Greater(data.Id, 0);
//            Assert.AreEqual(1, data.Attachments.Count);

//            var savedDraftData = _engineFactory.MessageEngine.GetMessage(data.Id, new MailMessageData.Options());

//            Assert.AreEqual("subject", savedDraftData.Subject);
//            Assert.AreEqual("test@gmail.com", savedDraftData.From);
//            Assert.AreEqual(1, savedDraftData.Attachments.Count);

//            message = _engineFactory.MessageEngine.GetMessage(id1, new MailMessageData.Options());

//            Assert.AreEqual(1, message.Attachments.Count);
//        }

//        [Test]
//        public void CreateDraftWithAttachmentsTotalExceededTest()
//        {
//            var attachments = new List<MailAttachmentData>();

//            var index = 0;

//            do
//            {
//                ++index;

//                var attachData = new MailAttachmentData()
//                {
//                    fileId = index,
//                    fileNumber = index,
//                    tenant = CURRENT_TENANT,
//                    user = TestUser.ID.ToString(),
//                    mailboxId = _testMailbox.MailBoxId,
//                    data = Encoding.UTF8.GetBytes("Test"),
//                    size = 100000,
//                    contentId = "Content" + index,
//                    streamId = MailUtil.CreateStreamId(),
//                    storedName = MailUtil.CreateStreamId() + ".txt",
//                    fileName = "Test_DATA.txt"
//                };

//                attachments.Add(attachData);

//            } while (attachments.Sum(a => a.size) < Defines.ATTACHMENTS_TOTAL_SIZE_LIMIT);

//            Assert.Throws<DraftException>(
//                () => new MailDraftData(0, _testMailbox, "test@gmail.com", new List<string>(), new List<string>(),
//                    new List<string>(), "subject", MailUtil.CreateMessageId(), null, false, null, "Test body",
//                    MailUtil.CreateStreamId(), attachments), "Total size of all files exceeds limit!");
//        }

//        [Test]
//        public void CreateDraftWithAttachAndOpenIt()
//        {
//            var folders = _engineFactory.FolderEngine.GetFolders();

//            Assert.IsNotEmpty(folders);

//            var id1 = _engineFactory.TestEngine.CreateSampleMessage((int?)FolderType.Inbox, _testMailbox.MailBoxId,
//                new List<string> { _testMailbox.EMailView }, new List<string>(), new List<string>(), false, true, "Test subject",
//                "Test body");

//            Assert.Greater(id1, 0);

//            MailAttachmentData attachData;

//            using (var fs = new FileStream(Eml1Path, FileMode.Open, FileAccess.Read))
//            {
//                attachData = _engineFactory.TestEngine.AppendAttachmentsToSampleMessage(id1, EML1_FILE_NAME, fs, "message/eml");
//            }

//            Assert.IsNotNull(attachData);
//            Assert.Greater(attachData.fileId, 0);

//            var message = _engineFactory.MessageEngine.GetMessage(id1, new MailMessageData.Options());

//            Assert.AreEqual(1, message.Attachments.Count);

//            var draftItem = new MailDraftData(0, _testMailbox, "test@gmail.com", new List<string>(), new List<string>(), new List<string>(), "subject", MailUtil.CreateMessageId(), null, false, null, "Test body", MailUtil.CreateStreamId(), message.Attachments);

//            var data = _engineFactory.DraftEngine.Save(draftItem);

//            Assert.AreEqual(1,data.Attachments.Count);

//            data = _engineFactory.DraftEngine.Save(draftItem);

//            Assert.AreEqual(1, data.Attachments.Count);

//            data = _engineFactory.DraftEngine.Save(draftItem);

//            Assert.AreEqual(1, data.Attachments.Count);
//        }
//    }
//}
