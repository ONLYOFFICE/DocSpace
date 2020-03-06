using ASC.Web.Core.Calendars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using static ASC.Calendar.Controllers.CalendarController;

namespace ASC.Calendar.Models
{
    public class EventModel
    {
        //int calendarId, string ics, EventAlertType alertType, List<SharingParam> sharingOptions, string eventUid = null 

        [DataMember(Name = "calendar_id", Order = 0)]
        public int CalendarId { get; set; }

        [DataMember(Name = "ics", Order = 2)]
        public string Ics { get; set; }

        [DataMember(Name = "alert_type", Order = 50)]
        public EventAlertType AlertType { get; set; }

        [DataMember(Name = "sharing_options", Order = 200)]
        public List<SharingParam> SharingOptions { get; set; }

        [DataMember(Name = "event_uid", Order = 1)]
        public string eventUid { get; set; }


        /*public static object GetSample()
        {
            return new
            {
                
            };
        }*/

    }
}
