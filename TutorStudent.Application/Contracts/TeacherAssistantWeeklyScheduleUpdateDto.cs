using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TutorStudent.Application.Contracts
{
    public class TeacherAssistantWeeklyScheduleUpdateDto
    {
        [Required] public int Capacity { get; set; }
    }
}
