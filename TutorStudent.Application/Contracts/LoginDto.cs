using System;
using System.ComponentModel.DataAnnotations;

namespace TutorStudent.Application.Contracts
{
    public class LoginDto
    {
        [Required] public Guid Id { get; set; }
    }
}