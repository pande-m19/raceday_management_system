namespace raceday_management_system.MVC.Models
{//start of namespace

    public class category_view_model
    {//start of class

        public int Category_id { get; set; }
        public string Category_name { get; set; } = string.Empty;
        public string? Cat_description { get; set; }
        public int? Min_age { get; set; }
        public int? Max_age { get; set; }

    }//end of class

}//end of namespace