using System.ComponentModel;

namespace TutorStudent.Domain.Enums
{
    public enum RoleType
    {
        [Description("مدیر")]
        Manager,
        [Description("استاد")]
        Tutor, 
        [Description("دانشجو")]
        Student, 
        [Description("تدریسیار")]
        TeacherAssistant, 
        [Description("دفتر دانشکده")]
        FacultyManagement
    }
}