namespace raceday_management_system.MVC.Models
{//start of namespace

    public class record_result_view_model
    {//start of class

        public int Enrolment_id { get; set; }
        public int Event_id { get; set; }
        public string ParticipantName { get; set; } = string.Empty; // For display
        public string FinishTime { get; set; } = string.Empty;      // e.g. "03:45:22"
        public int OverallPosition { get; set; }
        public int TotalFinishers { get; set; }

    }//end of class

}//end of namespace