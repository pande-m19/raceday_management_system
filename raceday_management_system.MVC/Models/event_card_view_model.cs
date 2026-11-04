namespace raceday_management_system.MVC.Models
{//start of namespace

    public class event_card_view_model
    {//start of class

        public int Event_id { get; set; }
        public string Event_name { get; set; } = string.Empty;
        public DateTime Event_date { get; set; }
        public string Location { get; set; } = string.Empty;
        public string Event_type { get; set; } = string.Empty;
        public string? Banner_image_url { get; set; }
        public decimal Entry_fee { get; set; }

    }//end of class

}//end of namespace