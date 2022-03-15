namespace ASC.Web.Core.Utility.Settings
{
    public class WebItemSettings : ISettings
    {
        public Guid ID
        {
            get { return new Guid("{C888CF56-585B-4c78-9E64-FE1093649A62}"); }
        }
        [JsonPropertyName("Settings")]
        public List<WebItemOption> SettingsCollection { get; set; }

        public WebItemSettings()
        {
            SettingsCollection = new List<WebItemOption>();
        }

        public ISettings GetDefault(IServiceProvider serviceProvider)
        {
            var settings = new WebItemSettings();
            var webItemManager = serviceProvider.GetService<WebItemManager>();
            webItemManager.GetItemsAll().ForEach(w =>
            {
                var opt = new WebItemOption
                {
                    ItemID = w.ID,
                    SortOrder = webItemManager.GetSortOrder(w),
                    Disabled = false,
                };
                settings.SettingsCollection.Add(opt);
            });
            return settings;
        }

        [Serializable]
        public class WebItemOption
        {
            public Guid ItemID { get; set; }

            public int SortOrder { get; set; }

            public bool Disabled { get; set; }
        }
    }
}