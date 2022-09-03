using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TutorStudent.Application.Contracts;
using TutorStudent.Domain.Enums;
using TutorStudent.Domain.Interfaces;
using TutorStudent.Domain.Models;
using TutorStudent.Domain.Specifications;
using System.Linq;
using System.Globalization;

namespace TutorStudent.Application.Services
{
    [ApiController]
    [Route("api/[controller]")]

    public class FacultyManagementAppService : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Tutor> _tutors;
        private readonly IRepository<User> _users;
        private readonly IRepository<TutorSchedule> _tutorSchedules;

        public FacultyManagementAppService(IMapper mapper, IUnitOfWork unitOfWork, IRepository<Tutor> tutors,
            IRepository<User> users, IRepository<TutorSchedule> tutorSchedules)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _tutors = tutors;
            _users = users;
            _tutorSchedules = tutorSchedules;
        }

        [HttpPost("TutorsFreeTimes")]
        public async Task<IActionResult> GetTutorsFreeTimes(Guid facultyManagementId, List<Guid> input)
        {
            var result = new List<FacultyManagementSuggestionDto>();

            var myFacultyManagement = await _users.GetByIdAsync(facultyManagementId);
            if (myFacultyManagement is null || myFacultyManagement.Role != RoleType.FacultyManagement)
            {
                return Unauthorized(new ResponseDto(Error.AccessDenied));
            }

            var myTutors = new List<Tutor>();
            foreach(var id in input)
            {
                var myTutor = await _tutors.GetByIdAsync(id);
                if(myTutor is null)
                {
                    return NotFound(new ResponseDto(Error.TutorNotFound));
                }
                myTutors.Add(myTutor);
            }

            var myTutorSchedules = new List<List<TutorSchedule>>();
            foreach (var myTutor in myTutors)
            {
                var myTutorSchedule = (await _tutorSchedules.ListAsync(new GetTutorScheduleByTutorId(myTutor.Id)))
                    .Where(y => CheckDate(y.Date)).OrderBy(x => x.Date).ToList();

                if(!myTutorSchedule.Any())
                {
                    var mytutorUser = await _users.GetByIdAsync(myTutor.UserId);
                    if(mytutorUser is null)
                    {
                        return NotFound(new ResponseDto(Error.UserNotFound));
                    }
                    return BadRequest(new ResponseDto(Error.FreeTutorScheduleNotExist + mytutorUser.FirstName + " " + mytutorUser.LastName));
                }

                myTutorSchedules.Add(myTutorSchedule);
            }

            foreach(var x in myTutorSchedules.FirstOrDefault())
            {
                result.Add(new FacultyManagementSuggestionDto
                {
                    Date = x.Date,
                    BeginHour = x.BeginHour,
                    EndHour = x.EndHour
                });
            }

            foreach (var myTutorSchedule in myTutorSchedules)
            {
                result = FindCommonTutorSchedule(result, myTutorSchedule);
                if(!result.Any())
                {
                    break;
                }
            }

            if (!result.Any() && myTutors.Count >= 3)
            {
                result = new List<FacultyManagementSuggestionDto>();

                foreach (var myTutor in myTutors)
                {
                    var mytutorUser = await _users.GetByIdAsync(myTutor.UserId);
                    if (mytutorUser is null)
                    {
                        return NotFound(new ResponseDto(Error.UserNotFound));
                    }

                    var tmpMyTutorSchedules = new List<List<TutorSchedule>>();
                    foreach (var myTutorSchedule in myTutorSchedules)
                    {
                        if(myTutorSchedule.FirstOrDefault().TutorId != myTutor.Id)
                        {
                            tmpMyTutorSchedules.Add(myTutorSchedule);
                        }
                    }

                    foreach (var x in tmpMyTutorSchedules.FirstOrDefault())
                    {
                        result.Add(new FacultyManagementSuggestionDto
                        {
                            Date = x.Date,
                            BeginHour = x.BeginHour,
                            EndHour = x.EndHour
                        });
                    }

                    foreach (var tmpMyTutorSchedule in tmpMyTutorSchedules)
                    {
                        result = FindCommonTutorSchedule(result, tmpMyTutorSchedule);
                        if (!result.Any())
                        {
                            break;
                        }
                    }

                    if (result.Any())
                    {
                        result.ForEach(x => x.Condition = 
                            String.Format(Error.RemoveTutorCondition, mytutorUser.FirstName, mytutorUser.LastName));
                    }

                }
            }

            if (!result.Any())
            {
                return NotFound(new ResponseDto(Error.CommonTutorScheduleNotFound));
            }

            return Ok(result);
        }

        private static List<FacultyManagementSuggestionDto> FindCommonTutorSchedule(List<FacultyManagementSuggestionDto> list1, List<TutorSchedule> list2)
        {
            var result = new List<FacultyManagementSuggestionDto>();

            foreach (var item1 in list1)
            {
                var tmpList2 = list2.Where(x => x.Date == item1.Date).ToList();
                if (tmpList2.Any())
                {
                    foreach (var item2 in tmpList2)
                    {
                        if (item1.EndHour <= item2.BeginHour || item1.BeginHour >= item2.EndHour)
                        {
                            continue;
                        }
                        else if(item1.EndHour > item2.BeginHour && item1.BeginHour <= item2.BeginHour &&
                            item1.EndHour <= item2.EndHour)
                        {
                            result.Add(new FacultyManagementSuggestionDto
                            {
                                Date = item1.Date,
                                BeginHour = item2.BeginHour,
                                EndHour = item1.EndHour
                            });
                        }
                        else if(item1.EndHour > item2.BeginHour && item1.BeginHour > item2.BeginHour &&
                            item1.EndHour <= item2.EndHour)
                        {
                            result.Add(new FacultyManagementSuggestionDto
                            {
                                Date = item1.Date,
                                BeginHour = item1.BeginHour,
                                EndHour = item1.EndHour
                            });
                        }
                        else if(item1.EndHour >= item2.EndHour && item1.BeginHour <= item2.BeginHour)
                        {
                            result.Add(new FacultyManagementSuggestionDto
                            {
                                Date = item1.Date,
                                BeginHour = item2.BeginHour,
                                EndHour = item2.EndHour
                            });
                        }
                        else if(item1.BeginHour >= item2.BeginHour && item1.BeginHour <= item2.EndHour &&
                            item1.EndHour >= item2.EndHour)
                        {
                            result.Add(new FacultyManagementSuggestionDto
                            {
                                Date = item1.Date,
                                BeginHour = item1.BeginHour,
                                EndHour = item2.EndHour
                            });
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }

            return result;
        }

        private static bool CheckDate(string date)
        {
            var result = String.Compare(date, ParseToSolar(), StringComparison.Ordinal);
            return result >= 0;
        }

        private static string ParseToSolar()
        {
            var persianCalendar = new PersianCalendar();
            var solarDate =
                persianCalendar.GetYear(DateTime.Now).ToString().Substring(0, 4) +
                persianCalendar.GetMonth(DateTime.Now).ToString().PadLeft(2, '0') +
                persianCalendar.GetDayOfMonth(DateTime.Now).ToString().PadLeft(2, '0');
            return solarDate;
        }

    }
}