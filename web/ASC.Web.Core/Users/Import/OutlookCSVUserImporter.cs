namespace ASC.Web.Core.Users.Import
{
    public class OutlookCSVUserImporter : TextFileUserImporter
    {
        public OutlookCSVUserImporter(Stream stream)
            : base(stream)
        {
            HasHeader = true;
            NameMapping = new Dictionary<string, string>()
                              {
                                  {"First Name", "FirstName"},
                                  {"Last Name", "LastName"},
                                  {"Middle Name", ""},
                                  {"Name", ""},
                                  {"Nickname", ""},
                                  {"E-mail Address", "Email"},
                                  {"Home Street", "PrimaryAddress"},
                                  {"Home City", ""},
                                  {"Home Postal Code", "PostalCode"},
                                  {"Home State", ""},
                                  {"Home Country/Region", ""},
                                  {"Home Phone", "PhoneHome"},
                                  {"Home Fax", ""},
                                  {"Mobile Phone", "PhoneMobile"},
                                  {"Personal Web Page", ""},
                                  {"Business Street", ""},
                                  {"Business City", ""},
                                  {"Business Postal Code", ""},
                                  {"Business State", ""},
                                  {"Business Country/Region", ""},
                                  {"Business Web Page", ""},
                                  {"Business Phone", "PhoneOffice"},
                                  {"Business Fax", ""},
                                  {"Pager", ""},
                                  {"Job Title", "Title"},
                                  {"Department", "Department"},
                                  {"Office Location", ""},
                                  {"Notes", "Notes"}
                              };
        }
    }
}