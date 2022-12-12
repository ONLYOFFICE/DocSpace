// (c) Copyright Ascensio System SIA 2010-2022
//
// This program is a free software product.
// You can redistribute it and/or modify it under the terms
// of the GNU Affero General Public License (AGPL) version 3 as published by the Free Software
// Foundation. In accordance with Section 7(a) of the GNU AGPL its Section 15 shall be amended
// to the effect that Ascensio System SIA expressly excludes the warranty of non-infringement of
// any third-party rights.
//
// This program is distributed WITHOUT ANY WARRANTY, without even the implied warranty
// of MERCHANTABILITY or FITNESS FOR A PARTICULAR  PURPOSE. For details, see
// the GNU AGPL at: http://www.gnu.org/licenses/agpl-3.0.html
//
// You can contact Ascensio System SIA at Lubanas st. 125a-25, Riga, Latvia, EU, LV-1021.
//
// The  interactive user interfaces in modified source and object code versions of the Program must
// display Appropriate Legal Notices, as required under Section 5 of the GNU AGPL version 3.
//
// Pursuant to Section 7(b) of the License you must retain the original Product logo when
// distributing the program. Pursuant to Section 7(e) we decline to grant you any rights under
// trademark law for use of our trademarks.
//
// All the Product's GUI elements, including illustrations and icon sets, as well as technical writing
// content are licensed under the terms of the Creative Commons Attribution-ShareAlike 4.0
// International. See the License terms at http://creativecommons.org/licenses/by-sa/4.0/legalcode



namespace ASC.Web.Api.Tests;

[Serializable]
public class WizardData
{
    public bool Completed { get; set; }
}

[TestFixture]
class WizardTest : BaseApiTests
{
    private UserInfo _newUser;

    [OneTimeSetUp]
    public override void SetUp()
    {
        base.SetUp();
        _newUser = _userManager.GetUsers(_currentTenant.OwnerId);
    }

    [TestCaseSource(typeof(ApiTestsData), nameof(ApiTestsData.WebStudioSettingsData))]
    [Category("Wizard")]
    [Order(1)]
    public void WizardDataTest(int tenant, Guid userId, string data)
    {
        var user = _userManager.GetUsers(Guid.Parse("99223c7b-e3c9-11eb-9063-982cbc0ea1e5"));
        _securityContext.AuthenticateMe(user.Id);
        var WebStudioData = new DbWebstudioSettings
        {
            TenantId = tenant,
            Id = user.Id,
            UserId = userId,
            Data = data
        };

        var wizard = _settingsManager.Load<WizardSettings>();
        var WizardSet = new WizardData
        {
            Completed = wizard.Completed
        };
        var json = JsonSerializer.Serialize(WizardSet);
        Assert.AreEqual(WebStudioData.Data, json);
    }


    [TestCaseSource(typeof(ApiTestsData), nameof(ApiTestsData.UserForWizard))]
    [Category("Wizard")]
    [Order(2)]
    public void WizardSaveDataTest(string email, string passwordHash, string lng, string timeZone, string promocode, string amiid, bool subscribeFromSite)
    {
        var wizardModel = new WizardRequestsDto
        {
            Email = email,
            PasswordHash = passwordHash,
            Lng = lng,
            TimeZone = timeZone,
            AmiId = amiid,
            SubscribeFromSite = subscribeFromSite
        };
        var wizard = _firstTimeTenantSettings.SaveData(wizardModel);
        _securityContext.AuthenticateMe(_currentTenant.OwnerId);
        Assert.IsTrue(wizard.Completed);
        Assert.Throws<Exception>(() => _firstTimeTenantSettings.SaveData(wizardModel));
    }

}

