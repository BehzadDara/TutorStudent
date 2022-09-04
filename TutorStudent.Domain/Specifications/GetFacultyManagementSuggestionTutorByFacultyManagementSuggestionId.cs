using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using TutorStudent.Domain.Interfaces;
using TutorStudent.Domain.Models;

namespace TutorStudent.Domain.Specifications
{
    public class GetFacultyManagementSuggestionTutorByFacultyManagementSuggestionId : Specification<FacultyManagementSuggestionTutor>
    {
        private readonly Guid _facultyManagementSuggestionId;

        public GetFacultyManagementSuggestionTutorByFacultyManagementSuggestionId(Guid facultyManagementSuggestionId)
        {
            _facultyManagementSuggestionId = facultyManagementSuggestionId;
        }

        public override Expression<Func<FacultyManagementSuggestionTutor, bool>> Criteria =>
            myFacultyManagementSuggestionTutor => myFacultyManagementSuggestionTutor.FacultyManagementSuggestionId == _facultyManagementSuggestionId;
    }
}
