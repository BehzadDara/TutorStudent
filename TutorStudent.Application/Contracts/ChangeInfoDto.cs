using JetBrains.Annotations;

namespace TutorStudent.Application.Contracts
{
    public class ChangeInfoDto
    {
        [CanBeNull] public string Email { get; set; }
        [CanBeNull] public string PhoneNumber { get; set; }
        [CanBeNull] public string Address { get; set; }
    }
}