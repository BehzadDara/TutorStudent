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

            CreateMap<TutorScheduleCreateDto, TutorSchedule>()
                .ForMember(x => x.Remain, opt => opt.MapFrom(c => c.Capacity));
            CreateMap<TutorScheduleUpdateDto, TutorSchedule>();
            CreateMap<TutorSchedule, TutorScheduleDto>()
                .ForMember(x => x.MeetingValue, opt => opt.MapFrom(c => c.Meeting.Humanize()));

            CreateMap<Meeting, MeetingDto>();

            CreateMap<AdvertisementCreateDto, Advertisement>();
            CreateMap<AdvertisementUpdateDto, Advertisement>();
            CreateMap<Advertisement, AdvertisementDto>()
                .ForMember(x => x.TicketValue, opt => opt.MapFrom(c => c.Ticket));

            CreateMap<ApplyCreateDto, Apply>();
            CreateMap<Apply, ApplyDto>()
                .ForMember(x => x.TicketValue, opt => opt.MapFrom(c => c.Ticket))
                .ForMember(x => x.StateValue, opt => opt.MapFrom(c => c.State));
            


        }
    }
}