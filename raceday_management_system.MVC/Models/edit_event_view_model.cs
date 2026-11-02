using System.ComponentModel.DataAnnotations;

namespace raceday_management_system.MVC.Models
{
    public class edit_event_view_model
    {
        public int Event_id { get; set; }

        [Required]
        public string Event_name { get; set; } = string.Empty;

        [Required]
        public string Event_description { get; set; } = string.Empty;

        [Required]
        public DateTime Event_date { get; set; }

        [Required]
        public string Location { get; set; } = string.Empty;

        [Required]
        public decimal Distance { get; set; }

        [Required]
        public string Event_type { get; set; } = "Run";

        [Required]
        public string Event_status { get; set; } = "Upcoming";

        public int? Max_participants { get; set; }

        public decimal? Entry_fee { get; set; }
    }
}