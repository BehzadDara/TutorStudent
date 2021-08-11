using System;
using System.ComponentModel.DataAnnotations;
using TutorStudent.Domain.Enums;
using TutorStudent.Domain.Implementations;

namespace TutorStudent.Domain.Models
{
    public class TutorSchedule : TrackableEntity
    {
        [Required] public Tutor Tutor { get; set; }
        [Required] public Guid TutorId { get; set; }
        [Required] public string Date { get; set; }
        [Required] public string BeginHour { get; set; }
        [Required] public string EndHour { get; set; }
        [Required] public MeetingType Meeting { get; set; }
        [Required] public int Capacity { get; set; }
        [Required] public int Remain { get; set; }
        [Required] public bool AutoConfirm { get; set; }
    }
}