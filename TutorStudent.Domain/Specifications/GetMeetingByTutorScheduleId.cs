using System;
using System.Linq.Expressions;
using TutorStudent.Domain.Interfaces;
using TutorStudent.Domain.Models;

namespace TutorStudent.Domain.Specifications
{
    public class GetMeetingByTutorScheduleId : Specification<Meeting>
    {

        private readonly Guid _tutorScheduleId;

        public GetMeetingByTutorScheduleId(Guid tutorScheduleId)
        {
            _tutorScheduleId = tutorScheduleId;
        }

        public override Expression<Func<Meeting, bool>> Criteria =>
            myMeeting => myMeeting.TutorScheduleId == _tutorScheduleId && !myMeeting.IsDeleted;
    }
}