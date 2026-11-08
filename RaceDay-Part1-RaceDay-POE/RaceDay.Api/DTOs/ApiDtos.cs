using System;

namespace RaceDay.Api.DTOs
{
    public class UpdateProfileRequest
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
    }

public class CreateEventRequest
    {
        public string EventName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal Distance { get; set; }
        public string EventType { get; set; } = string.Empty;
        public int? RouteID { get; set; }
    }

    public class UpdateEventRequest
    {
        public string EventName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public decimal Distance { get; set; }
        public string EventType { get; set; } = string.Empty;
        public int? RouteID { get; set; }
    }

    public class CreateCategoryRequest
    {
        public string CategoryName { get; set; } = string.Empty;
        public int? MinimumAge { get; set; }
        public int? MaximumAge { get; set; }
        public decimal? CategoryDistance { get; set; }
    }

    public class CreateEnrolmentRequest
    {
        public int EventID { get; set; }
        public int CategoryID { get; set; }
    }

    public class UpdateEnrolmentStatusRequest
    {
        public string Status { get; set; } = string.Empty;
    }

    public class CreateResultRequest
    {
        public int EnrolmentID { get; set; }
        public TimeSpan? FinishTime { get; set; }
        public int? FinishingPosition { get; set; }
    }

}
