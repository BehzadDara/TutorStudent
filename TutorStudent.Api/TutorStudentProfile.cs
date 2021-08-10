using Humanizer;
using TutorStudent.Application.Contracts;
using TutorStudent.Domain.Models;

namespace TutorStudent.Api
{
    public class TutorStudentProfile : AutoMapper.Profile
    {
        public TutorStudentProfile()
        {
            CreateMap<UserCreateDto, User>();
            CreateMap<UserUpdateDto, User>();
            CreateMap<User, UserDto>()
                .ForMember(x => x.GenderValue, opt => opt.MapFrom(c => c.Gender.Humanize()))
                .ForMember(x => x.RoleValue, opt => opt.MapFrom(c => c.Role.Humanize()));

            CreateMap<TutorCreateDto, Tutor>();
            CreateMap<TutorUpdateDto, Tutor>();
            CreateMap<Tutor, TutorDto>();
            
            CreateMap<StudentCreateDto, Student>();
            CreateMap<StudentUpdateDto, Student>();
            CreateMap<Student, StudentDto>();
            
        }
    }
}