using System;
using System.ComponentModel.DataAnnotations;
using TutorStudent.Domain.Enums;
using TutorStudent.Domain.Implementations;

namespace TutorStudent.Domain.Models
{
    public class Advertisement: TrackableEntity
    {
        [Required] public Tutor Tutor { get; set; }
        [Required] public Guid TutorId { get; set; }
        [Required] public TicketType Ticket { get; set; }
        [Required] public string Duration { get; set; }
        [Required] public string Description { get; set; }
    }
}