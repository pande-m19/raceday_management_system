namespace raceday_management_system.MVC.Models
{//start of namespace
    public class my_result_view_model
    {//start of class
        public int Result_id { get; set; }
        public string Event_name { get; set; } = string.Empty;
        public DateTime Event_date { get; set; }
        public string Finish_time { get; set; } = string.Empty;
        public int Overall_position { get; set; }
        public int Total_finishers { get; set; }
        public string PositionContext => $"{Overall_position} out of {Total_finishers}";
    }//end of class
}//end of namespace