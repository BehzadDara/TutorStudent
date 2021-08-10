using System.ComponentModel.DataAnnotations;

namespace TutorStudent.Application.Contracts
{
    public class ManagerDto
    {
        [Required] public UserDto User { get; set; }
    }
}