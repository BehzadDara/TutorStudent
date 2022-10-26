using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TutorStudent.Domain.Models;

namespace TutorStudent.Domain.Interfaces
{
    public interface IStudentService
    {
        IList<StudentDapperEntity> GetYearStudentsWithDapper(int year);
        Guid CreateStudentWithDapper(Student student);
    }
}
