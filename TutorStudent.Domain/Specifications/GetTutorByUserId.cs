using System;
using System.Linq.Expressions;
using TutorStudent.Domain.Interfaces;
using TutorStudent.Domain.Models;

namespace TutorStudent.Domain.Specifications
{
    public class GetTutorByUserId : Specification<Tutor>
    {

        private readonly Guid _userId;

        public GetTutorByUserId(Guid userId)
        {
            _userId = userId;
        }

        public override Expression<Func<Tutor, bool>> Criteria =>
            myTutor => myTutor.UserId == _userId;
    }
}