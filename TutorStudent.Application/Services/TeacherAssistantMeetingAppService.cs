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
using TutorStudent.Domain.ProxyServices.Dto;
using TutorStudent.Domain.ProxyServices;
using System.Linq;

namespace TeacherAssistantStudent.Application.Services
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class TeacherAssistantMeetingAppService: ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<TeacherAssistantMeeting> _repository;
        private readonly IRepository<TeacherAssistantSchedule> _teacherAssistantSchedule;
        private readonly IRepository<Student> _student;
        private readonly IRepository<User> _user;
        private readonly INotification<EmailContextDto> _notification;

        public TeacherAssistantMeetingAppService(IMapper mapper, IUnitOfWork unitOfWork, IRepository<TeacherAssistantMeeting> repository, 
            IRepository<TeacherAssistantSchedule> teacherAssistantSchedule, IRepository<Student> student,
            IRepository<User> user, INotification<EmailContextDto> notification)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _repository = repository;
            _teacherAssistantSchedule = teacherAssistantSchedule;
            _student = student;
            _user = user;
            _notification = notification;
        }
        
        [HttpPost("TeacherAssistantMeeting")]
        public async Task<IActionResult> CreateTeacherAssistantMeeting(Guid userId, Guid teacherAssistantScheduleId)
        {            
            var myStudent = await _student.GetAsync(new GetStudentByUserId(userId));
            if (myStudent is null)
            {
                return NotFound(new ResponseDto(Error.StudentNotFound));
            }

            var myStudentUser = await _user.GetByIdAsync(myStudent.UserId);
            if (myStudentUser is null)
            {
                return NotFound(new ResponseDto(Error.UserNotFound));
            }

            var myTeacherAssistantSchedule = await _teacherAssistantSchedule.GetByIdAsync(teacherAssistantScheduleId);
            if (myTeacherAssistantSchedule is null)
            {
                return NotFound(new ResponseDto(Error.TeacherAssistantScheduleNotFound));
            }

            var myTeacherAssistantUser = await _user.GetByIdAsync(myTeacherAssistantSchedule.TeacherAssistantId);

            if (myTeacherAssistantUser is null)
            {
                return NotFound(new ResponseDto(Error.UserNotFound));
            }

            if (myTeacherAssistantSchedule.Remain <= 0)
            {
                return BadRequest(new ResponseDto(Error.RemainControl));
            }

            var myTeacherAssistantMeetings = await _repository.ListAsync(new GetTeacherAssistantMeetingByTeacherAssistantScheduleId(teacherAssistantScheduleId));
            if (myTeacherAssistantMeetings.Where(x => x.StudentId == myStudent.Id).Count() > 0)
            {
                return BadRequest(new ResponseDto(Error.DuplicateMeeting));
            }

            var myTeacherAssistantMeeting = new TeacherAssistantMeeting
            {
                Student = myStudent,
                StudentId = myStudent.Id,
                TeacherAssistantScheduleId = myTeacherAssistantSchedule.Id
            };
            
            _repository.Add(myTeacherAssistantMeeting);
            myTeacherAssistantSchedule.Remain--;
            _teacherAssistantSchedule.Update(myTeacherAssistantSchedule);
            
            await _unitOfWork.CompleteAsync();

            var emailContextDto1 = new EmailContextDto
            {
                To = myStudentUser.Email,
                Subject = "رزرو جلسه توسط دانشجو",
                Body = $"دانشجوی گرامی {myStudentUser.FirstName} {myStudentUser.LastName}، رزرو جلسه با تدریسیار {myTeacherAssistantUser.FirstName} {myTeacherAssistantUser.LastName} تاریخ {myTeacherAssistantSchedule.Date} بازه زمانی {myTeacherAssistantSchedule.BeginHour} تا {myTeacherAssistantSchedule.EndHour} با موفقیت انجام شد."
            };

            _notification.Send(emailContextDto1);

            var emailContextDto2 = new EmailContextDto
            {
                To = myTeacherAssistantUser.Email,
                Subject = "رزرو جلسه توسط دانشجو",
                Body = $"تدریسیار گرامی {myTeacherAssistantUser.FirstName} {myTeacherAssistantUser.LastName}، دانشجوی {myStudentUser.FirstName} {myStudentUser.LastName} تاریخ {myTeacherAssistantSchedule.Date} بازه زمانی {myTeacherAssistantSchedule.BeginHour} تا {myTeacherAssistantSchedule.EndHour} را به عنوان وقت جلسه رزرو کرد."
            };

            _notification.Send(emailContextDto2);

            return Ok(_mapper.Map<TeacherAssistantMeetingDto>(myTeacherAssistantMeeting));
        }  
        
        [HttpDelete("TeacherAssistantMeeting")]
        public async Task<IActionResult> DeleteTeacherAssistantMeeting(Guid userId, Guid id)
        {
            var myStudent = await _student.GetAsync(new GetStudentByUserId(userId));
            if (myStudent is null)
            {
                return NotFound(new ResponseDto(Error.StudentNotFound));
            }
            
            var myTeacherAssistantMeeting = await _repository.GetByIdAsync(id);
            if (myTeacherAssistantMeeting is null)
            {
                return NotFound(new ResponseDto(Error.TeacherAssistantMeetingNotFound));
            }
            
            if (myTeacherAssistantMeeting.StudentId != myStudent.Id)
            {
                return Unauthorized(new ResponseDto(Error.AccessDenied));
            }
            
            var myTeacherAssistantSchedule = await _teacherAssistantSchedule.GetByIdAsync(myTeacherAssistantMeeting.TeacherAssistantScheduleId);
            if (myTeacherAssistantSchedule is null)
            {
                return NotFound(new ResponseDto(Error.TeacherAssistantScheduleNotFound));
            }

            await _repository.DeleteAsync(myTeacherAssistantMeeting.Id);
            
            myTeacherAssistantSchedule.Remain++;
            _teacherAssistantSchedule.Update(myTeacherAssistantSchedule);
            
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }  
        
        [HttpGet("TeacherAssistantMeeting")]
        public async Task<IActionResult> GetTeacherAssistantMeeting(Guid TeacherAssistantScheduleId)
        {
            var myTeacherAssistantMeetings = await _repository.ListAsync(new GetTeacherAssistantMeetingByTeacherAssistantScheduleId(TeacherAssistantScheduleId));
            if (myTeacherAssistantMeetings is null)
            {
                return NotFound(new ResponseDto(Error.TeacherAssistantMeetingNotFound));
            }

            foreach (var myTeacherAssistantMeeting in myTeacherAssistantMeetings)
            {
                myTeacherAssistantMeeting.Student = await _student.GetByIdAsync(myTeacherAssistantMeeting.StudentId);
            }
            
            return Ok(_mapper.Map<IList<TeacherAssistantMeetingDto>>(myTeacherAssistantMeetings));
        }   
        
        
        
    }
}