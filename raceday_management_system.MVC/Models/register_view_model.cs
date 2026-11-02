using System.ComponentModel.DataAnnotations;

namespace raceday_management_system.MVC.Models
{//start of namespace
    public class register_view_model
    {//start of class
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Firstname { get; set; } = string.Empty;

        [Required]
        public string Surname { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "Participant"; // Default role

        public DateOnly? DateOfBirth { get; set; }

        public string? PhoneNumber { get; set; }
    }//end of class
}//end of namespace