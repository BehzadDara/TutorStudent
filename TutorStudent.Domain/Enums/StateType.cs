using System.ComponentModel;

namespace TutorStudent.Domain.Enums
{
    public enum StateType
    {
        [Description("دیده نشده")]
        Unseen,
        [Description("دیده شده")]
        Seen,
        [Description("رد شده")]
        Rejected,
        [Description("تایید شده")]
        Confirmed
    }
}