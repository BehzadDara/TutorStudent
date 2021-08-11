using System;
using System.ComponentModel.DataAnnotations;
using TutorStudent.Domain.Implementations;

namespace TutorStudent.Domain.Models
{
    public class Meeting : TrackableEntity
    { 
        [Required] public Guid TutorScheduleId { get; set; }
        
        [Required] public Student Student { get; set; }
        [Required] public Guid StudentId { get; set; }

    }
}