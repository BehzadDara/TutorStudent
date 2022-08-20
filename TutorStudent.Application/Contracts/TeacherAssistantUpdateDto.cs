using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TutorStudent.Application.Contracts
{
    public class TeacherAssistantUpdateDto
    {
        [Required] public ChangeInfoDto ChangeInfo { get; set; }
        [CanBeNull] public string SkypeId { get; set; }
    }
}
