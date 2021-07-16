using System;

using ASC.Api.Settings;
using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.Core.Users;
using ASC.Web.Api.Models;
using ASC.Web.Api.Routing;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Studio.UserControls.FirstTime;

using Microsoft.AspNetCore.Authorization;

using NUnit.Framework;

namespace ASC.Files.Tests
{
    [TestFixture]
    class WizardTest : BaseFilesTests
    {
        private TenantManager tenantManager { get; set; }
        private SettingsManager settingsManager { get; set; }
        public UserInfo NewUser { get; set; }
        public FirstTimeTenantSettings FirstTimeTenantSettings { get; set; }
        
        public WizardSettings wizardSettings { get; set; }
        

        [OneTimeSetUp]
        public override void SetUp()
        {
            base.SetUp();
        }

        [TestCaseSource(typeof(DocumentData), nameof(DocumentData.UserForWizard))]
        [Category("Wizard")]
        public void WizardSaveDataTest(string email, string passwordHash, string lng, string timeZone, string promocode, string amiid, bool subscribeFromSite)
        {
            var wizardModel = new WizardModel
            {
                Email = email,
                PasswordHash = passwordHash,
                Lng = lng,
                TimeZone = timeZone,
                Promocode = promocode,
                AmiId = amiid,
                SubscribeFromSite = subscribeFromSite
            };

            var wizard = FirstTimeTenantSettings.SaveData(wizardModel);

            Assert.IsTrue(wizard.Completed);
           
        }

        

    }
}
