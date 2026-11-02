namespace raceday_management_system.MVC.Models
{
    public class dashboard_view_model
    {
        public int TotalEvents { get; set; }
        public int TotalEnrolments { get; set; }
        public List<event_summary> EventsSummary { get; set; } = new List<event_summary>();
    }

    public class event_summary
    {
        public int Event_id { get; set; }
        public string Event_name { get; set; } = string.Empty;
        public DateTime Event_date { get; set; }
        public string Event_status { get; set; } = string.Empty;
        public int Total_enrolments { get; set; }
    }
}