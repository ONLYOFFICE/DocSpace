using System.Collections.Generic;
using NUnit.Framework;

namespace ASC.Web.Api.Tests.Infrastructure
{
    class ApiTestsData
    {
        public static IEnumerable<TestCaseData> UserForWizard()
        {
            yield return new TestCaseData("testuser@onlyoffice.com", "11111111", "en-US", "UTC", "", "", false);
        }

        public static IEnumerable<TestCaseData> WizardGetSettings()
        {
            yield return new TestCaseData("66faa6e4-f133-11ea-b126-00ffeec8b4ef", "en-US", "UTC");
        }
    }
}
