namespace raceday_management_system.MVC.Models
{//start of namespace
    public class profile_view_model
    {//start of class
        public string Email { get; set; } = string.Empty;
        public string Firstname { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string? Phone_number { get; set; }
        public DateOnly? Date_of_birth { get; set; }
        public string? Profile_picture_url { get; set; }
    }//end of class
}//end of namespace