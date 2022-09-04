using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TutorStudent.Domain.Implementations;

namespace TutorStudent.Domain.Models
{
    public class FacultyManagementSuggestionTutor : TrackableEntity
    {
        [Required] public FacultyManagementSuggestion FacultyManagementSuggestion { get; set; }
        [Required] public Guid FacultyManagementSuggestionId { get; set; }
        [Required] public Guid TutorId { get; set; }
    }
}
