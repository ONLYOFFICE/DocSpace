/*
 *
 * (c) Copyright Ascensio System Limited 2010-2018
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


#if DEBUG
namespace ASC.Core.Common.Tests
{
    using System;
    using System.Linq;

    using ASC.Common.Security.Authorizing;
    using ASC.Core.Data;

    using NUnit.Framework;

    [TestFixture]
    class DbAzServiceTest : DbBaseTest<DbAzService>
    {
        [SetUp]
        public void ClearData()
        {
            foreach (var ac in Service.GetAces(Tenant, default))
            {
                if (ac.Tenant == Tenant) Service.RemoveAce(Tenant, ac);
            }
        }

        [Test]
        public void AceRecords()
        {
            var ar1 = new AzRecord(Guid.Empty, Guid.Empty, AceType.Allow);
            Service.SaveAce(Tenant, ar1);

            var ar2 = new AzRecord(new Guid("00000000-0000-0000-0000-000000000001"), new Guid("00000000-0000-0000-0000-000000000002"), AceType.Allow);
            Service.SaveAce(Tenant, ar2);

            var list = Service.GetAces(Tenant, default).ToList();

            CompareAces(ar1, list[list.IndexOf(ar1)]);
            CompareAces(ar2, list[list.IndexOf(ar2)]);

            Service.RemoveAce(Tenant, ar1);
            Service.RemoveAce(Tenant, ar2);

            list = Service.GetAces(Tenant, new DateTime(1900, 1, 1)).ToList();
            CollectionAssert.DoesNotContain(list, ar1);
            CollectionAssert.DoesNotContain(list, ar2);
        }

        private void CompareAces(AzRecord ar1, AzRecord ar2)
        {
            Assert.AreEqual(ar1.Action, ar2.Action);
            Assert.AreEqual(ar1.Object, ar2.Object);
            Assert.AreEqual(ar1.AceType, ar2.AceType);
            Assert.AreEqual(ar1.Subject, ar2.Subject);
            Assert.AreEqual(ar1.Tenant, ar2.Tenant);
        }
    }
}
#endif
