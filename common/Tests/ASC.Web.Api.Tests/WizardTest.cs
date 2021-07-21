using System;

using ASC.Core;
using ASC.Core.Common.Settings;
using ASC.Core.Users;
using ASC.Web.Api.Models;
using ASC.Web.Api.Tests.Infrastructure;
using ASC.Web.Core.Utility.Settings;
using ASC.Web.Studio.UserControls.FirstTime;

using NUnit.Framework;

namespace ASC.Web.Api.Tests
{
    [TestFixture]
    class WizardTest : BaseApiTests
    {
        public UserInfo NewUser { get; set; }

        [OneTimeSetUp]
        public override void SetUp()
        {
            base.SetUp();
            NewUser = UserManager.GetUsers(CurrentTenant.OwnerId);
        }

       
        [TestCaseSource(typeof(ApiTestsData), nameof(ApiTestsData.UserForWizard))]
        [Category("Wizard")]
        [Order(1)]
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
            var wizard = firstTimeTenantSettings.SaveData(wizardModel);
            SecurityContext.AuthenticateMe(CurrentTenant.OwnerId);
            Assert.IsTrue(wizard.Completed);
            Assert.Throws<Exception>(() => firstTimeTenantSettings.SaveData(wizardModel));
        }


    }
}