using ASC.Core;
using ASC.Core.Common.Settings;
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
        private TenantManager tenantManager { get; set; }
        private SettingsManager settingsManager { get; set; }

        public WizardSettings wizardSettings { get; set; }


        [OneTimeSetUp]
        public override void SetUp()
        {
            base.SetUp();
        }


        [TestCaseSource(typeof(ApiTestsData), nameof(ApiTestsData.UserForWizard))]
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

            var wizard = firstTimeTenantSettings.SaveData(wizardModel);

            Assert.IsTrue(wizard.Completed);

        }

        [TestCaseSource(typeof(ApiTestsData), nameof(ApiTestsData.WizardGetSettings))]
        [Category("Wizard")]
        public void WizardGetSettings(string ownerId, string lng, string timeZone)
        {
            var trustedDomains = CurrentTenant.TrustedDomains;

            var wizardsettings = settingsController.GetSettings();

            Assert.AreEqual(lng, wizardsettings.Culture);
            Assert.AreEqual(timeZone, wizardsettings.Timezone);
            Assert.AreEqual(trustedDomains, wizardsettings.TrustedDomains);
        }

    }
}