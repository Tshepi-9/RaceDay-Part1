namespace RaceDay.Mvc.ViewModels
{
    public class EnrolmentViewModel
    {
        public int EnrolmentID { get; set; }

        public int EventID { get; set; }

        public int ParticipantID { get; set; }

        public string ParticipantName { get; set; } = string.Empty;

        public int CategoryID { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public DateTime EnrolmentDate { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}