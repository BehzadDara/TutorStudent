using System;
using System.Globalization;
using System.Linq.Expressions;
using TutorStudent.Domain.Interfaces;
using TutorStudent.Domain.Models;

namespace TutorStudent.Domain.Specifications
{
    public class GetApplyByStudentId : Specification<Apply>
    {

        private readonly Guid _studentId;

        public GetApplyByStudentId(Guid studentId)
        {
            _studentId = studentId;
        }

        public override Expression<Func<Apply, bool>> Criteria =>
            myApply => myApply.StudentId == _studentId && !myApply.IsDeleted;
    }
    
    
}