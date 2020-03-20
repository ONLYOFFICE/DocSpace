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
using ASC.Mail.Core;
using ASC.Mail.Core.Engine;
using ASC.Mail.Data.Contracts;
using ASC.Mail.Enums.Filter;
using NUnit.Framework;

namespace ASC.Mail.Aggregator.Tests.Common.Filters
{
    [TestFixture]
    internal class CrudFiltersTests
    {
        private const int CURRENT_TENANT = 0;
        public const string PASSWORD = "123456";
        public const string DOMAIN = "gmail.com";

        public UserInfo TestUser { get; private set; }

        private EngineFactory _engineFactory;

        [SetUp]
        public void SetUp()
        {
            CoreContext.TenantManager.SetCurrentTenant(CURRENT_TENANT);
            SecurityContext.AuthenticateMe(ASC.Core.Configuration.Constants.CoreSystem);

            TestUser = TestHelper.CreateNewRandomEmployee();

            _engineFactory = new EngineFactory(CURRENT_TENANT, TestUser.ID.ToString());
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
            var eraser = _engineFactory.MailGarbageEngine;

            eraser.ClearUserMail(TestUser.ID, CoreContext.TenantManager.GetCurrentTenant());
        }

        [Test]
        public void CreateBaseFilterTest()
        {
            var engine = _engineFactory.FilterEngine;

            CreateBaseFilter(engine);
        }

        [Test]
        public void CreateFullFilterTest()
        {
            var engine = _engineFactory.FilterEngine;

            var filter = new MailSieveFilterData
            {
                Conditions = new List<MailSieveFilterConditionData>
                {
                    new MailSieveFilterConditionData
                    {
                        Key = ConditionKeyType.From,
                        Operation = ConditionOperationType.Contains,
                        Value = "support@example.com"
                    },
                    new MailSieveFilterConditionData
                    {
                        Key = ConditionKeyType.ToOrCc,
                        Operation = ConditionOperationType.NotContains,
                        Value = "toOrcc@example.com"
                    },
                    new MailSieveFilterConditionData
                    {
                        Key = ConditionKeyType.To,
                        Operation = ConditionOperationType.Matches,
                        Value = "to@example.com"
                    },
                    new MailSieveFilterConditionData
                    {
                        Key = ConditionKeyType.Cc,
                        Operation = ConditionOperationType.NotMatches,
                        Value = "cc@example.com"
                    },
                    new MailSieveFilterConditionData
                    {
                        Key = ConditionKeyType.Subject,
                        Operation = ConditionOperationType.Contains,
                        Value = "[TEST]"
                    }
                },
                Actions = new List<MailSieveFilterActionData>
                {
                    new MailSieveFilterActionData
                    {
                        Action = ActionType.MarkAsRead
                    },
                    new MailSieveFilterActionData
                    {
                        Action = ActionType.DeleteForever
                    },
                    new MailSieveFilterActionData
                    {
                        Action = ActionType.MoveTo,
                        Data = "5" // Spam default folder id
                    },
                    new MailSieveFilterActionData
                    {
                        Action = ActionType.MarkTag,
                        Data = "111" // Fake tag Id
                    }
                },
                Options = new MailSieveFilterOptionsData
                {
                    MatchMultiConditions = MatchMultiConditionsType.MatchAtLeastOne,
                    ApplyTo = new MailSieveFilterOptionsApplyToData
                    {
                        WithAttachments = ApplyToAttachmentsType.WithAndWithoutAttachments
                    }
                }
            };

            var id = engine.Create(filter);

            Assert.Greater(id, 0);
        }

        [Test]
        public void GetFilterTest()
        {
            var engine = _engineFactory.FilterEngine;

            var id = CreateBaseFilter(engine);

            var rFilter = engine.Get(id);

            Assert.IsNotNull(rFilter);

            Assert.AreEqual(id, rFilter.Id);
        }

        [Test]
        public void GetFiltersTest()
        {
            var engine = _engineFactory.FilterEngine;

            CreateBaseFilter(engine);
            CreateBaseFilter(engine);

            var rFilters = engine.GetList();

            Assert.AreEqual(rFilters.Count, 2);
        }

        [Test]
        public void UpdateFilterTest()
        {
            var engine = _engineFactory.FilterEngine;

            var id = CreateBaseFilter(engine);

            var rFilter = engine.Get(id);

            Assert.IsNotNull(rFilter);

            Assert.AreEqual(id, rFilter.Id);

            Assert.AreEqual(rFilter.Conditions.First().Key, ConditionKeyType.From);
            Assert.AreEqual(rFilter.Actions.First().Action, ActionType.MarkAsRead);

            rFilter.Conditions.First().Key = ConditionKeyType.To;
            rFilter.Actions.First().Action = ActionType.DeleteForever;

            engine.Update(rFilter);

            rFilter = engine.Get(id);

            Assert.AreEqual(rFilter.Conditions.First().Key, ConditionKeyType.To);
            Assert.AreEqual(rFilter.Actions.First().Action, ActionType.DeleteForever);
        }

        [Test]
        public void DeleteFilterTest()
        {
            var engine = _engineFactory.FilterEngine;

            var id = CreateBaseFilter(engine);

            var success = engine.Delete(id);

            Assert.AreEqual(true, success);

            var rFilter = engine.Get(id);

            Assert.AreEqual(null, rFilter);
        }

        [Test]
        public void CreateDisabledFilterTest()
        {
            var engine = _engineFactory.FilterEngine;

            var id = CreateBaseFilter(engine, false);

            var rFilter = engine.Get(id);

            Assert.AreEqual(false, rFilter.Enabled);
        }

        [Test]
        public void EnabledFilterTest()
        {
            var engine = _engineFactory.FilterEngine;

            var id = CreateBaseFilter(engine, false);

            var rFilter = engine.Get(id);

            rFilter.Enabled = true;

            var success = engine.Update(rFilter);

            Assert.AreEqual(true, success);

            rFilter = engine.Get(rFilter.Id);

            Assert.AreEqual(true, rFilter.Enabled);
        }

        private static int CreateBaseFilter(FilterEngine engine, bool enabled = true)
        {
            var filter = new MailSieveFilterData
            {
                Enabled = enabled,

                Conditions = new List<MailSieveFilterConditionData>
                {
                    new MailSieveFilterConditionData
                    {
                        Key = ConditionKeyType.From,
                        Operation = ConditionOperationType.Contains,
                        Value = "support@example.com"
                    }
                },
                Actions = new List<MailSieveFilterActionData>
                {
                    new MailSieveFilterActionData
                    {
                        Action = ActionType.MarkAsRead
                    }
                }
            };

            var id = engine.Create(filter);

            Assert.Greater(id, 0);

            return id;
        }
    }
}
