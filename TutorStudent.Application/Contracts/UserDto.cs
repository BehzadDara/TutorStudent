using System;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace TutorStudent.Application.Contracts
{
    public class UserDto: EntityDto<Guid>
    {
        [Required] public string UserName { get; set; }
        
        [Required] public string FirstName { get; set; }
        [Required] public string LastName { get; set; }
        [Required] public string Gender { get; set; }
        [Required] public string GenderValue { get; set; }
        [Required] public string Role { get; set; }
        [Required] public string RoleValue { get; set; }
        
        [CanBeNull] public string Email { get; set; }
        [CanBeNull] public string PhoneNumber { get; set; }
        [CanBeNull] public string Address { get; set; }
    }
}