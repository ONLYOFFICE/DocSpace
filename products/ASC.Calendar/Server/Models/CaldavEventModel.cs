using System.Collections.Generic;

namespace ASC.Calendar.Models
{
    public class CaldavEventModel
    {
        public string CalendarId { get; set; }
        public string Uid { get; set; }
        public int Alert { get; set; }
        public List<string> Responsibles { get; set; } 
    }
}
