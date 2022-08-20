using System.ComponentModel.DataAnnotations;

namespace TutorStudent.Application.Contracts
{
    public class TeacherAssistantScheduleCreateDto
    {
        [Required] public string Date { get; set; }
        [Required] public int BeginHour { get; set; }
        [Required] public int EndHour { get; set; }
        [Required] public string Meeting { get; set; }
        [Required] public int Capacity { get; set; }
    }
}