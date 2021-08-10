using System.ComponentModel.DataAnnotations;

namespace TutorStudent.Application.Contracts
{
    public class ChangePasswordDto
    {
        [Required] public string OldPassword { get; set; }
        [Required] public string NewPassword { get; set; }
    }
}