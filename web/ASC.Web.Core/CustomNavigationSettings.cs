namespace ASC.Web.Studio.Core
{
    [Serializable]
    public class CustomNavigationSettings : ISettings
    {
        public List<CustomNavigationItem> Items { get; set; }

        public Guid ID
        {
            get { return new Guid("{32E02E4C-925D-4391-BAA4-3B5D223A2104}"); }
        }

        public ISettings GetDefault(IServiceProvider serviceProvider)
        {
            return new CustomNavigationSettings { Items = new List<CustomNavigationItem>() };
        }
    }

    [Serializable]
    public class CustomNavigationItem
    {
        public Guid Id { get; set; }

        public string Label { get; set; }

        public string Url { get; set; }

        public string BigImg { get; set; }

        public string SmallImg { get; set; }

        public bool ShowInMenu { get; set; }

        public bool ShowOnHomePage { get; set; }

        private static string GetDefaultBigImg()
        {
            return "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAGQAAABkAgMAAAANjH3HAAAADFBMVEUAAADJycnJycnJycmiuNtHAAAAA3RSTlMAf4C/aSLHAAAAyElEQVR4Xu3NsQ3CMBSE4YubFB4ilHQegdGSjWACvEpGoEyBYiL05AdnXUGHolx10lf82MmOpfLeo5UoJUhBlpKkRCnhUy7b9XCWkqQMUkYlXVHSf8kTvkHKqKQrSnopg5SRxTMklLmS1MwaSWpmCSQ1MyOzWGZCYrEMEFksA4QqlAFuJJYBcCKxjM3FMySeIfEMC2dMOONCGZZgmdr1ly3TSrJMK9EyJBaaGrHQikYstAiJZRYSyiQEdyg5S8Evckih/YPscsdej0H6dc0TYw4AAAAASUVORK5CYII=";
        }

        private static string GetDefaultSmallImg()
        {
            return "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAUUlEQVR4AWMY/KC5o/cAEP9HxxgKcSpCGELYADyu2E6mAQjNxBlAWPNxkHdwGkBIM3KYYDUAr2ZCAE+oH8eujrAXDsA0k2EAAtDXAGLx4MpsADUgvkRKUlqfAAAAAElFTkSuQmCC";
        }

        public static CustomNavigationItem GetSample()
        {
            return new CustomNavigationItem
            {
                Id = Guid.Empty,
                ShowInMenu = true,
                ShowOnHomePage = true,
                BigImg = GetDefaultBigImg(),
                SmallImg = GetDefaultSmallImg()
            };
        }
    }
}
