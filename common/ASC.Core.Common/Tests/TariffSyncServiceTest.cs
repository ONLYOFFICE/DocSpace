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

    using ASC.Common.Logging;
    using ASC.Core.Billing;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Options;

    using NUnit.Framework;

    [TestFixture]
    public class TariffSyncServiceTest
    {
        private readonly ITariffSyncService tariffSyncService;

        public IServiceProvider ServiceProvider { get; set; }
        public IConfiguration Configuration { get; set; }
        public IOptionsMonitor<ILog> Options { get; set; }

        public TariffSyncServiceTest()
        {
            tariffSyncService = new TariffSyncService(null, null, null, null);
        }

        [Test]
        public void GetTeriffsTest()
        {
            var tariff = tariffSyncService.GetTariffs(70, null).FirstOrDefault(t => t.Id == -38);
            Assert.AreEqual(1024 * 1024 * 1024, tariff.MaxFileSize);
            tariff = tariffSyncService.GetTariffs(74, null).FirstOrDefault(t => t.Id == -38);
            Assert.AreEqual(100 * 1024 * 1024, tariff.MaxFileSize);
        }

        [Test]
        public void SyncTest()
        {
            using var wcfClient = new TariffSyncClient();
            var tariffs = wcfClient.GetTariffs(74, null);
            Assert.IsTrue(tariffs.Any());
        }
    }
}
#endif
