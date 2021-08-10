using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;
using TutorStudent.Domain.Enums;
using TutorStudent.Domain.Implementations;

namespace TutorStudent.Domain.Models
{
    public class User : TrackableEntity
    {
        [Required] public string UserName { get; set; }
        [Required] public string Password { get; set; }
        
        [Required] public string FirstName { get; set; }
        [Required] public string LastName { get; set; }
        [Required] public GenderType Gender { get; set; }
        [Required] public RoleType Role { get; set; }
        
        [CanBeNull] public string Email { get; set; }
        [CanBeNull] public string PhoneNumber { get; set; }
        [CanBeNull] public string Address { get; set; }
        
    }
}