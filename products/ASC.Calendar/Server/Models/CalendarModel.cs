using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace ASC.Calendar.Models
{
    public class CalendarModel
    {

        [DataMember(Name = "id", Order = 0)]
        public int Id { get; set; }

        [DataMember(Name = "owner_id", Order = 1)]
        public string ICalUrl { get; set; }

        [DataMember(Name = "name", Order = 2)]
        public string Name { get; set; }

        [DataMember(Name = "description", Order = 50)]
        public string Description { get; set; }

        [DataMember(Name = "tenant", Order = 200)]
        public int Tenant { get; set; }

        [DataMember(Name = "text_color", Order = 60)]
        public string TextColor { get; set; }

        [DataMember(Name = "background_color", Order = 60)]
        public string BackgroundColor { get; set; }

        [DataMember(Name = "alert_type", Order = 90)]
        public int AlertType { get; set; }

        [DataMember(Name = "time_zone", Order = 60)]
        public string TimeZone { get; set; }

        /*public static object GetSample()
        {
            return new
            {
                canEditTimeZone = false,
                timeZone = TimeZoneWrapper.GetSample(),
                defaultAlert = EventAlertWrapper.GetSample(),
                events = new List<object>() { EventWrapper.GetSample() },
                owner = UserParams.GetSample(),
                objectId = "1",
                title = "Calendar Name",
                description = "Calendar Description",
                backgroundColor = "#000000",
                textColor = "#ffffff",
                isEditable = true,
                permissions = CalendarPermissions.GetSample(),
                isShared = true,
                canAlertModify = true,
                isHidden = false,
                isiCalStream = false,
                isSubscription = false
            };
        }*/

    }
}
