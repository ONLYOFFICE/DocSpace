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
using System.IO;
using ASC.Core;
using ASC.Mail.Clients;
using ASC.Mail.Core;
using ASC.Mail.Data.Contracts;
using ASC.Mail.Enums;
using ASC.Mail.Enums.Filter;
using ASC.Mail.Extensions;
using NUnit.Framework;

namespace ASC.Mail.Aggregator.Tests.Common.Filters
{
    [TestFixture]
    internal class FilterConditionsTests
    {
        private const int CURRENT_TENANT = 0;

        public EngineFactory Factory { get; private set; }

        public MailMessageData MessageData { get; private set; }

        private static readonly string TestFolderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
            @"..\..\Data\");

        private const string EML1_FILE_NAME = @"Test subject.eml";

        private static readonly string Eml1Path = TestFolderPath + EML1_FILE_NAME;

        public FilterConditionsTests()
        {
            CoreContext.TenantManager.SetCurrentTenant(CURRENT_TENANT);
            SecurityContext.AuthenticateMe(ASC.Core.Configuration.Constants.CoreSystem);

            Factory = new EngineFactory(CURRENT_TENANT, SecurityContext.CurrentAccount.ID.ToString());

            using (var fs = new FileStream(Eml1Path, FileMode.Open, FileAccess.Read))
            {
                var mimeMessage = MailClient.ParseMimeMessage(fs);

                MessageData = mimeMessage.CreateMailMessage(-1, FolderType.Inbox, true, "--chain-id--", DateTime.UtcNow, "--stream-id--");
            }
        }

        [Test]
        public void CheckFromEmailConditionMatchSuccess()
        {
            var condition = new MailSieveFilterConditionData
            {
                Key = ConditionKeyType.From,
                Operation = ConditionOperationType.Matches,
                Value = "mono.mail.4test@gmail.com"
            };

            var success = Factory.FilterEngine.IsConditionSucceed(condition, MessageData);

            Assert.AreEqual(true, success);
        }

        [Test]
        public void CheckFromEmailConditionContainsSuccess()
        {
            var condition = new MailSieveFilterConditionData
            {
                Key = ConditionKeyType.From,
                Operation = ConditionOperationType.Contains,
                Value = "Alexey Safronov"
            };

            var success = Factory.FilterEngine.IsConditionSucceed(condition, MessageData);

            Assert.AreEqual(true, success);
        }

        [Test]
        public void CheckFromEmailConditionNotMatchSuccess()
        {
            var condition = new MailSieveFilterConditionData
            {
                Key = ConditionKeyType.From,
                Operation = ConditionOperationType.NotMatches,
                Value = "to@to.com"
            };

            var success = Factory.FilterEngine.IsConditionSucceed(condition, MessageData);

            Assert.AreEqual(true, success);
        }

        [Test]
        public void CheckFromEmailConditionNotContainsSuccess()
        {
            var condition = new MailSieveFilterConditionData
            {
                Key = ConditionKeyType.From,
                Operation = ConditionOperationType.NotContains,
                Value = "@to.com"
            };

            var success = Factory.FilterEngine.IsConditionSucceed(condition, MessageData);

            Assert.AreEqual(true, success);
        }

        [Test]
        public void CheckToEmailsConditionMatchSuccess()
        {
            var condition = new MailSieveFilterConditionData
            {
                Key = ConditionKeyType.To,
                Operation = ConditionOperationType.Matches,
                Value = "alexey.safronov@onlyoffice.com"
            };

            var success = Factory.FilterEngine.IsConditionSucceed(condition, MessageData);

            Assert.AreEqual(true, success);

            condition = new MailSieveFilterConditionData
            {
                Key = ConditionKeyType.To,
                Operation = ConditionOperationType.Matches,
                Value = "doctor@onlyomail.com"
            };

            success = Factory.FilterEngine.IsConditionSucceed(condition, MessageData);

            Assert.AreEqual(true, success);
        }

        [Test]
        public void CheckCcEmailsConditionMatchSuccess()
        {
            var condition = new MailSieveFilterConditionData
            {
                Key = ConditionKeyType.Cc,
                Operation = ConditionOperationType.Matches,
                Value = "mono.mail.4test@mail.ru"
            };

            var success = Factory.FilterEngine.IsConditionSucceed(condition, MessageData);

            Assert.AreEqual(true, success);

            condition = new MailSieveFilterConditionData
            {
                Key = ConditionKeyType.Cc,
                Operation = ConditionOperationType.Matches,
                Value = "mono.mail.4test@yandex.ru"
            };

            success = Factory.FilterEngine.IsConditionSucceed(condition, MessageData);

            Assert.AreEqual(true, success);
        }

        [Test]
        public void CheckToOrCcEmailsConditionMatchSuccess()
        {
            var condition = new MailSieveFilterConditionData
            {
                Key = ConditionKeyType.ToOrCc,
                Operation = ConditionOperationType.Matches,
                Value = "alexey.safronov@onlyoffice.com"
            };

            var success = Factory.FilterEngine.IsConditionSucceed(condition, MessageData);

            Assert.AreEqual(true, success);

            condition = new MailSieveFilterConditionData
            {
                Key = ConditionKeyType.ToOrCc,
                Operation = ConditionOperationType.Matches,
                Value = "mono.mail.4test@yandex.ru"
            };

            success = Factory.FilterEngine.IsConditionSucceed(condition, MessageData);

            Assert.AreEqual(true, success);
        }

        [Test]
        public void CheckSubjectConditionMatchSuccess()
        {
            var condition = new MailSieveFilterConditionData
            {
                Key = ConditionKeyType.Subject,
                Operation = ConditionOperationType.Matches,
                Value = "Test subject"
            };

            var success = Factory.FilterEngine.IsConditionSucceed(condition, MessageData);

            Assert.AreEqual(true, success);
        }
    }
}
