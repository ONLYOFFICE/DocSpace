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
using ASC.Mail.Aggregator.Tests.Common.Utils;
using ASC.Mail.Aggregator.Tests.Utils;
using ASC.Mail.Core;
using ASC.Mail.Data.Contracts;
using ASC.Mail.Enums;
using ASC.Mail.Enums.Filter;
using ASC.Mail.Utils;
using NUnit.Framework;

namespace ASC.Mail.Aggregator.Tests.Common.Filters
{
    [TestFixture]
    internal class SqlFilteringTests
    {
        private const int CURRENT_TENANT = 0;
        public const string PASSWORD = "123456";
        public const string DOMAIN = "gmail.com";

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

            // Clear TestUser1 mail data
            var eraser = Factory.MailGarbageEngine;

            eraser.ClearUserMail(TestUser.ID, CoreContext.TenantManager.GetCurrentTenant());
        }

        [Test, IgnoreIfFullTextSearch(enabled: true)]
        public void CheckSimpleFilterFromMatch()
        {
            var id = Factory.TestEngine.CreateSampleMessage((int)FolderType.Inbox, Mbox.MailBoxId,
                new List<string> { "to@to.com" }, new List<string> { "cc@cc.com" }, new List<string>(), false, true,
                "[TEST SUBJECT]", "This is SPARTA");

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
                }
            };

            long total;
            var messages = Factory.MessageEngine.GetFilteredMessages(filter, PAGE, LIMIT, out total);

            Assert.AreEqual(1, total);
            Assert.AreEqual(1, messages.Count);
            Assert.AreEqual(Mbox.EMail.Address, messages[0].From);
        }

        [Test, IgnoreIfFullTextSearch(enabled: true)]
        public void CheckSimpleFilterFromContains()
        {
            var id = Factory.TestEngine.CreateSampleMessage((int)FolderType.Inbox, Mbox.MailBoxId,
                new List<string> { "to@to.com" }, new List<string> { "cc@cc.com" }, new List<string>(), false, true,
                "[TEST SUBJECT]", "This is SPARTA");

            Assert.Greater(id, 0);

            var filter = new MailSieveFilterData
            {
                Conditions = new List<MailSieveFilterConditionData>
                {
                    new MailSieveFilterConditionData
                    {
                        Key = ConditionKeyType.From,
                        Operation = ConditionOperationType.Contains,
                        Value = Mbox.EMail.Host
                    }
                }
            };

            long total;
            var messages = Factory.MessageEngine.GetFilteredMessages(filter, PAGE, LIMIT, out total);

            Assert.AreEqual(1, total);
            Assert.AreEqual(1, messages.Count);
            Assert.AreEqual(Mbox.EMail.Address, messages[0].From);
        }

        [Test, IgnoreIfFullTextSearch(enabled: true)]
        public void CheckSimpleFilterToMatch()
        {
            var id = Factory.TestEngine.CreateSampleMessage((int) FolderType.Inbox, Mbox.MailBoxId,
                new List<string> {"to@to.com"}, new List<string> {"cc@cc.com"}, new List<string>(), false, true,
                "[TEST SUBJECT]", "This is SPARTA");

            Assert.Greater(id, 0);

            var filter = new MailSieveFilterData
            {
                Conditions = new List<MailSieveFilterConditionData>
                {
                    new MailSieveFilterConditionData
                    {
                        Key = ConditionKeyType.To,
                        Operation = ConditionOperationType.Matches,
                        Value = "to@to.com"
                    }
                }
            };

            long total;
            var messages = Factory.MessageEngine.GetFilteredMessages(filter, PAGE, LIMIT, out total);

            Assert.AreEqual(1, total);
            Assert.AreEqual(1, messages.Count);
            Assert.AreEqual("to@to.com", messages[0].To);
        }

        [Test, IgnoreIfFullTextSearch(enabled: true)]
        public void CheckSimpleFilterToContains()
        {
            var id = Factory.TestEngine.CreateSampleMessage((int)FolderType.Inbox, Mbox.MailBoxId,
                new List<string> { "to@to.com" }, new List<string> { "cc@cc.com" }, new List<string>(), false, true,
                "[TEST SUBJECT]", "This is SPARTA");

            Assert.Greater(id, 0);

            var filter = new MailSieveFilterData
            {
                Conditions = new List<MailSieveFilterConditionData>
                {
                    new MailSieveFilterConditionData
                    {
                        Key = ConditionKeyType.To,
                        Operation = ConditionOperationType.Contains,
                        Value = "@to.com"
                    }
                }
            };

            long total;
            var messages = Factory.MessageEngine.GetFilteredMessages(filter, PAGE, LIMIT, out total);

            Assert.AreEqual(1, total);
            Assert.AreEqual(1, messages.Count);
            Assert.AreEqual("to@to.com", messages[0].To);
        }

        [Test, IgnoreIfFullTextSearch(enabled: true)]
        public void CheckSimpleFilterCcMatch()
        {
            var id = Factory.TestEngine.CreateSampleMessage((int)FolderType.Inbox, Mbox.MailBoxId,
                new List<string> { "to@to.com" }, new List<string> { "cc@cc.com" }, new List<string>(), false, true,
                "[TEST SUBJECT]", "This is SPARTA");

            Assert.Greater(id, 0);

            var filter = new MailSieveFilterData
            {
                Conditions = new List<MailSieveFilterConditionData>
                {
                    new MailSieveFilterConditionData
                    {
                        Key = ConditionKeyType.Cc,
                        Operation = ConditionOperationType.Matches,
                        Value = "cc@cc.com"
                    }
                }
            };

            long total;
            var messages = Factory.MessageEngine.GetFilteredMessages(filter, PAGE, LIMIT, out total);

            Assert.AreEqual(1, total);
            Assert.AreEqual(1, messages.Count);
            Assert.AreEqual("cc@cc.com", messages[0].Cc);
        }

        [Test, IgnoreIfFullTextSearch(enabled: true)]
        public void CheckSimpleFilterCcContains()
        {
            var id = Factory.TestEngine.CreateSampleMessage((int)FolderType.Inbox, Mbox.MailBoxId,
                new List<string> { "to@to.com" }, new List<string> { "cc@cc.com" }, new List<string>(), false, true,
                "[TEST SUBJECT]", "This is SPARTA");

            Assert.Greater(id, 0);

            var filter = new MailSieveFilterData
            {
                Conditions = new List<MailSieveFilterConditionData>
                {
                    new MailSieveFilterConditionData
                    {
                        Key = ConditionKeyType.Cc,
                        Operation = ConditionOperationType.Contains,
                        Value = "cc"
                    }
                }
            };

            long total;
            var messages = Factory.MessageEngine.GetFilteredMessages(filter, PAGE, LIMIT, out total);

            Assert.AreEqual(1, total);
            Assert.AreEqual(1, messages.Count);
            Assert.AreEqual("cc@cc.com", messages[0].Cc);
        }

        [Test, IgnoreIfFullTextSearch(enabled: true)]
        public void CheckSimpleFilterToOrCcMatch()
        {
            var id = Factory.TestEngine.CreateSampleMessage((int)FolderType.Inbox, Mbox.MailBoxId,
                new List<string> { "to@to.com" }, new List<string> { "cc@cc.com" }, new List<string>(), false, true,
                "[TEST SUBJECT]", "This is SPARTA");

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
        }

        [Test, IgnoreIfFullTextSearch(enabled: true)]
        public void CheckSimpleFilterToOrCcContains()
        {
            var id = Factory.TestEngine.CreateSampleMessage((int)FolderType.Inbox, Mbox.MailBoxId,
                new List<string> { "to@to.com" }, new List<string> { "cc@cc.com" }, new List<string>(), false, true,
                "[TEST SUBJECT]", "This is SPARTA");

            Assert.Greater(id, 0);

            var filter = new MailSieveFilterData
            {
                Conditions = new List<MailSieveFilterConditionData>
                {
                    new MailSieveFilterConditionData
                    {
                        Key = ConditionKeyType.ToOrCc,
                        Operation = ConditionOperationType.Contains,
                        Value = "to"
                    }
                }
            };

            long total;
            var messages = Factory.MessageEngine.GetFilteredMessages(filter, PAGE, LIMIT, out total);

            Assert.AreEqual(1, total);
            Assert.AreEqual(1, messages.Count);
            Assert.AreEqual("to@to.com", messages[0].To);
            Assert.AreEqual("cc@cc.com", messages[0].Cc);
        }

        [Test, IgnoreIfFullTextSearch(enabled: true)]
        public void CheckSimpleFilterSubjectMatch()
        {
            var id = Factory.TestEngine.CreateSampleMessage((int)FolderType.Inbox, Mbox.MailBoxId,
                new List<string> { "to@to.com" }, new List<string> { "cc@cc.com" }, new List<string>(), false, true,
                "[TEST SUBJECT]", "This is SPARTA");

            Assert.Greater(id, 0);

            var filter = new MailSieveFilterData
            {
                Conditions = new List<MailSieveFilterConditionData>
                {
                    new MailSieveFilterConditionData
                    {
                        Key = ConditionKeyType.Subject,
                        Operation = ConditionOperationType.Matches,
                        Value = "[TEST SUBJECT]"
                    }
                }
            };

            long total;
            var messages = Factory.MessageEngine.GetFilteredMessages(filter, PAGE, LIMIT, out total);

            Assert.AreEqual(1, total);
            Assert.AreEqual(1, messages.Count);
            Assert.AreEqual("[TEST SUBJECT]", messages[0].Subject);
        }

        [Test, IgnoreIfFullTextSearch(enabled: true)]
        public void CheckSimpleFilterSubjectContains()
        {
            var id = Factory.TestEngine.CreateSampleMessage((int)FolderType.Inbox, Mbox.MailBoxId,
                new List<string> { "to@to.com" }, new List<string> { "cc@cc.com" }, new List<string>(), false, true,
                "[TEST SUBJECT]", "This is SPARTA");

            Assert.Greater(id, 0);

            var filter = new MailSieveFilterData
            {
                Conditions = new List<MailSieveFilterConditionData>
                {
                    new MailSieveFilterConditionData
                    {
                        Key = ConditionKeyType.Subject,
                        Operation = ConditionOperationType.Contains,
                        Value = "SUBJECT"
                    }
                }
            };

            long total;
            var messages = Factory.MessageEngine.GetFilteredMessages(filter, PAGE, LIMIT, out total);

            Assert.AreEqual(1, total);
            Assert.AreEqual(1, messages.Count);
            Assert.AreEqual("[TEST SUBJECT]", messages[0].Subject);
        }

        [Test, IgnoreIfFullTextSearch(enabled: true)]
        public void CheckComplexFilterFromAndSubjectMatchAll()
        {
            var id = Factory.TestEngine.CreateSampleMessage((int)FolderType.Inbox, Mbox.MailBoxId,
                new List<string> { "to@to.com" }, new List<string> { "cc@cc.com" }, new List<string>(), false, true,
                "[TEST SUBJECT]", "This is SPARTA");

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
                        Value = "[TEST SUBJECT]"
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
            Assert.AreEqual(Mbox.EMail.Address, messages[0].From);
            Assert.AreEqual("[TEST SUBJECT]", messages[0].Subject);
        }

        [Test, IgnoreIfFullTextSearch(enabled: true)]
        public void CheckComplexFilterFromAndSubjectMatchAtLeastOne()
        {
            var id = Factory.TestEngine.CreateSampleMessage((int)FolderType.Inbox, Mbox.MailBoxId,
                new List<string> { "to@to.com" }, new List<string> { "cc@cc.com" }, new List<string>(), false, true,
                "[TEST SUBJECT]", "This is SPARTA");

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
                        Value = "[TEST SUBJECT1]"
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
            Assert.AreEqual(Mbox.EMail.Address, messages[0].From);
            Assert.AreEqual("[TEST SUBJECT]", messages[0].Subject);
        }
    }
}
