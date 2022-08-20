using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TutorStudent.Application.Contracts
{
    public class TeacherAssistantDto : EntityDto<Guid>
    {
        [Required] public UserDto User { get; set; }
        [Required] public Guid UserId { get; set; }
        [CanBeNull] public string SkypeId { get; set; }
    }
}
