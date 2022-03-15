namespace ASC.Web.Studio.Core.TFA
{
    [Serializable]
    public class TfaAppAuthSettings : ISettings
    {
        public Guid ID
        {
            get { return new Guid("{822CA059-AA8F-4588-BEE3-6CD2AA920CDB}"); }
        }

        public ISettings GetDefault(IServiceProvider serviceProvider)
        {
            return new TfaAppAuthSettings { EnableSetting = false, };
        }

        [JsonPropertyName("Enable")]
        public bool EnableSetting { get; set; }


        public static bool IsVisibleSettings
        {
            get { return SetupInfo.IsVisibleSettings<TfaAppAuthSettings>(); }
        }
    }
}