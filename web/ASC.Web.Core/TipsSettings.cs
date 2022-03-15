namespace ASC.Web.Studio.Core
{
    [Serializable]
    public class TipsSettings : ISettings
    {
        [DataMember(Name = "Show")]
        public bool Show { get; set; }

        public Guid ID
        {
            get { return new Guid("{27909339-B4D4-466F-8F40-A64C9D2FC041}"); }
        }

        public ISettings GetDefault(IServiceProvider serviceProvider)
        {
            return new TipsSettings { Show = true };
        }
    }
}