using System;
using System.Collections.Generic;
using System.Text;
using TutorStudent.Domain.Enums;

namespace TutorStudent.Domain.Models
{
    public class StudentDapperEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }
        public Guid UserId { get; set; }
        public string StudentNumber { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public GenderType Gender { get; set; }
        public RoleType Role { get; set; }

        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
    }
}
