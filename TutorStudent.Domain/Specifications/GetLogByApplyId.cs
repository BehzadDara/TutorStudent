using System;
using System.Globalization;
using System.Linq.Expressions;
using TutorStudent.Domain.Interfaces;
using TutorStudent.Domain.Models;

namespace TutorStudent.Domain.Specifications
{
    public class GetLogByApplyId : Specification<Log>
    {

        private readonly Guid _applyId;

        public GetLogByApplyId(Guid applyId)
        {
            _applyId = applyId;
        }

        public override Expression<Func<Log, bool>> Criteria =>
            myLog => myLog.ApplyId == _applyId;
    }
    
    
}