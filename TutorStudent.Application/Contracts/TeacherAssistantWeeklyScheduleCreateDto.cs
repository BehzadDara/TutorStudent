using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TutorStudent.Application.Contracts
{
    public class TeacherAssistantWeeklyScheduleCreateDto
    {
        [Required] public string WeekDay { get; set; }
        [Required] public int BeginHour { get; set; }
        [Required] public int EndHour { get; set; }
        [Required] public string Meeting { get; set; }
        [Required] public int Capacity { get; set; }
    }
}
