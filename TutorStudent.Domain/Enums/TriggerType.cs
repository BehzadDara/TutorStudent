using System.ComponentModel;

namespace TutorStudent.Domain.Enums
{
    public enum TriggerType
    {
        [Description("باز کردن")]
        Open,
        [Description("رد")]
        Reject,
        [Description("تایید")]
        Confirm,
    }
}