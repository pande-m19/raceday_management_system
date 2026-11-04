namespace raceday_management_system.MVC.Models
{//start of namespace
    public class event_details_view_model
    {//start of class
        public int Event_id { get; set; }
        public string Event_name { get; set; } = string.Empty;
        public string Event_description { get; set; } = string.Empty;
        public DateTime Event_date { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal Distance { get; set; }
        public string Event_type { get; set; } = string.Empty;
        public string? Banner_image_url { get; set; }
        public decimal Entry_fee { get; set; }
        public string Event_status { get; set; } = string.Empty;

        //method to check if the user is already enrolled
        public bool IsEnrolled { get; set; }

        //list of categories available for this event
        public List<category_view_model> Categories { get; set; } = new List<category_view_model>();

    }//end of class

}//end of namespace