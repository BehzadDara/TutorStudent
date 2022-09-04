using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TutorStudent.Domain.Implementations;

namespace TutorStudent.Domain.Models
{
    public class FacultyManagementSuggestion : TrackableEntity
    {
        [Required] public string Date { get; set; }
        [Required] public int BeginHour { get; set; }
        [Required] public int EndHour { get; set; }
        [CanBeNull] public string Condition { get; set; }
    }
}
