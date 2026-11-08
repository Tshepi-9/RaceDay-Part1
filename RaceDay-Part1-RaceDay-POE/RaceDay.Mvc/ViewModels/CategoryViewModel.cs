using System.ComponentModel.DataAnnotations;

namespace RaceDay.Mvc.ViewModels
{
    public class CategoryViewModel
    {
        public int CategoryID { get; set; }

        public int EventID { get; set; }

        [Required(ErrorMessage = "Category name is required.")]
        public string CategoryName { get; set; } = string.Empty;

        [Range(0, 120, ErrorMessage = "Minimum age cannot be negative.")]
        public int? MinimumAge { get; set; }

        [Range(0, 120, ErrorMessage = "Maximum age cannot be negative.")]
        public int? MaximumAge { get; set; }

        [Range(0.01, 1000, ErrorMessage = "Distance must be greater than zero.")]
        public decimal? CategoryDistance { get; set; }
    }
}