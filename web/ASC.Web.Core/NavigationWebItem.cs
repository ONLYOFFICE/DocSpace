namespace ASC.Web.Core
{
    [WebZoneAttribute(WebZoneType.Nowhere)]
    public class NavigationWebItem : IWebItem
    {
        public virtual Guid ID { get; set; }

        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual string StartURL { get; set; }

        public virtual string HelpURL { get; set; }

        public virtual string ProductClassName { get; set; }

        public bool Visible { get { return true; } }

        public virtual WebItemContext Context { get; set; }

        public string ApiURL
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool Equals(object obj)
        {
            return obj is IWebItem m && ID == m.ID;
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
    }
}
