using System.ComponentModel.DataAnnotations;

namespace TutorStudent.Application.Contracts
{
    public class StudentCreateDto
    {
        [Required] public UserCreateDto User { get; set; }
        [Required] public string StudentNumber { get; set; }
    }
}