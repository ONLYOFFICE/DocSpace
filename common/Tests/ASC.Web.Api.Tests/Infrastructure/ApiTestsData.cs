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
    }
}
