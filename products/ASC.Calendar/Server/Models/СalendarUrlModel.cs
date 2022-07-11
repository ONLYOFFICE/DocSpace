using System.Runtime.Serialization;

namespace ASC.Calendar.Models
{
    public class СalendarUrlModel
    {

        [DataMember(Name = "ical_url", Order = 1)]
        public string ICalUrl { get; set; }

        [DataMember(Name = "name", Order = 0)]
        public string Name { get; set; }

        [DataMember(Name = "text_color", Order = 2)]
        public string TextColor { get; set; }

        [DataMember(Name = "background_color", Order = 50)]
        public string BackgroundColor { get; set; }

    }
}
