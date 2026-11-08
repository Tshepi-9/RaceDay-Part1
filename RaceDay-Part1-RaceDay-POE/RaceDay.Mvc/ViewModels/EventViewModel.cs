using System.ComponentModel.DataAnnotations;

namespace RaceDay.Mvc.ViewModels
{
    public class EventViewModel
    {
        public int EventID { get; set; }

        [Required]
        public string EventName { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; } = DateTime.Today;

        [Required]
        public string Location { get; set; } = string.Empty;

        [Required]
        [Range(0.01, 1000)]
        public decimal Distance { get; set; }

        [Required]
        public string EventType { get; set; } = "Run";

        public int? RouteID { get; set; }
    }
}