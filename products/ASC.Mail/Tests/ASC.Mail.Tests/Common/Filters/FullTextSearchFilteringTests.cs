/*
 *
 * (c) Copyright Ascensio System Limited 2010-2020
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
using ASC.Core;
using ASC.Core.Users;
using ASC.ElasticSearch;
using ASC.Mail.Aggregator.Tests.Common.Utils;
using ASC.Mail.Aggregator.Tests.Utils;
using ASC.Mail.Core;
using ASC.Mail.Data.Contracts;
using ASC.Mail.Data.Search;
using ASC.Mail.Enums;
using ASC.Mail.Enums.Filter;
using ASC.Mail.Utils;
using NUnit.Framework;

namespace ASC.Mail.Aggregator.Tests.Common.Filters
{
    [TestFixture]
    internal class FullTextSearchFilteringTests
    {
        private const int CURRENT_TENANT = 0;
        public const string PASSWORD = "123456";
        public const string DOMAIN = "gmail.com";
        public const string EMAIL_NAME = "Test User";

        public UserInfo TestUser { get; set; }
        public MailBoxData Mbox { get; private set; }
        public EngineFactory Factory { get; private set; }

        private const int PAGE = 0;
        private const int LIMIT = 10;

        [SetUp]
        public void SetUp()
        {
            CoreContext.TenantManager.SetCurrentTenant(CURRENT_TENANT);
            SecurityContext.AuthenticateMe(ASC.Core.Configuration.Constants.CoreSystem);

            TestUser = TestHelper.CreateNewRandomEmployee();

            Factory = new EngineFactory(CURRENT_TENANT, TestUser.ID.ToString());

            var mailboxSettings = Factory.MailBoxSettingEngine.GetMailBoxSettings(DOMAIN);

            var testMailboxes = mailboxSettings.ToMailboxList(TestUser.Email, PASSWORD, CURRENT_TENANT, TestUser.ID.ToString());

            Mbox = testMailboxes.FirstOrDefault();

            if (Mbox != null)
                Mbox.Name = EMAIL_NAME;

            if (!Factory.MailboxEngine.SaveMailBox(Mbox))
            {
                throw new Exception(string.Format("Can't create mailbox with email: {0}", TestUser.Email));
            }
        }

        [TearDown]
        public void CleanUp()
        {
            if (TestUser == null || TestUser.ID == Guid.Empty)
                return;

            CoreContext.TenantManager.SetCurrentTenant(CURRENT_TENANT);

            SecurityContext.AuthenticateMe(ASC.Core.Configuration.Constants.CoreSystem);

            CoreContext.UserManager.DeleteUser(TestUser.ID);

            FactoryIndexer<MailWrapper>.DeleteAsync(s => s.Where(m => m.UserId, TestUser.ID)).Wait();

            // Clear TestUser1 mail data
            var eraser = Factory.MailGarbageEngine;

            eraser.ClearUserMail(TestUser.ID, CoreContext.TenantManager.GetCurrentTenant());
        }

        [Test, IgnoreIfFullTextSearch(enabled: false)]
        public void CheckFtSearchSubjectMatch()
        {
            const string subj = "[TEST SUBJECT]";

            var id = Factory.TestEngine.CreateSampleMessage((int)FolderType.Inbox, Mbox.MailBoxId,
                new List<string> { "to@to.com" }, new List<string> { "cc@cc.com" }, new List<string>(), false, true,
                subj, "This is SPARTA");

            var message = Factory.MessageEngine.GetMessage(id, new MailMessageData.Options());

            var mailWrapper = message.ToMailWrapper(CURRENT_TENANT, TestUser.ID);

            FactoryIndexer<MailWrapper>.Index(mailWrapper);

            Assert.Greater(id, 0);

            var filter = new MailSieveFilterData
            {
                Conditions = new List<MailSieveFilterConditionData>
                {
                    new MailSieveFilterConditionData
                    {
                        Key = ConditionKeyType.Subject,
                        Operation = ConditionOperationType.Matches,
                        Value = subj
                    }
                },
                Options = new MailSieveFilterOptionsData
                {
                    MatchMultiConditions = MatchMultiConditionsType.MatchAtLeastOne
                }
            };

            long total;
            var messages = Factory.MessageEngine.GetFilteredMessages(filter, PAGE, LIMIT, out total);

            Assert.AreEqual(1, total);
            Assert.AreEqual(1, messages.Count);
            Assert.AreEqual(subj, messages[0].Subject);
        }

        [Test, IgnoreIfFullTextSearch(enabled: false)]
        public void CheckFtSearchSubjectContains()
        {
            var rndGuid = Guid.NewGuid().ToString("N");
            var subj = string.Format("[TEST SUBJECT] {0} zzz", rndGuid);

            var id = Factory.TestEngine.CreateSampleMessage((int)FolderType.Inbox, Mbox.MailBoxId,
                new List<string> { "to@to.com" }, new List<string> { "cc@cc.com" }, new List<string>(), false, true,
                subj, "This is SPARTA");

            var message = Factory.MessageEngine.GetMessage(id, new MailMessageData.Options());

            var mailWrapper = message.ToMailWrapper(CURRENT_TENANT, TestUser.ID);

            FactoryIndexer<MailWrapper>.Index(mailWrapper);

            Assert.Greater(id, 0);

            var filter = new MailSieveFilterData
            {
                Conditions = new List<MailSieveFilterConditionData>
                {
                    new MailSieveFilterConditionData
                    {
                        Key = ConditionKeyType.Subject,
                        Operation = ConditionOperationType.Contains,
                        Value = rndGuid
                    }
                },
                Options = new MailSieveFilterOptionsData
                {
                    MatchMultiConditions = MatchMultiConditionsType.MatchAtLeastOne
                }
            };

            long total;
            var messages = Factory.MessageEngine.GetFilteredMessages(filter, PAGE, LIMIT, out total);

            Assert.AreEqual(1, total);
            Assert.AreEqual(1, messages.Count);
            Assert.AreEqual(subj, messages[0].Subject);
        }

        [Test, IgnoreIfFullTextSearch(enabled: false)]
        public void CheckFtSearchFromMatch()
        {
            var id = Factory.TestEngine.CreateSampleMessage((int)FolderType.Inbox, Mbox.MailBoxId,
                new List<string> { "to@to.com" }, new List<string> { "cc@cc.com" }, new List<string>(), false, true,
                "[TEST SUBJECT]", "This is SPARTA");

            var message = Factory.MessageEngine.GetMessage(id, new MailMessageData.Options());

            var mailWrapper = message.ToMailWrapper(CURRENT_TENANT, TestUser.ID);

            FactoryIndexer<MailWrapper>.Index(mailWrapper);

            Assert.Greater(id, 0);

            var filter = new MailSieveFilterData
            {
                Conditions = new List<MailSieveFilterConditionData>
                {
                    new MailSieveFilterConditionData
                    {
                        Key = ConditionKeyType.From,
                        Operation = ConditionOperationType.Matches,
                        Value = Mbox.EMail.Address
                    }
                },
                Options = new MailSieveFilterOptionsData
                {
                    MatchMultiConditions = MatchMultiConditionsType.MatchAtLeastOne
                }
            };

            long total;
            var messages = Factory.MessageEngine.GetFilteredMessages(filter, PAGE, LIMIT, out total);

            Assert.AreEqual(1, total);
            Assert.AreEqual(1, messages.Count);
            Assert.AreEqual(message.From, messages[0].From);
        }

        [Test, IgnoreIfFullTextSearch(enabled: false)]
        public void CheckFtSearchFromContains()
        {
            var id = Factory.TestEngine.CreateSampleMessage((int)FolderType.Inbox, Mbox.MailBoxId,
                new List<string> { "to@to.com" }, new List<string> { "cc@cc.com" }, new List<string>(), false, true,
                "[TEST SUBJECT]", "This is SPARTA");

            var message = Factory.MessageEngine.GetMessage(id, new MailMessageData.Options());

            var mailWrapper = message.ToMailWrapper(CURRENT_TENANT, TestUser.ID);

            FactoryIndexer<MailWrapper>.Index(mailWrapper);

            Assert.Greater(id, 0);

            var filter = new MailSieveFilterData
            {
                Conditions = new List<MailSieveFilterConditionData>
                {
                    new MailSieveFilterConditionData
                    {
                        Key = ConditionKeyType.From,
                        Operation = ConditionOperationType.Contains,
                        Value = EMAIL_NAME
                    }
                },
                Options = new MailSieveFilterOptionsData
                {
                    MatchMultiConditions = MatchMultiConditionsType.MatchAtLeastOne
                }
            };

            long total;
            var messages = Factory.MessageEngine.GetFilteredMessages(filter, PAGE, LIMIT, out total);

            Assert.AreEqual(1, total);
            Assert.AreEqual(1, messages.Count);
            Assert.AreEqual(message.From, messages[0].From);
        }

        [Test, IgnoreIfFullTextSearch(enabled: false)]
        public void CheckFtSearchToOrCcMatch()
        {
            var id = Factory.TestEngine.CreateSampleMessage((int)FolderType.Inbox, Mbox.MailBoxId,
                new List<string> { "to@to.com" }, new List<string> { "cc@cc.com" }, new List<string>(), false, true,
                "[TEST SUBJECT]", "This is SPARTA");

            var message = Factory.MessageEngine.GetMessage(id, new MailMessageData.Options());

            var mailWrapper = message.ToMailWrapper(CURRENT_TENANT, TestUser.ID);

            FactoryIndexer<MailWrapper>.Index(mailWrapper);

            Assert.Greater(id, 0);

            var filter = new MailSieveFilterData
            {
                Conditions = new List<MailSieveFilterConditionData>
                {
                    new MailSieveFilterConditionData
                    {
                        Key = ConditionKeyType.ToOrCc,
                        Operation = ConditionOperationType.Matches,
                        Value = "cc@cc.com"
                    }
                }
            };

            long total;
            var messages = Factory.MessageEngine.GetFilteredMessages(filter, PAGE, LIMIT, out total);

            Assert.AreEqual(1, total);
            Assert.AreEqual(1, messages.Count);
            Assert.AreEqual("to@to.com", messages[0].To);
            Assert.AreEqual("cc@cc.com", messages[0].Cc);

            filter = new MailSieveFilterData
            {
                Conditions = new List<MailSieveFilterConditionData>
                {
                    new MailSieveFilterConditionData
                    {
                        Key = ConditionKeyType.ToOrCc,
                        Operation = ConditionOperationType.Matches,
                        Value = "to@to.com"
                    }
                }
            };

            messages = Factory.MessageEngine.GetFilteredMessages(filter, PAGE, LIMIT, out total);

            Assert.AreEqual(1, total);
            Assert.AreEqual(1, messages.Count);
            Assert.AreEqual("to@to.com", messages[0].To);
            Assert.AreEqual("cc@cc.com", messages[0].Cc);
        }

        [Test, IgnoreIfFullTextSearch(enabled: false)]
        public void CheckFtSearchToOrCcContains()
        {
            var id = Factory.TestEngine.CreateSampleMessage((int)FolderType.Inbox, Mbox.MailBoxId,
                new List<string> { "\"to to\" <to@to.com>" }, new List<string> { "\"cc cc\" <cc@cc.com>" }, new List<string>(), false, true,
                "[TEST SUBJECT]", "This is SPARTA");

            var message = Factory.MessageEngine.GetMessage(id, new MailMessageData.Options());

            var mailWrapper = message.ToMailWrapper(CURRENT_TENANT, TestUser.ID);

            FactoryIndexer<MailWrapper>.Index(mailWrapper);

            Assert.Greater(id, 0);

            var filter = new MailSieveFilterData
            {
                Conditions = new List<MailSieveFilterConditionData>
                {
                    new MailSieveFilterConditionData
                    {
                        Key = ConditionKeyType.ToOrCc,
                        Operation = ConditionOperationType.Contains,
                        Value = "cc@cc.com"
                    }
                }
            };

            long total;
            var messages = Factory.MessageEngine.GetFilteredMessages(filter, PAGE, LIMIT, out total);

            Assert.AreEqual(1, total);
            Assert.AreEqual(1, messages.Count);
            Assert.AreEqual("\"to to\" <to@to.com>", messages[0].To);
            Assert.AreEqual("\"cc cc\" <cc@cc.com>", messages[0].Cc);

            filter = new MailSieveFilterData
            {
                Conditions = new List<MailSieveFilterConditionData>
                {
                    new MailSieveFilterConditionData
                    {
                        Key = ConditionKeyType.ToOrCc,
                        Operation = ConditionOperationType.Contains,
                        Value = "to@to.com"
                    }
                }
            };

            messages = Factory.MessageEngine.GetFilteredMessages(filter, PAGE, LIMIT, out total);

            Assert.AreEqual(1, total);
            Assert.AreEqual(1, messages.Count);
            Assert.AreEqual("\"to to\" <to@to.com>", messages[0].To);
            Assert.AreEqual("\"cc cc\" <cc@cc.com>", messages[0].Cc);
        }

        [Test, IgnoreIfFullTextSearch(enabled: false)]
        public void CheckFtSearchByFromEmailOnly()
        {
            var id = Factory.TestEngine.CreateSampleMessage((int)FolderType.Inbox, Mbox.MailBoxId,
                new List<string> { "\"mono\" <mono.mail.4test@gmail.com>" }, new List<string>(), new List<string>(), false, true,
                "[TEST SUBJECT]", "This is SPARTA", fromAddress: "\"Twitter\" <info@twitter.com>");

            var message = Factory.MessageEngine.GetMessage(id, new MailMessageData.Options());

            var mailWrapper = message.ToMailWrapper(CURRENT_TENANT, TestUser.ID);

            FactoryIndexer<MailWrapper>.Index(mailWrapper);

            Assert.Greater(id, 0);

            var filter = new MailSearchFilterData // With page
            {
                PrimaryFolder = FolderType.Inbox,
                FromAddress = "info@twitter.com",
                Page = 0,
                PageSize = 10
            };

            long total;
            var messages = Factory.MessageEngine.GetFilteredMessages(filter, out total);

            Assert.AreEqual(1, total);
            Assert.AreEqual(1, messages.Count);
            Assert.AreEqual(message.From, messages[0].From);
        }

        [Test, IgnoreIfFullTextSearch(enabled: false)]
        public void CheckFtSearchFromAndSubjectMatchAll()
        {
            var subject = "[TEST SUBJECT]";

            var id = Factory.TestEngine.CreateSampleMessage((int)FolderType.Inbox, Mbox.MailBoxId,
                new List<string> { "to@to.com" }, new List<string> { "cc@cc.com" }, new List<string>(), false, true,
                subject, "This is SPARTA");

            var message = Factory.MessageEngine.GetMessage(id, new MailMessageData.Options());

            var mailWrapper = message.ToMailWrapper(CURRENT_TENANT, TestUser.ID);

            FactoryIndexer<MailWrapper>.Index(mailWrapper);

            Assert.Greater(id, 0);

            var filter = new MailSieveFilterData
            {
                Conditions = new List<MailSieveFilterConditionData>
                {
                    new MailSieveFilterConditionData
                    {
                        Key = ConditionKeyType.From,
                        Operation = ConditionOperationType.Matches,
                        Value = Mbox.EMail.Address
                    },
                    new MailSieveFilterConditionData
                    {
                        Key = ConditionKeyType.Subject,
                        Operation = ConditionOperationType.Matches,
                        Value = subject
                    }
                },
                Options = new MailSieveFilterOptionsData
                {
                    MatchMultiConditions = MatchMultiConditionsType.MatchAll
                }
            };

            long total;
            var messages = Factory.MessageEngine.GetFilteredMessages(filter, PAGE, LIMIT, out total);

            Assert.AreEqual(1, total);
            Assert.AreEqual(1, messages.Count);
            Assert.AreEqual(message.From, messages[0].From);
            Assert.AreEqual(message.Subject, messages[0].Subject);
        }

        [Test, IgnoreIfFullTextSearch(enabled: false)]
        public void CheckFtSearchFromAndSubjectMatchAtLeastOne()
        {
            var subject = "[TEST SUBJECT]";

            var id = Factory.TestEngine.CreateSampleMessage((int)FolderType.Inbox, Mbox.MailBoxId,
                new List<string> { "to@to.com" }, new List<string> { "cc@cc.com" }, new List<string>(), false, true,
                subject, "This is SPARTA");

            var message = Factory.MessageEngine.GetMessage(id, new MailMessageData.Options());

            var mailWrapper = message.ToMailWrapper(CURRENT_TENANT, TestUser.ID);

            FactoryIndexer<MailWrapper>.Index(mailWrapper);

            Assert.Greater(id, 0);

            var filter = new MailSieveFilterData
            {
                Conditions = new List<MailSieveFilterConditionData>
                {
                    new MailSieveFilterConditionData
                    {
                        Key = ConditionKeyType.From,
                        Operation = ConditionOperationType.Matches,
                        Value = Mbox.EMail.Address
                    },
                    new MailSieveFilterConditionData
                    {
                        Key = ConditionKeyType.Subject,
                        Operation = ConditionOperationType.Matches,
                        Value = subject + "1"
                    }
                },
                Options = new MailSieveFilterOptionsData
                {
                    MatchMultiConditions = MatchMultiConditionsType.MatchAtLeastOne
                }
            };

            long total;
            var messages = Factory.MessageEngine.GetFilteredMessages(filter, PAGE, LIMIT, out total);

            Assert.AreEqual(1, total);
            Assert.AreEqual(1, messages.Count);
            Assert.AreEqual(message.From, messages[0].From);
            Assert.AreEqual(message.Subject, messages[0].Subject);
        }

        [Test, IgnoreIfFullTextSearch(enabled: false)]
        public void CheckFtSearchToWithDiffMatchAll()
        {
            var to = "to@to.com";

            var id = Factory.TestEngine.CreateSampleMessage((int)FolderType.Inbox, Mbox.MailBoxId,
                new List<string> { to }, new List<string> { "cc@cc.com" }, new List<string>(), false, true,
                "[TEST SUBJECT]", "This is SPARTA");

            Assert.Greater(id, 0);

            var id2 = Factory.TestEngine.CreateSampleMessage((int)FolderType.Inbox, Mbox.MailBoxId,
                new List<string> { "to@todo.com" }, new List<string> { "cc@ccdo.com" }, new List<string>(), false, true,
                "[TEST 2 SUBJECT]", "This is TROY");

            Assert.Greater(id2, 0);

            var message = Factory.MessageEngine.GetMessage(id, new MailMessageData.Options());
            var mailWrapper = message.ToMailWrapper(CURRENT_TENANT, TestUser.ID);

            FactoryIndexer<MailWrapper>.Index(mailWrapper);

            var message2 = Factory.MessageEngine.GetMessage(id2, new MailMessageData.Options());
            var mailWrapper2 = message2.ToMailWrapper(CURRENT_TENANT, TestUser.ID);

            FactoryIndexer<MailWrapper>.Index(mailWrapper2);

            var filter = new MailSieveFilterData
            {
                Conditions = new List<MailSieveFilterConditionData>
                {
                    new MailSieveFilterConditionData
                    {
                        Key = ConditionKeyType.To,
                        Operation = ConditionOperationType.Matches,
                        Value = to
                    }
                }
            };

            long total;
            var messages = Factory.MessageEngine.GetFilteredMessages(filter, PAGE, LIMIT, out total);

            Assert.AreEqual(1, total);
            Assert.AreEqual(1, messages.Count);
            Assert.AreEqual(message.To, messages[0].To);
        }

        [Test, IgnoreIfFullTextSearch(enabled: false)]
        public void CheckFtSearchWithCalendar()
        {
            var id = Factory.TestEngine.CreateSampleMessage((int)FolderType.Inbox, Mbox.MailBoxId,
                new List<string> { "to@to.com" }, new List<string> { "cc@cc.com" }, new List<string>(), false, true,
                "[TEST SUBJECT]", "This is SPARTA");

            Assert.Greater(id, 0);

            var id2 = Factory.TestEngine.CreateSampleMessage((int)FolderType.Inbox, Mbox.MailBoxId,
                new List<string> { "to@todo.com" }, new List<string> { "cc@ccdo.com" }, new List<string>(), false, true,
                "[TEST 2 SUBJECT]", "This is TROY", calendarUid: "5FE291F6-64D0-4751-938F-53033FCD9225");

            Assert.Greater(id2, 0);

            var message = Factory.MessageEngine.GetMessage(id, new MailMessageData.Options());
            var mailWrapper = message.ToMailWrapper(CURRENT_TENANT, TestUser.ID);

            FactoryIndexer<MailWrapper>.Index(mailWrapper);

            var message2 = Factory.MessageEngine.GetMessage(id2, new MailMessageData.Options());
            var mailWrapper2 = message2.ToMailWrapper(CURRENT_TENANT, TestUser.ID);

            FactoryIndexer<MailWrapper>.Index(mailWrapper2);

            var selector = new Selector<MailWrapper>()
                .Where(m => m.WithCalendar, true)
                .Where(r => r.UserId, TestUser.ID)
                .Sort(r => r.DateSent, true);

            List<int> mailIds;

            var success = FactoryIndexer<MailWrapper>.TrySelectIds(m => selector, out mailIds);

            Assert.AreEqual(true, success);
            Assert.AreEqual(1, mailIds.Count);
            Assert.AreEqual(id2, mailIds.First());
        }

        [Test, IgnoreIfFullTextSearch(enabled: false)]
        public void CheckFtSortByDateSend()
        {
            const string sort_order = Defines.DESCENDING;

            var id1 = Factory.TestEngine.CreateSampleMessage((int)FolderType.Inbox, Mbox.MailBoxId,
                new List<string> { "to@to.com" }, new List<string> { "cc@cc.com" }, new List<string>(), false, true,
                "[TEST SUBJECT]", "This is SPARTA", date: new DateTime(2018, 09, 18, 9, 2, 33));

            Assert.Greater(id1, 0);

            var id2 = Factory.TestEngine.CreateSampleMessage((int)FolderType.Inbox, Mbox.MailBoxId,
                new List<string> { "to@todo.com" }, new List<string> { "cc@ccdo.com" }, new List<string>(), false, true,
                "[TEST 2 SUBJECT]", "This is TROY", date: new DateTime(2018, 09, 14, 16, 36, 30));

            Assert.Greater(id2, 0);

            var id3 = Factory.TestEngine.CreateSampleMessage((int)FolderType.Inbox, Mbox.MailBoxId,
                new List<string> { "to@todo.com" }, new List<string> { "cc@ccdo.com" }, new List<string>(), false, true,
                "[TEST 3 SUBJECT]", "This is ROMA", date: new DateTime(2018, 09, 15, 7, 51, 12));

            Assert.Greater(id3, 0);

            var ids = new[] { id1, id2, id3 };

            foreach (var id in ids)
            {
                var message = Factory.MessageEngine.GetMessage(id, new MailMessageData.Options());

                var mailWrapper = message.ToMailWrapper(CURRENT_TENANT, TestUser.ID);

                FactoryIndexer<MailWrapper>.Index(mailWrapper);
            }

            var selector = new Selector<MailWrapper>()
                .Where(r => r.Folder, (int)FolderType.Inbox)
                .Where(r => r.UserId, TestUser.ID)
                .Sort(r => r.DateSent, sort_order == Defines.ASCENDING);

            List<int> mailIds;

            var success = FactoryIndexer<MailWrapper>.TrySelectIds(m => selector, out mailIds);

            Assert.AreEqual(true, success);
            Assert.AreEqual(3, mailIds.Count);
            Assert.AreEqual(id1, mailIds[0]);
            Assert.AreEqual(id3, mailIds[1]);
            Assert.AreEqual(id2, mailIds[2]);

            var selector1 = new Selector<MailWrapper>();

            selector1.Where(r => r.Folder, (int)FolderType.Inbox);

            selector1.Where(r => r.UserId, TestUser.ID)
                .Sort(r => r.DateSent, sort_order == Defines.ASCENDING);

            success = FactoryIndexer<MailWrapper>.TrySelectIds(m => selector1, out mailIds);

            Assert.AreEqual(true, success);
            Assert.AreEqual(3, mailIds.Count);
            Assert.AreEqual(id1, mailIds[0]);
            Assert.AreEqual(id3, mailIds[1]);
            Assert.AreEqual(id2, mailIds[2]);

            var index = 0;
            const int max = 2;

            var selector2 = new Selector<MailWrapper>()
                .Where(r => r.Folder, (int)FolderType.Inbox)
                .Limit(index, max)
                .Where(r => r.UserId, TestUser.ID)
                .Sort(r => r.DateSent, sort_order == Defines.ASCENDING);

            success = FactoryIndexer<MailWrapper>.TrySelectIds(m => selector2, out mailIds);

            Assert.AreEqual(true, success);
            Assert.AreEqual(2, mailIds.Count);
            Assert.AreEqual(id1, mailIds[0]);
            Assert.AreEqual(id3, mailIds[1]);

            index += mailIds.Count;

            var selector3 = new Selector<MailWrapper>()
                .Where(r => r.Folder, (int)FolderType.Inbox)
                .Limit(index, max)
                .Where(r => r.UserId, TestUser.ID)
                .Sort(r => r.DateSent, sort_order == Defines.ASCENDING);

            success = FactoryIndexer<MailWrapper>.TrySelectIds(m => selector3, out mailIds);

            Assert.AreEqual(true, success);
            Assert.AreEqual(1, mailIds.Count);
            Assert.AreEqual(id2, mailIds[0]);

            var filter = new MailSearchFilterData
            {
                PrimaryFolder = FolderType.Inbox,
                PageSize = 25,
                Sort = Defines.ORDER_BY_DATE_SENT,
                SortOrder = sort_order
            };

            long totalMessages;

            var messages = Factory.MessageEngine.GetFilteredMessages(filter, out totalMessages);

            Assert.AreEqual(true, success);
            Assert.AreEqual(3, messages.Count);
            Assert.AreEqual(id1, messages[0].Id);
            Assert.AreEqual(id3, messages[1].Id);
            Assert.AreEqual(id2, messages[2].Id);
        }

        [Test, IgnoreIfFullTextSearch(enabled: false)]
        public void CheckFtPrevOrNextMessage()
        {
            var id1 = Factory.TestEngine.CreateSampleMessage((int)FolderType.Inbox, Mbox.MailBoxId,
                new List<string> { "to@to.com" }, new List<string> { "cc@cc.com" }, new List<string>(), false, true,
                "[TEST SUBJECT]", "This is SPARTA", date: new DateTime(2018, 09, 18, 9, 2, 33));

            Assert.Greater(id1, 0);

            var id2 = Factory.TestEngine.CreateSampleMessage((int)FolderType.Inbox, Mbox.MailBoxId,
                new List<string> { "to@todo.com" }, new List<string> { "cc@ccdo.com" }, new List<string>(), false, true,
                "[TEST 2 SUBJECT]", "This is TROY", date: new DateTime(2018, 09, 14, 16, 36, 30));

            Assert.Greater(id2, 0);

            var id3 = Factory.TestEngine.CreateSampleMessage((int)FolderType.Inbox, Mbox.MailBoxId,
                new List<string> { "to@todo.com" }, new List<string> { "cc@ccdo.com" }, new List<string>(), false, true,
                "[TEST 3 SUBJECT]", "This is ROMA", date: new DateTime(2018, 09, 15, 7, 51, 12));

            Assert.Greater(id3, 0);

            var ids = new[] { id1, id2, id3 };

            foreach (var id in ids)
            {
                var message = Factory.MessageEngine.GetMessage(id, new MailMessageData.Options());

                var mailWrapper = message.ToMailWrapper(CURRENT_TENANT, TestUser.ID);

                FactoryIndexer<MailWrapper>.Index(mailWrapper);
            }

            var filter = new MailSearchFilterData
            {
                PrimaryFolder = FolderType.Inbox,
                PageSize = 25,
                Sort = Defines.ORDER_BY_DATE_SENT,
                SortOrder = Defines.DESCENDING
            };

            var nextId = Factory.MessageEngine.GetNextFilteredMessageId(id1, filter);

            Assert.AreEqual(id3, nextId);

            nextId = Factory.MessageEngine.GetNextFilteredMessageId(id2, filter);

            Assert.AreEqual(-1, nextId);

            filter = new MailSearchFilterData
            {
                PrimaryFolder = FolderType.Inbox,
                PageSize = 25,
                Sort = Defines.ORDER_BY_DATE_SENT,
                SortOrder = Defines.ASCENDING
            };

            var prevId = Factory.MessageEngine.GetNextFilteredMessageId(id2, filter);

            Assert.AreEqual(id3, prevId);

            prevId = Factory.MessageEngine.GetNextFilteredMessageId(id1, filter);

            Assert.AreEqual(-1, prevId);
        }

        [Test, IgnoreIfFullTextSearch(enabled: false)]
        public void CheckFtSearchWithTags()
        {
            var tag1 = Factory.TagEngine.CreateTag("Tag1", "4", new List<string>());

            Assert.Greater(tag1.Id, 0);

            var tag2 = Factory.TagEngine.CreateTag("Tag2", "3", new List<string>());

            Assert.Greater(tag2.Id, 0);

            var tagIds1 = new List<int> { tag2.Id, tag1.Id };
            var tagIds2 = new List<int> { tag2.Id };

            var id1 = Factory.TestEngine.CreateSampleMessage((int)FolderType.Inbox, Mbox.MailBoxId,
                new List<string> { "to@to.com" }, new List<string> { "cc@cc.com" }, new List<string>(), false, true,
                "[TEST SUBJECT]", "This is SPARTA", tagIds: tagIds1);

            Assert.Greater(id1, 0);

            var id2 = Factory.TestEngine.CreateSampleMessage((int)FolderType.Inbox, Mbox.MailBoxId,
                new List<string> { "to@todo.com" }, new List<string> { "cc@ccdo.com" }, new List<string>(), false, true,
                "[TEST 2 SUBJECT]", "This is TROY", tagIds: tagIds2);

            Assert.Greater(id2, 0);

            var id3 = Factory.TestEngine.CreateSampleMessage((int)FolderType.Inbox, Mbox.MailBoxId,
                new List<string> { "to@todo.com" }, new List<string> { "cc@ccdo.com" }, new List<string>(), false, true,
                "[TEST 3 SUBJECT]", "This is TROY");

            Assert.Greater(id3, 0);

            var message = Factory.MessageEngine.GetMessage(id1, new MailMessageData.Options());
            var mailWrapper = message.ToMailWrapper(CURRENT_TENANT, TestUser.ID);

            FactoryIndexer<MailWrapper>.Index(mailWrapper);

            var message2 = Factory.MessageEngine.GetMessage(id2, new MailMessageData.Options());
            var mailWrapper2 = message2.ToMailWrapper(CURRENT_TENANT, TestUser.ID);

            FactoryIndexer<MailWrapper>.Index(mailWrapper2);

            var message3 = Factory.MessageEngine.GetMessage(id3, new MailMessageData.Options());
            var mailWrapper3 = message3.ToMailWrapper(CURRENT_TENANT, TestUser.ID);

            FactoryIndexer<MailWrapper>.Index(mailWrapper3);

            List<int> mailIds;

            var selectorOneTag = new Selector<MailWrapper>()
                .InAll(r => r.Tags.Select(t => t.Id), tagIds2.ToArray())
                .Where(r => r.UserId, TestUser.ID)
                .Sort(r => r.DateSent, true);

            var success = FactoryIndexer<MailWrapper>.TrySelectIds(m => selectorOneTag, out mailIds);

            Assert.AreEqual(true, success);
            Assert.AreEqual(2, mailIds.Count);

            var ids = new[] { id1, id2 };
            Assert.AreEqual(true, mailIds.All(id => ids.Contains(id)));

            var selectorTwoTags = new Selector<MailWrapper>()
                .InAll(r => r.Tags.Select(t => t.Id), tagIds1.ToArray())
                .Where(r => r.UserId, TestUser.ID)
                .Sort(r => r.DateSent, true);

            success = FactoryIndexer<MailWrapper>.TrySelectIds(m => selectorTwoTags, out mailIds);

            Assert.AreEqual(true, success);
            Assert.AreEqual(1, mailIds.Count);
            Assert.AreEqual(id1, mailIds.First());
        }

        private List<int> GenerateIndexedMessages(int count)
        {
            var idList = new List<int>(count);

            var i = 0;

            var date = DateTime.Now;

            while (i < count)
            {
                var id = Factory.TestEngine.CreateSampleMessage((int)FolderType.Inbox, Mbox.MailBoxId,
                    new List<string> { "to@to.com" }, new List<string> { "cc@cc.com" }, new List<string>(), false, true,
                    string.Format("{0} [TEST SUBJECT]", i), "This is SPARTA", date: date, add2Index: true);

                idList.Add(id);

                i++;
                date = date.AddHours(-i);
            }

            return idList;
        }


        [Test, IgnoreIfFullTextSearch(enabled: false)]
        public void CheckFtUnreadMessagesPaging()
        {
            const int TOTAL_COUNT = 27;
            const int PAGE_SIZE = 25;

            // control message
            var ids = GenerateIndexedMessages(TOTAL_COUNT);

            var id = Factory.TestEngine.CreateSampleMessage((int)FolderType.Inbox, Mbox.MailBoxId,
                    new List<string> { "to@to.com" }, new List<string> { "cc@cc.com" }, new List<string>(), false, false,
                    "[TEST CONTROL SUBJECT]", "This is SPARTA", date: new DateTime(2018, 09, 18, 9, 2, 33), add2Index: true);

            Assert.Greater(id, 0);

            Assert.AreEqual(TOTAL_COUNT, ids.Count);

            var filter = new MailSearchFilterData
            {
                PrimaryFolder = FolderType.Inbox,
                Unread = true,
                Page = 0,
                PageSize = PAGE_SIZE,
                Sort = Defines.ORDER_BY_DATE_SENT,
                SortOrder = Defines.ASCENDING
            };

            long totalMessagesCount;

            var messages = Factory.MessageEngine.GetFilteredMessages(filter, out totalMessagesCount); // FIRST PAGE

            Assert.AreEqual(PAGE_SIZE, messages.Count);
            Assert.AreEqual(TOTAL_COUNT, totalMessagesCount);

            filter.Page++;

            messages = Factory.MessageEngine.GetFilteredMessages(filter, out totalMessagesCount); // SECOND AND LAST PAGE

            Assert.AreEqual(TOTAL_COUNT - PAGE_SIZE, messages.Count);
            Assert.AreEqual(TOTAL_COUNT - PAGE_SIZE, totalMessagesCount);

            filter.Page--;

            messages = Factory.MessageEngine.GetFilteredMessages(filter, out totalMessagesCount); // BACK TO FIRST PAGE

            Assert.AreEqual(PAGE_SIZE, messages.Count);
            Assert.AreEqual(TOTAL_COUNT, totalMessagesCount);

            filter.Page = -1;

            messages = Factory.MessageEngine.GetFilteredMessages(filter, out totalMessagesCount); // NEGATIVE PAGE TEST

            Assert.AreEqual(0, messages.Count);
            Assert.AreEqual(0, totalMessagesCount);

            filter.Page = 2;

            messages = Factory.MessageEngine.GetFilteredMessages(filter, out totalMessagesCount); // OVERFLOW PAGE TEST

            Assert.AreEqual(0, messages.Count);
            Assert.AreEqual(0, totalMessagesCount);
        }
    }
}
