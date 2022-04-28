using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TutorStudent.Application.Contracts
{
    public class TutorWeeklyScheduleUpdateDto
    {
        [Required] public int Capacity { get; set; }
    }
}
