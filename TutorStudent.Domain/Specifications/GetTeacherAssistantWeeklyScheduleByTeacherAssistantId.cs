using System;
using System.Globalization;
using System.Linq.Expressions;
using TutorStudent.Domain.Interfaces;
using TutorStudent.Domain.Models;

namespace TutorStudent.Domain.Specifications
{
    public class GetTeacherAssistantWeeklyScheduleByTeacherAssistantId : Specification<TeacherAssistantWeeklySchedule>
    {

        private readonly Guid _teacherAssistantId;

        public GetTeacherAssistantWeeklyScheduleByTeacherAssistantId(Guid teacherAssistantId)
        {
            _teacherAssistantId = teacherAssistantId;
        }

        public override Expression<Func<TeacherAssistantWeeklySchedule, bool>> Criteria =>
            myTeacherAssistantWeeklySchedule => myTeacherAssistantWeeklySchedule.TeacherAssistantId == _teacherAssistantId;


    }


}