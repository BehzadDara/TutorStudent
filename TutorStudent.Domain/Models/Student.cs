using System;
using System.ComponentModel.DataAnnotations;
using TutorStudent.Domain.Implementations;

namespace TutorStudent.Domain.Models
{
    public class Student : TrackableEntity
    {
        [Required] public User User { get; set; }
        [Required] public Guid UserId { get; set; }
        
        [Required] public string StudentNumber { get; set; }
    }
}