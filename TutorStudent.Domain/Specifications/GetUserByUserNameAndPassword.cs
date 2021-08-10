using System;
using System.Linq.Expressions;
using TutorStudent.Domain.Interfaces;
using TutorStudent.Domain.Models;

namespace TutorStudent.Domain.Specifications
{
    public class GetUserByUserNameAndPassword : Specification<User>
    {

        private readonly string _userName;
        private readonly string _password;

        public GetUserByUserNameAndPassword(string userName, string password)
        {
            _userName = userName;
            _password = password;
        }

        public override Expression<Func<User, bool>> Criteria =>
            myUser => myUser.UserName == _userName && myUser.Password == _password;

    }
}