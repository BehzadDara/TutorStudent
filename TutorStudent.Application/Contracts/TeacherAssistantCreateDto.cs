using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TutorStudent.Application.Contracts
{
    public class TeacherAssistantCreateDto
    {
        [Required] public UserCreateDto User { get; set; }
        [CanBeNull] public string SkypeId { get; set; }
    }
}
