using System;
using System.Globalization;
using System.Linq.Expressions;
using TutorStudent.Domain.Interfaces;
using TutorStudent.Domain.Models;

namespace TutorStudent.Domain.Specifications
{
    public class GetApplyByTutorId : Specification<Apply>
    {

        private readonly Guid _tutorId;

        public GetApplyByTutorId(Guid tutorId)
        {
            _tutorId = tutorId;
        }

        public override Expression<Func<Apply, bool>> Criteria =>
            myApply => myApply.TutorId == _tutorId && !myApply.IsDeleted;
    }
    
    
}