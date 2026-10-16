using System;
using System.ComponentModel.DataAnnotations;

namespace RaceDay.Contracts
{
    // Used when a user registers for RaceDay.
    public class RegisterRequest
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;

        [Required]
        [RegularExpression("^(Organiser|Participant)$")]
        public string Role { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Phone { get; set; }
    }

    // Used when a user logs in.
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }

    // Returned after successful authentication.
    public class AuthResponse
    {
        public int UserID { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
    }

    // Information displayed when a user views their profile.
    public class ProfileDto
    {
        public int UserID { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string? Phone { get; set; }
    }

    // Information accepted when updating a profile.
    public class UpdateProfileRequest
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Phone { get; set; }
    }

    // Information required to create an event.
    public class CreateEventRequest
    {
        [Required]
        [StringLength(100)]
        public string EventName { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        [StringLength(150)]
        public string Location { get; set; } = string.Empty;

        [Range(0.01, 9999)]
        public decimal Distance { get; set; }

        [Required]
        [RegularExpression("^(Run|Walk|Cycle)$")]
        public string EventType { get; set; } = string.Empty;

        public int? RouteID { get; set; }
    }

    // Information accepted when updating an event.
    public class UpdateEventRequest
    {
        [Required]
        [StringLength(100)]
        public string EventName { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        [StringLength(150)]
        public string Location { get; set; } = string.Empty;

        [Range(0.01, 9999)]
        public decimal Distance { get; set; }

        [Required]
        [RegularExpression("^(Run|Walk|Cycle)$")]
        public string EventType { get; set; } = string.Empty;

        public int? RouteID { get; set; }
    }

    // Information returned when an event is viewed.
    public class EventDto
    {
        public int EventID { get; set; }

        public int OrganiserID { get; set; }

        public int? RouteID { get; set; }

        public string EventName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime EventDate { get; set; }

        public string Location { get; set; } = string.Empty;

        public decimal Distance { get; set; }

        public string EventType { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
    }

    // Information required to create an event category.
    public class CreateCategoryRequest
    {
        [Required]
        [StringLength(80)]
        public string CategoryName { get; set; } = string.Empty;

        [Range(0, 120)]
        public int? MinimumAge { get; set; }

        [Range(0, 120)]
        public int? MaximumAge { get; set; }

        [Range(0.01, 9999)]
        public decimal? CategoryDistance { get; set; }
    }

    // Information returned when a category is viewed.
    public class CategoryDto
    {
        public int CategoryID { get; set; }

        public int EventID { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public int? MinimumAge { get; set; }

        public int? MaximumAge { get; set; }

        public decimal? CategoryDistance { get; set; }
    }

    // Information required when a participant enrols in an event.
    public class CreateEnrolmentRequest
    {
        [Range(1, int.MaxValue)]
        public int CategoryID { get; set; }
    }

    // Used if you implement enrolment status updates.
    public class UpdateEnrolmentStatusRequest
    {
        [Required]
        [RegularExpression("^(Pending|Confirmed|Cancelled)$")]
        public string Status { get; set; } = string.Empty;
    }

    // Information returned when an enrolment is viewed.
    public class EnrolmentDto
    {
        public int EnrolmentID { get; set; }

        public int ParticipantID { get; set; }

        public int EventID { get; set; }

        public int CategoryID { get; set; }

        public DateTime EnrolmentDate { get; set; }

        public string Status { get; set; } = string.Empty;
    }

    // Information required when an organiser records a result.
    public class CreateResultRequest
    {
        public TimeSpan? FinishTime { get; set; }

        [Range(1, int.MaxValue)]
        public int? FinishingPosition { get; set; }
    }

    // Information returned when a result is viewed.
    public class ResultDto
    {
        public int ResultID { get; set; }

        public int EnrolmentID { get; set; }

        public TimeSpan? FinishTime { get; set; }

        public int? FinishingPosition { get; set; }

        public DateTime RecordedAt { get; set; }
    }

    // Optional response class for file uploads in a later part.
    public class UploadResponse
    {
        public string FileName { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;
    }
}