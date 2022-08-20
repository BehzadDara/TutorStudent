using System;
using System.ComponentModel.DataAnnotations;
using TutorStudent.Domain.Enums;
using TutorStudent.Domain.Implementations;

namespace TutorStudent.Domain.Models
{
    public class TeacherAssistantSchedule : TrackableEntity
    {
        [Required] public TeacherAssistant TeacherAssistant { get; set; }
        [Required] public Guid TeacherAssistantId { get; set; }
        [Required] public string Date { get; set; }
        [Required] public int BeginHour { get; set; }
        [Required] public int EndHour { get; set; }
        [Required] public MeetingType Meeting { get; set; }
        [Required] public int Capacity { get; set; }
        [Required] public int Remain { get; set; }
    }
}