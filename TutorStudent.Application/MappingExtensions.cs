using AutoMapper;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Text;
using TutorStudent.Application.Contracts;
using TutorStudent.Domain.Implementations;
using TutorStudent.Domain.Models;

namespace TutorStudent.Application
{
    public static class MappingExtensions
    {
        public static IList<StudentDto> ToModel(this IList<StudentDapperEntity> entity)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<StudentDapperEntity, StudentDto>()
                    .ForMember(x => x.Id, mo => mo.MapFrom(x => x.Id))
                    .ForMember(x => x.UserId, mo => mo.MapFrom(x => x.UserId))
                    .ForMember(x => x.CreatedAtUtc, mo => mo.MapFrom(x => x.CreatedAtUtc))
                    .ForMember(x => x.UpdatedAtUtc, mo => mo.MapFrom(x => x.UpdatedAtUtc))
                    .ForMember(x => x.StudentNumber, mo => mo.MapFrom(x => x.StudentNumber))
                    .ForPath(x => x.User.Id, mo => mo.MapFrom(x => x.UserId))
                    .ForPath(x => x.User.CreatedAtUtc, mo => mo.MapFrom(x => x.CreatedAtUtc))
                    .ForPath(x => x.User.UpdatedAtUtc, mo => mo.MapFrom(x => x.UpdatedAtUtc))
                    .ForPath(x => x.User.UserName, mo => mo.MapFrom(x => x.UserName))
                    .ForPath(x => x.User.FirstName, mo => mo.MapFrom(x => x.FirstName))
                    .ForPath(x => x.User.LastName, mo => mo.MapFrom(x => x.LastName))
                    .ForPath(x => x.User.Gender, mo => mo.MapFrom(x => x.Gender))
                    .ForPath(x => x.User.GenderValue, mo => mo.MapFrom(x => x.Gender.Humanize()))
                    .ForPath(x => x.User.Role, mo => mo.MapFrom(x => x.Role))
                    .ForPath(x => x.User.RoleValue, mo => mo.MapFrom(x => x.Role.Humanize()))
                    .ForPath(x => x.User.Email, mo => mo.MapFrom(x => x.Email))
                    .ForPath(x => x.User.PhoneNumber, mo => mo.MapFrom(x => x.PhoneNumber))
                    .ForPath(x => x.User.Address, mo => mo.MapFrom(x => x.Address));
            });

            IMapper mapper = config.CreateMapper();
            return mapper.Map<IList<StudentDapperEntity>, IList<StudentDto>>(entity);
        }

        public static Student ToEntity(this StudentCreateDto model)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<StudentCreateDto, Student>()
                    .ForMember(x => x.StudentNumber, mo => mo.MapFrom(x => x.StudentNumber))
                    .ForPath(x => x.User.UserName, mo => mo.MapFrom(x => x.User.UserName))
                    .ForPath(x => x.User.Password, mo => mo.MapFrom(x => Comb.HashPassword(x.User.UserName + x.User.Password + Error.PasswordTemp)))
                    .ForPath(x => x.User.FirstName, mo => mo.MapFrom(x => x.User.FirstName))
                    .ForPath(x => x.User.LastName, mo => mo.MapFrom(x => x.User.LastName))
                    .ForPath(x => x.User.Gender, mo => mo.MapFrom(x => x.User.Gender))
                    .ForPath(x => x.User.Email, mo => mo.MapFrom(x => x.User.Email))
                    .ForPath(x => x.User.PhoneNumber, mo => mo.MapFrom(x => x.User.PhoneNumber))
                    .ForPath(x => x.User.Address, mo => mo.MapFrom(x => x.User.Address));
            });

            IMapper mapper = config.CreateMapper();
            return mapper.Map<StudentCreateDto, Student>(model);
        }

    }
}
