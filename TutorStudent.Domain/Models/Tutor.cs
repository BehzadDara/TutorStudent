using System;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using TutorStudent.Domain.Implementations;

namespace TutorStudent.Domain.Models
{
    public class Tutor : TrackableEntity
    {
        [Required] public User User { get; set; }
        [Required] public Guid UserId { get; set; }
        
        [CanBeNull] public string SkypeId { get; set; }
    }
    
}