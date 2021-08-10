
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace TutorStudent.Application.Contracts
{
    public class UserCreateDto
    {
        [Required] public string UserName { get; set; }
        [Required] public string Password { get; set; }
            
        [Required] public string FirstName { get; set; }
        [Required] public string LastName { get; set; }
        [Required] public string Gender { get; set; }
            
        [CanBeNull] public string Email { get; set; }
        [CanBeNull] public string PhoneNumber { get; set; }
        [CanBeNull] public string Address { get; set; }
    }
}