using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutorStudent.Domain.DependencyInjectionAttribute;
using TutorStudent.Domain.Enums;
using TutorStudent.Domain.Interfaces;
using TutorStudent.Domain.Models;

namespace TutorStudent.Infrastructure.Implementations
{
    [SingletonDependency(ServiceType = (typeof(IStudentService)))]
    public class StudentService : IStudentService
    {
        private readonly IDapperService _dapperService;

        public StudentService(IDapperService dapperService)
        {
            _dapperService = dapperService;
        }

        public IList<StudentDapperEntity> GetYearStudentsWithDapper(int year)
        {
            return _dapperService.QuerySP<StudentDapperEntity>(StoredProcedureNames.GetYearStudentsWithDapper, new
            {
                year = year
            }).ToList();
        }

        public Guid CreateStudentWithDapper(Student student)
        {
            DynamicParameters objDynamicParameters = new DynamicParameters();

            objDynamicParameters.Add("studentNumber", student.StudentNumber, DbType.String);
            objDynamicParameters.Add("userName", student.User.UserName, DbType.String);
            objDynamicParameters.Add("password", student.User.Password, DbType.String);
            objDynamicParameters.Add("firstName", student.User.FirstName, DbType.String);
            objDynamicParameters.Add("lastName", student.User.LastName, DbType.String);
            objDynamicParameters.Add("gender", student.User.Gender.ToString(), DbType.String);
            objDynamicParameters.Add("role", RoleType.Student.ToString(), DbType.String);
            objDynamicParameters.Add("email", student.User.Email, DbType.String);
            objDynamicParameters.Add("phoneNumber", student.User.PhoneNumber, DbType.String);
            objDynamicParameters.Add("address", student.User.Address, DbType.String);

            return _dapperService.QuerySP<DapperInsertUpdateDto>(StoredProcedureNames.CreateStudentWithDapper, objDynamicParameters).FirstOrDefault().ReturnId;

        }

    }
}
