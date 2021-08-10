using System;
using System.Linq.Expressions;
using TutorStudent.Domain.Interfaces;
using TutorStudent.Domain.Models;

namespace TutorStudent.Domain.Specifications
{
    public class GetStudentByUserId : Specification<Student>
    {

        private readonly Guid _userId;

        public GetStudentByUserId(Guid userId)
        {
            _userId = userId;
        }

        public override Expression<Func<Student, bool>> Criteria =>
            myStudent => myStudent.UserId == _userId;
    }
}