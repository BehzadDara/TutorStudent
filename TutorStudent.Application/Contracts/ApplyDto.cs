using System;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace TutorStudent.Application.Contracts
{
    public class ApplyDto:EntityDto<Guid>
    {
        [Required] public TutorDto Tutor { get; set; }
        [Required] public Guid TutorId { get; set; }
        [Required] public StudentDto Student { get; set; }
        [Required] public Guid StudentId { get; set; }
        [Required] public string Ticket { get; set; }
        [Required] public string TicketValue { get; set; }
        [Required] public string Description { get; set; }
        [Required] public string State { get; set; }
        [Required] public string StateValue { get; set; }
        [CanBeNull] public string TrackingCode { get; set; }
    }
}