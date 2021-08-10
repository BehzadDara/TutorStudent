using System;
using System.ComponentModel.DataAnnotations;

namespace TutorStudent.Application.Contracts
{
    public class StudentDto :EntityDto<Guid>
    {
        [Required] public UserDto User { get; set; }
        [Required] public string StudentNumber { get; set; }
    }
}