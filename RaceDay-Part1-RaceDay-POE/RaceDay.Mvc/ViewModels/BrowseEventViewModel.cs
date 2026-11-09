namespace RaceDay.Mvc.ViewModels
{
    public class BrowseEventViewModel
    {
        public int EventID { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal Distance { get; set; }
        public string EventType { get; set; } = string.Empty;
        public int OrganiserID { get; set; }
        public string OrganiserName { get; set; } = string.Empty;
    }
}