namespace ASC.Web.Api.Models
{
    public class SchemaModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Current { get; set; }
        public SchemaItemModel Items { get; set; }
    }

    public class SchemaItemModel
    {
        public string Id { get; set; }
        public string UserCaption { get; set; }
        public string UsersCaption { get; set; }
        public string GroupCaption { get; set; }
        public string GroupsCaption { get; set; }
        public string UserPostCaption { get; set; }
        public string RegDateCaption { get; set; }
        public string GroupHeadCaption { get; set; }
        public string GuestCaption { get; set; }
        public string GuestsCaption { get; set; }
    }
}
