using System.ComponentModel.DataAnnotations;

namespace TutorStudent.Application.Contracts
{
    public class TeacherAssistantScheduleUpdateDto
    {
        [Required] public int Capacity { get; set; }
    }
}