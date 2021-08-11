using System.ComponentModel.DataAnnotations;

namespace TutorStudent.Application.Contracts
{
    public class TutorScheduleCreateDto
    {
        [Required] public string Date { get; set; }
        [Required] public string BeginHour { get; set; }
        [Required] public string EndHour { get; set; }
        [Required] public string Meeting { get; set; }
        [Required] public int Capacity { get; set; }
        [Required] public bool AutoConfirm { get; set; }
    }
}