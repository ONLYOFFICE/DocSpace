namespace ASC.CRM.ApiModels
{
    public class CreateOrUpdateCustomFieldValueRequestDto
    {
        public string Label { get; set; }
        public int FieldType { get; set; }
        public int Position { get; set; }
        public string Mask { get; set; }
    }
}
