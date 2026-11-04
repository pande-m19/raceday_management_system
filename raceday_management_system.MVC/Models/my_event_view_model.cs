namespace raceday_management_system.MVC.Models
{//start of namespace
    public class my_event_view_model
    {//start of class
        public int Enrolment_id { get; set; }
        public int Event_id { get; set; }
        public string Event_name { get; set; } = string.Empty;
        public DateTime Event_date { get; set; }
        public string Category_name { get; set; } = string.Empty;
        public string Enr_status { get; set; } = string.Empty;
        public string? Payment_status { get; set; }
    }//end of class
}//end of namespace