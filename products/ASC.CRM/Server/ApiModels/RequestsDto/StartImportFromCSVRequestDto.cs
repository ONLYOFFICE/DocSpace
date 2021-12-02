namespace ASC.CRM.ApiModels
{
    public class StartImportFromCSVRequestDto
    {
        public string CsvFileURI { get; set; }
        public string JsonSettings { get; set; }
    }
}
