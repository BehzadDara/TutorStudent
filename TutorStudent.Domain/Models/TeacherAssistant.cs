using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TutorStudent.Domain.Implementations;

namespace TutorStudent.Domain.Models
{
    public class TeacherAssistant : TrackableEntity
    {
        [Required] public User User { get; set; }
        [Required] public Guid UserId { get; set; }

        [Required] public Guid TutorId { get; set; }
        [CanBeNull] public string SkypeId { get; set; }
    }
}
