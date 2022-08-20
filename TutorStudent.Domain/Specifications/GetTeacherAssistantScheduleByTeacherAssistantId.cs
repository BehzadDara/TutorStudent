using System;
using System.Globalization;
using System.Linq.Expressions;
using TutorStudent.Domain.Interfaces;
using TutorStudent.Domain.Models;

namespace TutorStudent.Domain.Specifications
{
    public class GetTeacherAssistantScheduleByTeacherAssistantId : Specification<TeacherAssistantSchedule>
    {

        private readonly Guid _teacherAssistantId;

        public GetTeacherAssistantScheduleByTeacherAssistantId(Guid teacherAssistantId)
        {
            _teacherAssistantId = teacherAssistantId;
        }

        public override Expression<Func<TeacherAssistantSchedule, bool>> Criteria =>
            myTeacherAssistantSchedule => myTeacherAssistantSchedule.TeacherAssistantId == _teacherAssistantId && myTeacherAssistantSchedule.Remain > 0;


    }
    
    
}