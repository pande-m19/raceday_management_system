using System.ComponentModel.DataAnnotations;

namespace raceday_management_system.MVC.Models
{//start of namespace
    public class create_event_view_model
    {//start of class

        [Required]
        public string EventName { get; set; } = string.Empty;

        [Required]
        public string EventDescription { get; set; } = string.Empty;

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        public string Location { get; set; } = string.Empty;

        [Required]
        [Range(0.1, 1000, ErrorMessage = "Distance must be greater than 0")]
        public decimal Distance { get; set; }

        [Required]
        public string EventType { get; set; } = "Run"; // Run, Walk, Cycle

        public int? MaxParticipants { get; set; }

        public decimal? EntryFee { get; set; }

        public IFormFile? BannerFile { get; set; }

    }//end of class

}//end of namespace