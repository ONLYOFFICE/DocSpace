using ASC.Api.Core;
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
    public class CreateTodoModel
    {
        [DataMember(Name = "ics", Order = 0)]
        public string ics { get; set; }
        [DataMember(Name = "todo_uid", Order = 1)]
        public string todoUid { get; set; }
        [DataMember(Name = "todo_id", Order = 2)]
        public string todoId { get; set; }
        [DataMember(Name = "calendar_id", Order = 2)]
        public string calendarId { get; set; }
        [JsonPropertyName("from_caldav_server")]
        public bool fromCalDavServer { get; set; }

    }
}
