using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace TutorStudent.Application.Contracts
{
    public class TutorCreateDto
    {
        [Required] public UserCreateDto User { get; set; }
        [CanBeNull] public string SkypeId { get; set; }
    }
}