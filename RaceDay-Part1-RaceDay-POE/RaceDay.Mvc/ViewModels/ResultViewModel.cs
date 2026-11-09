using System.ComponentModel.DataAnnotations;

namespace RaceDay.Mvc.ViewModels
{
    public class ResultViewModel
    {
        public int ResultID { get; set; }

        public int EnrolmentID { get; set; }

        public string ParticipantName { get; set; } = string.Empty;

        public string EventName { get; set; } = string.Empty;

        public TimeSpan? FinishTime { get; set; }

        [Range(1, int.MaxValue,
            ErrorMessage = "Finishing position must be greater than zero.")]
        public int? FinishingPosition { get; set; }

        public DateTime RecordedAt { get; set; }
    }
}