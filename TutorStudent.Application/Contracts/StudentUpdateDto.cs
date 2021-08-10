using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace TutorStudent.Application.Contracts
{
    public class StudentUpdateDto
    {
        [Required] public ChangeInfoDto ChangeInfo { get; set; }
        [CanBeNull] public string StudentNumber { get; set; }
    }
}