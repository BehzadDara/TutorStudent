using System.ComponentModel;

namespace TutorStudent.Domain.Enums
{
    public enum TicketType
    {
        [Description("فرصت شغلی")]
        Job,
        [Description("پروژه عملی")]
        PracticalProject,
        [Description("پروژه تحقیقاتی")]
        ResearchProject,
        [Description("تدریسیاری")]
        TeacherAssistant,
        [Description("کاراموزی")]
        Internship,
        [Description("پروژه کارشناسی")]
        BachelorProject,
        [Description("پروژه کارشناسی ارشد")]
        MasterProject,
        [Description("سایر")]
        Other
    }
}