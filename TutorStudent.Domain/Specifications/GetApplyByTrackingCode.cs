using System;
using System.Linq.Expressions;
using TutorStudent.Domain.Interfaces;
using TutorStudent.Domain.Models;

namespace TutorStudent.Domain.Specifications
{
    public class GetApplyByTrackingCode : Specification<Apply>
    {

        private readonly string _trackingCode;

        public GetApplyByTrackingCode(string trackingCode)
        {
            _trackingCode = trackingCode;
        }

        public override Expression<Func<Apply, bool>> Criteria =>
            myApply => myApply.TrackingCode == _trackingCode;
    }
}