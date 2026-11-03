namespace raceday_management_system.MVC.Models
{//start of namespace

    public class enrolment_view_model
    {//start of class

        public int Enrolment_id { get; set; }
        public int Participant_id { get; set; }
        public int Category_id { get; set; }
        public DateTime Enrolment_date { get; set; }
        public string Enr_status { get; set; } = string.Empty;
        public string? Payment_status { get; set; }

    }//end of class

}//end of namespace