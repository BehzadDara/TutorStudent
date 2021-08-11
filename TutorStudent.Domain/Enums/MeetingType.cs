using System.ComponentModel;

namespace TutorStudent.Domain.Enums
{
    public enum MeetingType
    {
        [Description("حضوری")]
        Visit, 
        [Description("اسکایپ")]
        Skype,
        [Description("حضوری و اسکایپ")]
        VisitAndSkype
    }
}