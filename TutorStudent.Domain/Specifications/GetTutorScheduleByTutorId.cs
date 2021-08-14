using System;
using System.Globalization;
using System.Linq.Expressions;
using TutorStudent.Domain.Interfaces;
using TutorStudent.Domain.Models;

namespace TutorStudent.Domain.Specifications
{
    public class GetTutorScheduleByTutorId : Specification<TutorSchedule>
    {

        private readonly Guid _tutorId;

        public GetTutorScheduleByTutorId(Guid tutorId)
        {
            _tutorId = tutorId;
        }

        public override Expression<Func<TutorSchedule, bool>> Criteria =>
            myTutorSchedule => myTutorSchedule.TutorId == _tutorId && myTutorSchedule.Remain > 0;


    }
    
    
}