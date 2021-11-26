namespace ASC.CRM.ApiModels
{
    public class CreateOrUpdateContactStatusRequestDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string Color { get; set; }
        public int SortOrder { get; set; }

    }
}
