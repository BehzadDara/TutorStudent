using System;
using System.Collections.Generic;
using System.Text;

namespace TutorStudent.Application.Contracts
{
    public class FacultyManagementSuggestionDetailDto
    {
        public FacultyManagementSuggestionDto FacultyManagementSuggestionDto { get; set; }
        public List<TutorDto> Tutors { get; set; }
    }
}
