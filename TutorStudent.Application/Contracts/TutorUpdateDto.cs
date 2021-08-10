using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace TutorStudent.Application.Contracts
{
    public class TutorUpdateDto
    {
        [Required] public ChangeInfoDto ChangeInfo { get; set; }
        [CanBeNull] public string SkypeId { get; set; }
    }
}