using System.ComponentModel;

namespace GoogleCalendarMcpServer
{
    public class CalendarEvent
    {
        [Description("The summary/title of the calendar event.")]
        public string? Summary { get; set; }

        [Description("The start date and time of the event.")]
        public DateTime? Start { get; set; }

        [Description("The end date and time of the event.")]
        public DateTime? End { get; set; }

        [Description("The location of the event.")]
        public string? Location { get; set; }

        [Description("The description of the event.")]
        public string? Description { get; set; }
    }
}
