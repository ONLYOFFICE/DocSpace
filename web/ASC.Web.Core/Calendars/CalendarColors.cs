namespace ASC.Web.Core.Calendars
{
    public class CalendarColors
    {
        public string BackgroudColor { get; set; }
        public string TextColor { get; set; }
        public static List<CalendarColors> BaseColors
        {
            get
            {
                return new List<CalendarColors>(){
                    new CalendarColors(){ BackgroudColor = "#e34603", TextColor="#ffffff"},
                    new CalendarColors(){ BackgroudColor = "#f88e14", TextColor="#000000"},
                    new CalendarColors(){ BackgroudColor = "#ffb403", TextColor="#000000"},
                    new CalendarColors(){ BackgroudColor = "#9fbb4c", TextColor="#000000"},
                    new CalendarColors(){ BackgroudColor = "#288e31", TextColor="#ffffff"},
                    new CalendarColors(){ BackgroudColor = "#4cbb78", TextColor="#ffffff"},
                    new CalendarColors(){ BackgroudColor = "#0797ba", TextColor="#ffffff"},
                    new CalendarColors(){ BackgroudColor = "#1d5f99", TextColor="#ffffff"},
                    new CalendarColors(){ BackgroudColor = "#4c76bb", TextColor="#ffffff"},
                    new CalendarColors(){ BackgroudColor = "#3552d2", TextColor="#ffffff"},
                    new CalendarColors(){ BackgroudColor = "#473388", TextColor="#ffffff"},
                    new CalendarColors(){ BackgroudColor = "#884cbb", TextColor="#ffffff"},
                    new CalendarColors(){ BackgroudColor = "#cb59ba", TextColor="#000000"},
                    new CalendarColors(){ BackgroudColor = "#ca3083", TextColor="#ffffff"},
                    new CalendarColors(){ BackgroudColor = "#e24e78", TextColor="#000000"},
                    new CalendarColors(){ BackgroudColor = "#bf0036", TextColor="#ffffff"}
                };
            }
        }

    }
}
