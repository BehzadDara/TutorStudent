using System;
using System.ComponentModel.DataAnnotations;

namespace TutorStudent.Application.Contracts
{
    public class ApplyCreateDto
    {
        [Required] public Guid TutorId { get; set; }
        [Required] public string Ticket { get; set; }
        [Required] public string Description { get; set; }
    }
}