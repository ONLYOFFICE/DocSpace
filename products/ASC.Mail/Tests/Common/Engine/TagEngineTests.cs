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
//using ASC.Mail.Core.Entities;
//using ASC.Mail.Models;
//using ASC.Mail.Enums;
//using ASC.Mail.Utils;
//using NUnit.Framework;

//namespace ASC.Mail.Aggregator.Tests.Common.Engine
//{
//    [TestFixture]
//    internal class TagEngineTests
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
//        private int _mailId;

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

//            using (var fs = new FileStream(Eml1Path, FileMode.Open, FileAccess.Read))
//            {
//                _mailId = _engineFactory.TestEngine.LoadSampleMessage((int)FolderType.Inbox, null, _testMailbox.MailBoxId, true, fs);
//            }
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

//        private List<Tag> CreateTagsOnMessage()
//        {
//            var tag1 = _engineFactory.TagEngine.CreateTag("Tag1", "11", new List<string>());

//            Assert.IsNotNull(tag1);
//            Assert.Greater(tag1.Id, 0);

//            _engineFactory.TagEngine.SetMessagesTag(new List<int> { _mailId }, tag1.Id);

//            var tag2 = _engineFactory.TagEngine.CreateTag("Tag2", "10", new List<string>());

//            Assert.IsNotNull(tag1);
//            Assert.Greater(tag1.Id, 0);

//            _engineFactory.TagEngine.SetMessagesTag(new List<int> { _mailId }, tag2.Id);

//            var tags = _engineFactory.TagEngine.GetTags();

//            Assert.IsNotEmpty(tags);
//            Assert.AreEqual(2, tags.Count);
//            Assert.Contains(tag1.Id, tags.Select(m => m.Id).ToArray());
//            Assert.Contains(tag2.Id, tags.Select(m => m.Id).ToArray());

//            var message = _engineFactory.MessageEngine.GetMessage(_mailId, new MailMessageData.Options());

//            Assert.IsNotEmpty(message.TagIds);
//            Assert.AreEqual(2, message.TagIds.Count);
//            Assert.Contains(tag1.Id, message.TagIds);
//            Assert.Contains(tag2.Id, message.TagIds);

//            return tags;
//        }

//        private List<Tag> CreateTagsOnConversation()
//        {
//            var tag1 = _engineFactory.TagEngine.CreateTag("Tag1", "11", new List<string>());

//            Assert.IsNotNull(tag1);
//            Assert.Greater(tag1.Id, 0);

//            _engineFactory.TagEngine.SetConversationsTag(new List<int> { _mailId }, tag1.Id);

//            var tag2 = _engineFactory.TagEngine.CreateTag("Tag2", "10", new List<string>());

//            Assert.IsNotNull(tag1);
//            Assert.Greater(tag1.Id, 0);

//            _engineFactory.TagEngine.SetConversationsTag(new List<int> { _mailId }, tag2.Id);

//            var tags = _engineFactory.TagEngine.GetTags();

//            Assert.IsNotEmpty(tags);
//            Assert.AreEqual(2, tags.Count);
//            Assert.Contains(tag1.Id, tags.Select(m => m.Id).ToArray());
//            Assert.Contains(tag2.Id, tags.Select(m => m.Id).ToArray());

//            var message = _engineFactory.MessageEngine.GetMessage(_mailId, new MailMessageData.Options());

//            Assert.IsNotEmpty(message.TagIds);
//            Assert.AreEqual(2, message.TagIds.Count);
//            Assert.Contains(tag1.Id, message.TagIds);
//            Assert.Contains(tag2.Id, message.TagIds);

//            return tags;
//        }

//        [Test]
//        public void SetMessageNewTagsTest()
//        {
//            CreateTagsOnMessage();
//        }

//        [Test()]
//        public void SetConversationNewTagsTest()
//        {
//            CreateTagsOnConversation();
//        }

//        [Test]
//        public void UnsetMessageFirstTagTest()
//        {
//            var tags = CreateTagsOnMessage();

//            var tag1 = tags[0];
//            var tag2 = tags[1];

//            _engineFactory.TagEngine.UnsetMessagesTag(new List<int> { _mailId }, tag1.Id);

//            var message = _engineFactory.MessageEngine.GetMessage(_mailId, new MailMessageData.Options());

//            Assert.IsNotEmpty(message.TagIds);
//            Assert.AreEqual(1, message.TagIds.Count);

//            Assert.Contains(tag2.Id, message.TagIds);
//        }

//        [Test]
//        public void UnsetConversationFirstTagTest()
//        {
//            var tags = CreateTagsOnConversation();

//            var tag1 = tags[0];
//            var tag2 = tags[1];

//            _engineFactory.TagEngine.UnsetConversationsTag(new List<int> { _mailId }, tag1.Id);

//            var message = _engineFactory.MessageEngine.GetMessage(_mailId, new MailMessageData.Options());

//            Assert.IsNotEmpty(message.TagIds);
//            Assert.AreEqual(1, message.TagIds.Count);

//            Assert.Contains(tag2.Id, message.TagIds);
//        }

//        [Test]
//        public void UnsetMessageSecondTagTest()
//        {
//            var tags = CreateTagsOnMessage();

//            var tag1 = tags[0];
//            var tag2 = tags[1];

//            _engineFactory.TagEngine.UnsetMessagesTag(new List<int> { _mailId }, tag2.Id);

//            var message = _engineFactory.MessageEngine.GetMessage(_mailId, new MailMessageData.Options());

//            Assert.IsNotEmpty(message.TagIds);
//            Assert.AreEqual(1, message.TagIds.Count);

//            Assert.Contains(tag1.Id, message.TagIds);
//        }

//        [Test]
//        public void UnsetConversationSecondTagTest()
//        {
//            var tags = CreateTagsOnConversation();

//            var tag1 = tags[0];
//            var tag2 = tags[1];

//            _engineFactory.TagEngine.UnsetConversationsTag(new List<int> { _mailId }, tag2.Id);

//            var message = _engineFactory.MessageEngine.GetMessage(_mailId, new MailMessageData.Options());

//            Assert.IsNotEmpty(message.TagIds);
//            Assert.AreEqual(1, message.TagIds.Count);

//            Assert.Contains(tag1.Id, message.TagIds);
//        }
//    }
//}
