using System;
using System.Globalization;
using System.Linq.Expressions;
using TutorStudent.Domain.Interfaces;
using TutorStudent.Domain.Models;

namespace TutorStudent.Domain.Specifications
{
    public class GetAdvertisementByTutorId : Specification<Advertisement>
    {

        private readonly Guid _tutorId;

        public GetAdvertisementByTutorId(Guid tutorId)
        {
            _tutorId = tutorId;
        }

        public override Expression<Func<Advertisement, bool>> Criteria =>
            myAdvertisement => myAdvertisement.TutorId == _tutorId && !myAdvertisement.IsDeleted;
    }
    
    
}