using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TutorStudent.Application.Contracts
{
    public class TeacherAssistantMeetingDto : EntityDto<Guid>
    {
        [Required] public StudentDto Student { get; set; }
    }
}
