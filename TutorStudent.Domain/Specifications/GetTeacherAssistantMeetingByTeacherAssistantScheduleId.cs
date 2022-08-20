using System;
using System.Linq.Expressions;
using TutorStudent.Domain.Interfaces;
using TutorStudent.Domain.Models;

namespace TutorStudent.Domain.Specifications
{
    public class GetTeacherAssistantMeetingByTeacherAssistantScheduleId : Specification<TeacherAssistantMeeting>
    {

        private readonly Guid _teacherAssistantScheduleId;

        public GetTeacherAssistantMeetingByTeacherAssistantScheduleId(Guid teacherAssistantScheduleId)
        {
            _teacherAssistantScheduleId = teacherAssistantScheduleId;
        }

        public override Expression<Func<TeacherAssistantMeeting, bool>> Criteria =>
            myTeacherAssistantMeeting => myTeacherAssistantMeeting.TeacherAssistantScheduleId == _teacherAssistantScheduleId;
    }
}