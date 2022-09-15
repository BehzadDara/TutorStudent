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
using TutorStudent.Domain.ProxyServices;
using TutorStudent.Domain.ProxyServices.Dto;

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
        private readonly IRepository<FacultyManagementSuggestion> _facultyManagementSuggestions;
        private readonly IRepository<FacultyManagementSuggestionTutor> _facultyManagementSuggestionTutors;
        private readonly INotification<EmailContextDto> _notification;
        private readonly IRepository<User> _user;

        public FacultyManagementAppService(IMapper mapper, IUnitOfWork unitOfWork, IRepository<Tutor> tutors,
            IRepository<User> users, IRepository<TutorSchedule> tutorSchedules, 
            IRepository<FacultyManagementSuggestion> facultyManagementSuggestions, 
            IRepository<FacultyManagementSuggestionTutor> facultyManagementSuggestionTutors,
            INotification<EmailContextDto> notification, IRepository<User> user)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _tutors = tutors;
            _users = users;
            _tutorSchedules = tutorSchedules;
            _facultyManagementSuggestions = facultyManagementSuggestions;
            _facultyManagementSuggestionTutors = facultyManagementSuggestionTutors;
            _notification = notification;
            _user = user;
        }

        [HttpPost("TutorsMeeting")]
        public async Task<IActionResult> CreateTutorsMeeting(Guid facultyManagementId, FacultyManagementSuggestionDto facultyManagementSuggestionDto, [FromQuery] List<Guid> input)
        {
            var myFacultyManagement = await _users.GetByIdAsync(facultyManagementId);
            if (myFacultyManagement is null || myFacultyManagement.Role != RoleType.FacultyManagement)
            {
                return Unauthorized(new ResponseDto(Error.AccessDenied));
            }

            var myFacultyManagementSuggestion = _mapper.Map<FacultyManagementSuggestion>(facultyManagementSuggestionDto);

            _facultyManagementSuggestions.Add(myFacultyManagementSuggestion);

            foreach(var id in input)
            {
                var myTutor = await _tutors.GetByIdAsync(id);
                if (myTutor is null)
                {
                    return NotFound(new ResponseDto(Error.TutorNotFound));
                }

                var myTutorUser = await _user.GetByIdAsync(myTutor.UserId);

                if (myTutorUser is null)
                {
                    return NotFound(new ResponseDto(Error.UserNotFound));
                }

                var myFacultyManagementSuggestionTutor = new FacultyManagementSuggestionTutor
                {
                    FacultyManagementSuggestion = myFacultyManagementSuggestion,
                    TutorId = myTutor.Id
                };

                _facultyManagementSuggestionTutors.Add(myFacultyManagementSuggestionTutor);

                var emailContextDto = new EmailContextDto
                {
                    To = myTutorUser.Email,
                    Subject = "جلسه دفتر دانشکده",
                    Body = $"استاد گرامی {myTutorUser.FirstName} {myTutorUser.LastName}، دانشکده جلسه در تاریخ {facultyManagementSuggestionDto.Date} ساعت {facultyManagementSuggestionDto.BeginHour} الی {facultyManagementSuggestionDto.EndHour} را تنظیم کرد."
                };

                await _notification.Send(emailContextDto);


            }

            await _unitOfWork.CompleteAsync();

            return Ok(facultyManagementSuggestionDto);
        }

        [HttpDelete("TutorsMeeting")]
        public async Task<IActionResult> DeleteTutorsMeeting(Guid facultyManagementId, Guid id)
        {
            var myFacultyManagement = await _users.GetByIdAsync(facultyManagementId);
            if (myFacultyManagement is null || myFacultyManagement.Role != RoleType.FacultyManagement)
            {
                return Unauthorized(new ResponseDto(Error.AccessDenied));
            }

            var myFacultyManagementSuggestion = await _facultyManagementSuggestions.GetByIdAsync(id);
            if (myFacultyManagementSuggestion is null)
            {
                return NotFound(new ResponseDto(Error.FacultyManagementSuggestionNotFound));
            }

            var myFacultyManagementSuggestionTutors = await _facultyManagementSuggestionTutors.ListAsync(
                new GetFacultyManagementSuggestionTutorByFacultyManagementSuggestionId(id));

            foreach (var myFacultyManagementSuggestionTutor in myFacultyManagementSuggestionTutors)
            {
                await _facultyManagementSuggestionTutors.DeleteAsync(myFacultyManagementSuggestionTutor);
            }
            await _facultyManagementSuggestions.DeleteAsync(myFacultyManagementSuggestion);

            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        [HttpGet("TutorsMeeting")]
        public async Task<IActionResult> GetTutorsMeeting(Guid id, Guid input)
        {
            var myPerson = await _users.GetByIdAsync(id);
            if (myPerson is null || (myPerson.Role != RoleType.FacultyManagement && myPerson.Role != RoleType.Tutor))
            {
                return Unauthorized(new ResponseDto(Error.AccessDenied));
            }

            var myFacultyManagementSuggestion = await _facultyManagementSuggestions.GetByIdAsync(input);
            if(myFacultyManagementSuggestion is null)
            {
                return NotFound(new ResponseDto(Error.FacultyManagementSuggestionNotFound));
            }

            var myFacultyManagementSuggestionTutors = await _facultyManagementSuggestionTutors.ListAsync(
                new GetFacultyManagementSuggestionTutorByFacultyManagementSuggestionId(myFacultyManagementSuggestion.Id));

            var Tutors = new List<Tutor>();

            foreach(var myFacultyManagementSuggestionTutor in myFacultyManagementSuggestionTutors)
            {
                var myTutor = await _tutors.GetByIdAsync(myFacultyManagementSuggestionTutor.TutorId);
                if(myTutor is null)
                {
                    return NotFound(new ResponseDto(Error.TutorNotFound));
                }

                var myUser = await _users.GetByIdAsync(myTutor.UserId);
                if (myUser is null)
                {
                    return NotFound(new ResponseDto(Error.UserNotFound));
                }

                myTutor.User = myUser;

                Tutors.Add(myTutor);

            }

            var myFacultyManagementSuggestionDetailDto = new FacultyManagementSuggestionDetailDto
            {
                FacultyManagementSuggestionDto = _mapper.Map<FacultyManagementSuggestionDto>(myFacultyManagementSuggestion),
                Tutors = _mapper.Map<List<TutorDto>>(Tutors)
            };

            return Ok(myFacultyManagementSuggestionDetailDto);
        }
        

        [HttpGet("TutorsMeetings")]
        public async Task<IActionResult> GetTutorsMeetings(Guid id)
        {
            var myPerson = await _users.GetByIdAsync(id);
            if (myPerson is null || (myPerson.Role != RoleType.FacultyManagement && myPerson.Role != RoleType.Tutor))
            {
                return Unauthorized(new ResponseDto(Error.AccessDenied));
            }

            var myFacultyManagementSuggestions = (await _facultyManagementSuggestions.ListAllAsync()).Where(x=>CheckDate(x.Date)).OrderBy(x => x.Date).ToList();

            if(myPerson.Role == RoleType.Tutor)
            {
                var myTutor = await _tutors.GetAsync(new GetTutorByUserId(myPerson.Id));
                var myFacultyManagementSuggestionsTmp = new List<FacultyManagementSuggestion>();

                foreach(var myFacultyManagementSuggestion in myFacultyManagementSuggestions)
                {
                    var myFacultyManagementSuggestionTutors = await _facultyManagementSuggestionTutors.ListAsync(
                        new GetFacultyManagementSuggestionTutorByFacultyManagementSuggestionId(myFacultyManagementSuggestion.Id));

                    if(myFacultyManagementSuggestionTutors.Where(x=>x.TutorId == myTutor.Id).ToList().Count > 0)
                    {
                        myFacultyManagementSuggestionsTmp.Add(myFacultyManagementSuggestion);
                    }
                }

                myFacultyManagementSuggestions = myFacultyManagementSuggestionsTmp;
            }

            return Ok(_mapper.Map<List<FacultyManagementSuggestionDto>>(myFacultyManagementSuggestions));
        }


        [HttpPost("TutorsFreeTimes")]
        public async Task<IActionResult> GetTutorsFreeTimes(Guid facultyManagementId, List<Guid> input)
        {
            var result = new List<FacultyManagementSuggestion>();

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
                result.Add(new FacultyManagementSuggestion
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

                foreach (var myTutor in myTutors)
                {
                    var resultTmp = new List<FacultyManagementSuggestion>();

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
                        resultTmp.Add(new FacultyManagementSuggestion
                        {
                            Date = x.Date,
                            BeginHour = x.BeginHour,
                            EndHour = x.EndHour
                        });
                    }

                    foreach (var tmpMyTutorSchedule in tmpMyTutorSchedules)
                    {
                        resultTmp = FindCommonTutorSchedule(resultTmp, tmpMyTutorSchedule);
                        if (!resultTmp.Any())
                        {
                            break;
                        }
                    }

                    if (resultTmp.Any())
                    {
                        resultTmp.ForEach(x => x.Condition = 
                            String.Format(Error.RemoveTutorCondition, mytutorUser.FirstName, mytutorUser.LastName));
                        result.AddRange(resultTmp);
                    }

                }
            }

            if (!result.Any())
            {
                return NotFound(new ResponseDto(Error.CommonTutorScheduleNotFound));
            }

            return Ok(_mapper.Map<List<FacultyManagementSuggestionDto>>(result));
        }


        [HttpGet("FacultyManagements")]
        public async Task<IActionResult> GetFacultyManagements()
        {
            var users = await _users.ListAllAsync();

            return Ok(_mapper.Map<List<UserDto>>(users.Where(x => x.Role == RoleType.FacultyManagement)));
        }

        private static List<FacultyManagementSuggestion> FindCommonTutorSchedule(List<FacultyManagementSuggestion> list1, List<TutorSchedule> list2)
        {
            var result = new List<FacultyManagementSuggestion>();

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
                            result.Add(new FacultyManagementSuggestion
                            {
                                Date = item1.Date,
                                BeginHour = item2.BeginHour,
                                EndHour = item1.EndHour
                            });
                        }
                        else if(item1.EndHour > item2.BeginHour && item1.BeginHour > item2.BeginHour &&
                            item1.EndHour <= item2.EndHour)
                        {
                            result.Add(new FacultyManagementSuggestion
                            {
                                Date = item1.Date,
                                BeginHour = item1.BeginHour,
                                EndHour = item1.EndHour
                            });
                        }
                        else if(item1.EndHour >= item2.EndHour && item1.BeginHour <= item2.BeginHour)
                        {
                            result.Add(new FacultyManagementSuggestion
                            {
                                Date = item1.Date,
                                BeginHour = item2.BeginHour,
                                EndHour = item2.EndHour
                            });
                        }
                        else if(item1.BeginHour >= item2.BeginHour && item1.BeginHour <= item2.EndHour &&
                            item1.EndHour >= item2.EndHour)
                        {
                            result.Add(new FacultyManagementSuggestion
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