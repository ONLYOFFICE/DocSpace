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
//using ASC.Core;
//using ASC.ElasticSearch;
//using ASC.Mail.Aggregator.Tests.Utils;
//using ASC.Mail.Core;
//using ASC.Mail.Models;
//using ASC.Mail.Enums;
//using NUnit.Framework;

//namespace ASC.Mail.Aggregator.Tests.Common.Filters
//{
//    [TestFixture]
//    internal class ContactSearchFilteringTests
//    {
//        private const int CURRENT_TENANT = 0;
//        private static readonly Guid User = ASC.Core.Configuration.Constants.CoreSystem.ID;

//        private EngineFactory Factory { get; set; }

//        private const int CONTACT_ID_1 = 777;
//        private const int CONTACT_ID_2 = 778;
//        private const int CONTACT_ID_3 = 779;

//        [SetUp]
//        public void SetUp()
//        {
//            CoreContext.TenantManager.SetCurrentTenant(CURRENT_TENANT);
//            SecurityContext.AuthenticateMe(ASC.Core.Configuration.Constants.CoreSystem);

//            Factory = new EngineFactory(CURRENT_TENANT, User.ToString());
//        }

//        [TearDown]
//        public void CleanUp()
//        {
//            Factory.IndexEngine.RemoveContacts(new List<int> { CONTACT_ID_1, CONTACT_ID_2, CONTACT_ID_3 }, CURRENT_TENANT, User);
//        }

//        [Test, IgnoreIfFullTextSearch(enabled: false)]
//        public void CheckContactSearchEmailLocalPartMatch()
//        {
//            var now = DateTime.UtcNow;

//            var contact = new MailContactWrapper
//            {
//                Id = CONTACT_ID_1,
//                TenantId = CURRENT_TENANT,
//                User = User,
//                Name = "Test Contact Name",
//                ContactType = (int)ContactType.Personal,
//                Description = "Some test description",
//                InfoList = new List<MailContactInfoWrapper>
//                {
//                    new MailContactInfoWrapper
//                    {
//                        Id = 776,
//                        TenantId = CURRENT_TENANT,
//                        User = User,
//                        ContactId = CONTACT_ID_1,
//                        InfoType = (int)ContactInfoType.Email,
//                        Text = "qqq@test.ru",
//                        IsPrimary = true,
//                        LastModifiedOn = now
//                    }
//                },
//                LastModifiedOn = now
//            };

//            Factory.IndexEngine.Add(contact);

//            var term = "qqq";

//            var selector = new Selector<MailContactWrapper>()
//                        .MatchAll(term)
//                        .Where(s => s.User, User);

//            List<int> ids;

//            var success = FactoryIndexer<MailContactWrapper>.TrySelectIds(s => selector, out ids);

//            Assert.AreEqual(true, success);
//            Assert.IsNotEmpty(ids);
//            Assert.AreEqual(1, ids.Count);
//            Assert.Contains(CONTACT_ID_1, ids);
//        }

//        [Test, IgnoreIfFullTextSearch(enabled: false)]
//        public void CheckContactSearchEmailDomainMatch()
//        {
//            var now = DateTime.UtcNow;

//            var contact = new MailContactWrapper
//            {
//                Id = CONTACT_ID_1,
//                TenantId = CURRENT_TENANT,
//                User = User,
//                Name = "Test Contact Name",
//                ContactType = (int)ContactType.Personal,
//                Description = "Some test description",
//                InfoList = new List<MailContactInfoWrapper>
//                {
//                    new MailContactInfoWrapper
//                    {
//                        Id = 776,
//                        TenantId = CURRENT_TENANT,
//                        User = User,
//                        ContactId = CONTACT_ID_1,
//                        InfoType = (int)ContactInfoType.Email,
//                        Text = "qqq@test.ru",
//                        IsPrimary = true,
//                        LastModifiedOn = now
//                    }
//                },
//                LastModifiedOn = now
//            };

//            Factory.IndexEngine.Add(contact);

//            var term = "test.ru";

//            var selector = new Selector<MailContactWrapper>()
//                        .MatchAll(term)
//                        .Where(s => s.User, User);

//            List<int> ids;

//            var success = FactoryIndexer<MailContactWrapper>.TrySelectIds(s => selector, out ids);

//            Assert.AreEqual(true, success);
//            Assert.IsNotEmpty(ids);
//            Assert.AreEqual(1, ids.Count);
//            Assert.Contains(CONTACT_ID_1, ids);
//        }

//        [Test, IgnoreIfFullTextSearch(enabled: false)]
//        public void CheckContactSearchFullEmailMatch()
//        {
//            var now = DateTime.UtcNow;

//            var contact = new MailContactWrapper
//            {
//                Id = CONTACT_ID_1,
//                TenantId = CURRENT_TENANT,
//                User = User,
//                Name = "Test Contact Name",
//                ContactType = (int)ContactType.Personal,
//                Description = "Some test description",
//                InfoList = new List<MailContactInfoWrapper>
//                {
//                    new MailContactInfoWrapper
//                    {
//                        Id = 776,
//                        TenantId = CURRENT_TENANT,
//                        User = User,
//                        ContactId = CONTACT_ID_1,
//                        InfoType = (int)ContactInfoType.Email,
//                        Text = "qqq@test.ru",
//                        IsPrimary = true,
//                        LastModifiedOn = now
//                    }
//                },
//                LastModifiedOn = now
//            };

//            Factory.IndexEngine.Add(contact);

//            var term = "qqq@test.ru";

//            var selector = new Selector<MailContactWrapper>()
//                        .MatchAll(term)
//                        .Where(s => s.User, User);

//            List<int> ids;

//            var success = FactoryIndexer<MailContactWrapper>.TrySelectIds(s => selector, out ids);

//            Assert.AreEqual(true, success);
//            Assert.IsNotEmpty(ids);
//            Assert.AreEqual(1, ids.Count);
//            Assert.Contains(CONTACT_ID_1, ids);
//        }

//        [Test, IgnoreIfFullTextSearch(enabled: false)]
//        public void CheckContactSearchNameMatch()
//        {
//            var now = DateTime.UtcNow;

//            var contact1 = new MailContactWrapper
//            {
//                Id = CONTACT_ID_1,
//                TenantId = CURRENT_TENANT,
//                User = User,
//                Name = "Test Contact Name",
//                ContactType = (int)ContactType.Personal,
//                Description = "Some test description",
//                InfoList = new List<MailContactInfoWrapper>
//                {
//                    new MailContactInfoWrapper
//                    {
//                        Id = 1,
//                        TenantId = CURRENT_TENANT,
//                        User = User,
//                        ContactId = CONTACT_ID_1,
//                        InfoType = (int)ContactInfoType.Email,
//                        Text = "qqq1@test.ru",
//                        IsPrimary = true,
//                        LastModifiedOn = now
//                    }
//                },
//                LastModifiedOn = now
//            };

//            Factory.IndexEngine.Add(contact1);

//            var contact2 = new MailContactWrapper
//            {
//                Id = CONTACT_ID_2,
//                TenantId = CURRENT_TENANT,
//                User = User,
//                Name = "This is SPARTA",
//                ContactType = (int)ContactType.Personal,
//                Description = "Some test description",
//                InfoList = new List<MailContactInfoWrapper>
//                {
//                    new MailContactInfoWrapper
//                    {
//                        Id = 2,
//                        TenantId = CURRENT_TENANT,
//                        User = User,
//                        ContactId = CONTACT_ID_2,
//                        InfoType = (int)ContactInfoType.Email,
//                        Text = "qqq2@test.ru",
//                        IsPrimary = true,
//                        LastModifiedOn = now
//                    }
//                },
//                LastModifiedOn = now
//            };

//            Factory.IndexEngine.Add(contact2);

//            var term = "SPARTA";

//            var selector = new Selector<MailContactWrapper>()
//                        .MatchAll(term)
//                        .Where(s => s.User, User);

//            List<int> ids;

//            var success = FactoryIndexer<MailContactWrapper>.TrySelectIds(s => selector, out ids);

//            Assert.AreEqual(true, success);
//            Assert.IsNotEmpty(ids);
//            Assert.AreEqual(1, ids.Count);
//            Assert.Contains(CONTACT_ID_2, ids);
//        }

//        [Test, IgnoreIfFullTextSearch(enabled: false)]
//        public void CheckContactSearchDescriptionMatch()
//        {
//            var now = DateTime.UtcNow;

//            var contact1 = new MailContactWrapper
//            {
//                Id = CONTACT_ID_1,
//                TenantId = CURRENT_TENANT,
//                User = User,
//                Name = "Test Contact Name",
//                ContactType = (int)ContactType.Personal,
//                Description = "Some test description",
//                InfoList = new List<MailContactInfoWrapper>
//                {
//                    new MailContactInfoWrapper
//                    {
//                        Id = 1,
//                        TenantId = CURRENT_TENANT,
//                        User = User,
//                        ContactId = CONTACT_ID_1,
//                        InfoType = (int)ContactInfoType.Email,
//                        Text = "qqq1@test.ru",
//                        IsPrimary = true,
//                        LastModifiedOn = now
//                    }
//                },
//                LastModifiedOn = now
//            };

//            Factory.IndexEngine.Add(contact1);

//            var contact2 = new MailContactWrapper
//            {
//                Id = CONTACT_ID_2,
//                TenantId = CURRENT_TENANT,
//                User = User,
//                Name = "This is SPARTA",
//                ContactType = (int)ContactType.Personal,
//                Description = "Some test description",
//                InfoList = new List<MailContactInfoWrapper>
//                {
//                    new MailContactInfoWrapper
//                    {
//                        Id = 2,
//                        TenantId = CURRENT_TENANT,
//                        User = User,
//                        ContactId = CONTACT_ID_2,
//                        InfoType = (int)ContactInfoType.Email,
//                        Text = "qqq2@test.ru",
//                        IsPrimary = true,
//                        LastModifiedOn = now
//                    }
//                },
//                LastModifiedOn = now
//            };

//            Factory.IndexEngine.Add(contact2);

//            var contact3 = new MailContactWrapper
//            {
//                Id = CONTACT_ID_3,
//                TenantId = CURRENT_TENANT,
//                User = User,
//                Name = "This is TROY",
//                ContactType = (int)ContactType.Personal,
//                Description = "Troy is the best",
//                InfoList = new List<MailContactInfoWrapper>
//                {
//                    new MailContactInfoWrapper
//                    {
//                        Id = 2,
//                        TenantId = CURRENT_TENANT,
//                        User = User,
//                        ContactId = CONTACT_ID_3,
//                        InfoType = (int)ContactInfoType.Email,
//                        Text = "qqq3@test.ru",
//                        IsPrimary = true,
//                        LastModifiedOn = now
//                    }
//                },
//                LastModifiedOn = now
//            };

//            Factory.IndexEngine.Add(contact3);

//            var term = "description";

//            var selector = new Selector<MailContactWrapper>()
//                        .MatchAll(term)
//                        .Where(s => s.User, User);

//            List<int> ids;

//            var success = FactoryIndexer<MailContactWrapper>.TrySelectIds(s => selector, out ids);

//            Assert.AreEqual(true, success);
//            Assert.IsNotEmpty(ids);
//            Assert.AreEqual(2, ids.Count);
//            Assert.Contains(CONTACT_ID_1, ids);
//            Assert.Contains(CONTACT_ID_2, ids);
//        }

//        [Test, IgnoreIfFullTextSearch(enabled: false)]
//        public void CheckContactSearchPartPhoneMatch()
//        {
//            var now = DateTime.UtcNow;

//            var contact1 = new MailContactWrapper
//            {
//                Id = CONTACT_ID_1,
//                TenantId = CURRENT_TENANT,
//                User = User,
//                Name = "Test Contact Name",
//                ContactType = (int)ContactType.Personal,
//                Description = "Some test description",
//                InfoList = new List<MailContactInfoWrapper>
//                {
//                    new MailContactInfoWrapper
//                    {
//                        Id = 1,
//                        TenantId = CURRENT_TENANT,
//                        User = User,
//                        ContactId = CONTACT_ID_1,
//                        InfoType = (int)ContactInfoType.Phone,
//                        Text = "+7999999997",
//                        IsPrimary = true,
//                        LastModifiedOn = now
//                    }
//                },
//                LastModifiedOn = now
//            };

//            Factory.IndexEngine.Add(contact1);

//            var contact2 = new MailContactWrapper
//            {
//                Id = CONTACT_ID_2,
//                TenantId = CURRENT_TENANT,
//                User = User,
//                Name = "This is SPARTA",
//                ContactType = (int)ContactType.Personal,
//                Description = "Some test description",
//                InfoList = new List<MailContactInfoWrapper>
//                {
//                    new MailContactInfoWrapper
//                    {
//                        Id = 2,
//                        TenantId = CURRENT_TENANT,
//                        User = User,
//                        ContactId = CONTACT_ID_2,
//                        InfoType = (int)ContactInfoType.Phone,
//                        Text = "+7999999998",
//                        IsPrimary = true,
//                        LastModifiedOn = now
//                    }
//                },
//                LastModifiedOn = now
//            };

//            Factory.IndexEngine.Add(contact2);

//            var contact3 = new MailContactWrapper
//            {
//                Id = CONTACT_ID_3,
//                TenantId = CURRENT_TENANT,
//                User = User,
//                Name = "This is TROY",
//                ContactType = (int)ContactType.Personal,
//                Description = "Troy is the best",
//                InfoList = new List<MailContactInfoWrapper>
//                {
//                    new MailContactInfoWrapper
//                    {
//                        Id = 2,
//                        TenantId = CURRENT_TENANT,
//                        User = User,
//                        ContactId = CONTACT_ID_3,
//                        InfoType = (int)ContactInfoType.Phone,
//                        Text = "+7999999999",
//                        IsPrimary = true,
//                        LastModifiedOn = now
//                    }
//                },
//                LastModifiedOn = now
//            };

//            Factory.IndexEngine.Add(contact3);

//            var term = "799";

//            var selector = new Selector<MailContactWrapper>()
//                        .MatchAll(term)
//                        .Where(s => s.User, User);

//            List<int> ids;

//            var success = FactoryIndexer<MailContactWrapper>.TrySelectIds(s => selector, out ids);

//            Assert.AreEqual(true, success);
//            Assert.IsNotEmpty(ids);
//            Assert.AreEqual(3, ids.Count);
//            Assert.Contains(CONTACT_ID_1, ids);
//            Assert.Contains(CONTACT_ID_2, ids);
//            Assert.Contains(CONTACT_ID_3, ids);
//        }

//        [Test, IgnoreIfFullTextSearch(enabled: false)]
//        public void CheckContactSearchFullPhoneMatch()
//        {
//            var now = DateTime.UtcNow;

//            var contact1 = new MailContactWrapper
//            {
//                Id = CONTACT_ID_1,
//                TenantId = CURRENT_TENANT,
//                User = User,
//                Name = "Test Contact Name",
//                ContactType = (int)ContactType.Personal,
//                Description = "Some test description",
//                InfoList = new List<MailContactInfoWrapper>
//                {
//                    new MailContactInfoWrapper
//                    {
//                        Id = 1,
//                        TenantId = CURRENT_TENANT,
//                        User = User,
//                        ContactId = CONTACT_ID_1,
//                        InfoType = (int)ContactInfoType.Phone,
//                        Text = "+7999999997",
//                        IsPrimary = true,
//                        LastModifiedOn = now
//                    }
//                },
//                LastModifiedOn = now
//            };

//            Factory.IndexEngine.Add(contact1);

//            var contact2 = new MailContactWrapper
//            {
//                Id = CONTACT_ID_2,
//                TenantId = CURRENT_TENANT,
//                User = User,
//                Name = "This is SPARTA",
//                ContactType = (int)ContactType.Personal,
//                Description = "Some test description",
//                InfoList = new List<MailContactInfoWrapper>
//                {
//                    new MailContactInfoWrapper
//                    {
//                        Id = 2,
//                        TenantId = CURRENT_TENANT,
//                        User = User,
//                        ContactId = CONTACT_ID_2,
//                        InfoType = (int)ContactInfoType.Phone,
//                        Text = "+7999999998",
//                        IsPrimary = true,
//                        LastModifiedOn = now
//                    }
//                },
//                LastModifiedOn = now
//            };

//            Factory.IndexEngine.Add(contact2);

//            var contact3 = new MailContactWrapper
//            {
//                Id = CONTACT_ID_3,
//                TenantId = CURRENT_TENANT,
//                User = User,
//                Name = "This is TROY",
//                ContactType = (int)ContactType.Personal,
//                Description = "Troy is the best",
//                InfoList = new List<MailContactInfoWrapper>
//                {
//                    new MailContactInfoWrapper
//                    {
//                        Id = 2,
//                        TenantId = CURRENT_TENANT,
//                        User = User,
//                        ContactId = CONTACT_ID_3,
//                        InfoType = (int)ContactInfoType.Phone,
//                        Text = "+7999999999",
//                        IsPrimary = true,
//                        LastModifiedOn = now
//                    }
//                },
//                LastModifiedOn = now
//            };

//            Factory.IndexEngine.Add(contact3);

//            var term = "+7999999999";

//            var selector = new Selector<MailContactWrapper>()
//                        .MatchAll(term)
//                        .Where(s => s.User, User);

//            List<int> ids;

//            var success = FactoryIndexer<MailContactWrapper>.TrySelectIds(s => selector, out ids);

//            Assert.AreEqual(true, success);
//            Assert.IsNotEmpty(ids);
//            Assert.AreEqual(1, ids.Count);
//            Assert.Contains(CONTACT_ID_3, ids);
//        }
//    }
//}
