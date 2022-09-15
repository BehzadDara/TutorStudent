using Humanizer;
using System;
using System.Globalization;
using TutorStudent.Application.Contracts;
using TutorStudent.Domain.Enums;
using TutorStudent.Domain.Implementations;
using TutorStudent.Domain.Models;

namespace TutorStudent.Api
{
    public class TutorStudentProfile : AutoMapper.Profile
    {
        public TutorStudentProfile()
        {
            CreateMap<UserCreateDto, User>()
                .ForMember(x => x.Password, opt => opt.MapFrom(c => Comb.HashPassword(c.UserName + c.Password + Error.PasswordTemp)));
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

            CreateMap<TutorWeeklyScheduleCreateDto, TutorWeeklySchedule>();
            CreateMap<TutorWeeklySchedule, TutorWeeklyScheduleDto>()
                .ForMember(x => x.MeetingValue, opt => opt.MapFrom(c => c.Meeting.Humanize()))
                .ForMember(x => x.WeekDayValue, opt => opt.MapFrom(c => c.WeekDay.Humanize()));

            CreateMap<TutorWeeklySchedule, TutorSchedule>()
                .ForMember(x => x.Remain, opt => opt.MapFrom(c => c.Capacity))
                .ForMember(x => x.Date, opt => opt.MapFrom(c => getDateFromWeekDay(c.WeekDay)));

            CreateMap<Meeting, MeetingDto>();

            CreateMap<AdvertisementCreateDto, Advertisement>();
            CreateMap<AdvertisementUpdateDto, Advertisement>();
            CreateMap<Advertisement, AdvertisementDto>()
                .ForMember(x => x.TicketValue, opt => opt.MapFrom(c => c.Ticket.Humanize()));

            CreateMap<ApplyCreateDto, Apply>();
            CreateMap<Apply, ApplyDto>()
                .ForMember(x => x.TicketValue, opt => opt.MapFrom(c => c.Ticket.Humanize()))
                .ForMember(x => x.StateValue, opt => opt.MapFrom(c => c.State.Humanize()));
            
            CreateMap<Log, LogDto>();

            CreateMap<TeacherAssistantCreateDto, TeacherAssistant>()
                .ForMember(x => x.User.UserName, opt => opt.MapFrom(c => c.User.UserName + "TA"));
            CreateMap<TeacherAssistant, TeacherAssistantDto>();

            CreateMap<TeacherAssistantMeeting, TeacherAssistantMeetingDto>();

            CreateMap<TeacherAssistantScheduleCreateDto, TeacherAssistantSchedule>()
                .ForMember(x => x.Remain, opt => opt.MapFrom(c => c.Capacity));
            CreateMap<TeacherAssistantScheduleUpdateDto, TeacherAssistantSchedule>();
            CreateMap<TeacherAssistantSchedule, TeacherAssistantScheduleDto>()
                .ForMember(x => x.MeetingValue, opt => opt.MapFrom(c => c.Meeting.Humanize()));

            CreateMap<TeacherAssistantWeeklyScheduleCreateDto, TeacherAssistantWeeklySchedule>();
            CreateMap<TeacherAssistantWeeklySchedule, TeacherAssistantWeeklyScheduleDto>()
                .ForMember(x => x.MeetingValue, opt => opt.MapFrom(c => c.Meeting.Humanize()))
                .ForMember(x => x.WeekDayValue, opt => opt.MapFrom(c => c.WeekDay.Humanize()));

            CreateMap<TeacherAssistantWeeklySchedule, TeacherAssistantSchedule>()
                .ForMember(x => x.Remain, opt => opt.MapFrom(c => c.Capacity))
                .ForMember(x => x.Date, opt => opt.MapFrom(c => getDateFromWeekDay(c.WeekDay)));

            CreateMap<FacultyManagementSuggestionDto, FacultyManagementSuggestion>();
            CreateMap<FacultyManagementSuggestion, FacultyManagementSuggestionDto>();

        }

        private string getDateFromWeekDay(WeekDayType weekDay)
        {
            var date = DateTime.Now; // today is thursday
            var dayOfWeek = date.DayOfWeek;
            var tmp = ((int)dayOfWeek * -1) + 7;

            switch (weekDay)
            {
                case WeekDayType.Saturday:
                    date = date.AddDays((tmp + 6) % 7);
                    break;
                case WeekDayType.Sunday:
                    date = date.AddDays((tmp + 0) % 7);
                    break;
                case WeekDayType.Monday:
                    date = date.AddDays((tmp + 1) % 7);
                    break;
                case WeekDayType.tuesday:
                    date = date.AddDays((tmp + 2) % 7);
                    break;
                case WeekDayType.Wednesday:
                    date = date.AddDays((tmp + 3) % 7);
                    break;
                case WeekDayType.Thursday:
                    date = date.AddDays((tmp + 4) % 7);
                    break;
                case WeekDayType.Friday:
                    date = date.AddDays((tmp + 5) % 7);
                    break;
            }
            var persianCalendar = new PersianCalendar();
            return 
                persianCalendar.GetYear(date).ToString().Substring(0, 4) +
                persianCalendar.GetMonth(date).ToString().PadLeft(2, '0') +
                persianCalendar.GetDayOfMonth(date).ToString().PadLeft(2, '0');
        }
    }
}