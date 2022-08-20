using System;
using System.Linq.Expressions;
using TutorStudent.Domain.Interfaces;
using TutorStudent.Domain.Models;

namespace TutorStudent.Domain.Specifications
{
    public class GetTeacherAssistantByUserId : Specification<TeacherAssistant>
    {

        private readonly Guid _userId;

        public GetTeacherAssistantByUserId(Guid userId)
        {
            _userId = userId;
        }

        public override Expression<Func<TeacherAssistant, bool>> Criteria =>
            myTeacherAssistant => myTeacherAssistant.UserId == _userId;
    }
}