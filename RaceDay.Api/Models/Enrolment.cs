using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RaceDay.Api.Models
{
    [Table("Enrolments")]
    public class Enrolment
    {
        [Key]
        public int EnrolmentID { get; set; }

        [Required]
        public int ParticipantID { get; set; }

        [Required]
        public int EventID { get; set; }

        [Required]
        public int CategoryID { get; set; }

        public DateTime EnrolmentDate { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Confirmed";

        public User? Participant { get; set; }

        public Event? Event { get; set; }

        public Category? Category { get; set; }

        public Result? Result { get; set; }
    }
}