using System.ComponentModel.DataAnnotations;

namespace raceday_management_system.MVC.Models
{//start of namespace
    public class login_view_model
    {//start of class
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }//end of class
}//end of namespace