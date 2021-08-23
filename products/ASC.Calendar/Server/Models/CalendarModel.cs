using ASC.Web.Core.Calendars;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static ASC.Calendar.Controllers.CalendarController;

namespace ASC.Calendar.Models
{
    public class CalendarModel
    {
        [DataMember(Name = "id", Order = 0)]
        public string Id { get; set; }

        [DataMember(Name = "name", Order = 2)]
        public string Name { get; set; }

        [DataMember(Name = "description", Order = 50)]
        public string Description { get; set; }

        [DataMember(Name = "tenant", Order = 200)]
        public int Tenant { get; set; }

        [JsonPropertyName("owner_id")]
        public Guid OwnerId { get; set; }

        [JsonPropertyName("alert_type")]
        public EventAlertType AlertType { get; set; }

        [DataMember(Name = "time_zone", Order = 60)]
        public string TimeZone { get; set; }

        [DataMember(Name = "text_color", Order = 60)]
        public string TextColor { get; set; }

        [DataMember(Name = "background_color", Order = 60)]
        public string BackgroundColor { get; set; }

        [DataMember(Name = "ical_url", Order = 60)]
        public string ICalUrl { get; set; }

        [DataMember(Name = "caldav_guid", Order = 60)]
        public string calDavGuid { get; set; }

        [DataMember(Name = "is_todo", Order = 60)]
        public int IsTodo { get; set; }

        [JsonPropertyName("sharing_options")]
        public List<SharingParam> SharingOptions { get; set; }

        [JsonPropertyName("hide_events")]
        public bool HideEvents { get; set; }

        public static object GetSample()
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
        }

    }
}
