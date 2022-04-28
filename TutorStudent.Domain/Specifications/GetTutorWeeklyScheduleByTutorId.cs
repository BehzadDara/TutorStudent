using System;
using System.Globalization;
using System.Linq.Expressions;
using TutorStudent.Domain.Interfaces;
using TutorStudent.Domain.Models;

namespace TutorStudent.Domain.Specifications
{
    public class GetTutorWeeklyScheduleByTutorId : Specification<TutorWeeklySchedule>
    {

        private readonly Guid _tutorId;

        public GetTutorWeeklyScheduleByTutorId(Guid tutorId)
        {
            _tutorId = tutorId;
        }

        public override Expression<Func<TutorWeeklySchedule, bool>> Criteria =>
            myTutorWeeklySchedule => myTutorWeeklySchedule.TutorId == _tutorId;


    }


}