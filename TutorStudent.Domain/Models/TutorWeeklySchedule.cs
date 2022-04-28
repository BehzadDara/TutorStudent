using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TutorStudent.Domain.Enums;
using TutorStudent.Domain.Implementations;

namespace TutorStudent.Domain.Models
{
    public class TutorWeeklySchedule : TrackableEntity
    {
        [Required] public Tutor Tutor { get; set; }
        [Required] public Guid TutorId { get; set; }
        [Required] public WeekDayType WeekDay { get; set; }
        [Required] public int BeginHour { get; set; }
        [Required] public int EndHour { get; set; }
        [Required] public MeetingType Meeting { get; set; }
        [Required] public int Capacity { get; set; }
    }
}
