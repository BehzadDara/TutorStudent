using System;
using System.Linq.Expressions;
using TutorStudent.Domain.Implementations;
using TutorStudent.Domain.Interfaces;
using TutorStudent.Domain.Models;

namespace TutorStudent.Domain.Specifications
{
    public class GetUserByUserName : Specification<User>
    {

        private readonly string _userName;

        public GetUserByUserName(string userName)
        {
            _userName = userName;
        }

        public override Expression<Func<User, bool>> Criteria =>
            myUser => myUser.UserName == _userName;

    }
}