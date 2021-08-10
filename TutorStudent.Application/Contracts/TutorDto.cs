using System;
using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace TutorStudent.Application.Contracts
{
    public class TutorDto :EntityDto<Guid>
    {
        [Required] public UserDto User { get; set; }
        [CanBeNull] public string SkypeId { get; set; }
    }
}