using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Api.Models
{
    [Table("Routes")]
    public class Route
    {
        [Key]
        public int RouteID { get; set; }

        [Required]
        [MaxLength(100)]
        public string RouteName { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string StartPoint { get; set; } = string.Empty;

        [Required]
        [MaxLength(150)]
        public string EndPoint { get; set; } = string.Empty;

        [Column(TypeName = "decimal(8,2)")]
        public decimal? ElevationGain { get; set; }

        [MaxLength(500)]
        public string? RouteMapURL { get; set; }

        public ICollection<Event> Events { get; set; }
            = new List<Event>();
    }
}