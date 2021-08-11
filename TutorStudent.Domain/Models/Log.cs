using System;
using System.ComponentModel.DataAnnotations;
using TutorStudent.Domain.Implementations;

namespace TutorStudent.Domain.Models
{
    public class Log: TrackableEntity
    { 
        [Required] public Apply Apply { get; set; }
        [Required] public Guid ApplyId { get; set; }
        [Required] public string Before { get; set; }
        [Required] public string After { get; set; }
        [Required] public string Comment { get; set; }
    }
    
}