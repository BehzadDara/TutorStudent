using System.ComponentModel.DataAnnotations;

namespace TutorStudent.Application.Contracts
{
    public class TutorScheduleUpdateDto
    {
        [Required] public int Capacity { get; set; }
    }
}