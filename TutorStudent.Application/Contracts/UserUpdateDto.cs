using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace TutorStudent.Application.Contracts
{
    public class UserUpdateDto
    {
        [Required] public string UserName { get; set; }
        [CanBeNull] public string Password { get; set; }
        
        [Required] public string FirstName { get; set; }
        [Required] public string LastName { get; set; }
        [Required] public string Gender { get; set; }
        [Required] public string Role { get; set; }
    }
}