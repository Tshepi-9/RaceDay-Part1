using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Api.Models
{
    [Table("Events")]
    public class Event
    {
        [Key]
        public int EventID { get; set; }

        [Required]
        public int OrganiserID { get; set; }

        public int? RouteID { get; set; }

        [Required]
        [MaxLength(100)]
        public string EventName { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [Column(TypeName = "date")]
        public DateTime EventDate { get; set; }

        [Required]
        [MaxLength(150)]
        public string Location { get; set; } = string.Empty;

        [Column(TypeName = "decimal(6,2)")]
        public decimal Distance { get; set; }

        [Required]
        [MaxLength(10)]
        public string EventType { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public User? Organiser { get; set; }

        public Route? Route { get; set; }

        public ICollection<Category> Categories { get; set; }
            = new List<Category>();

        public ICollection<Enrolment> Enrolments { get; set; }
            = new List<Enrolment>();
    }
}