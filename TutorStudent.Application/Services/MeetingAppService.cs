using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TutorStudent.Application.Contracts;
using TutorStudent.Domain.Interfaces;
using TutorStudent.Domain.Models;
using TutorStudent.Domain.Specifications;
using TutorStudent.Domain.ProxyServices.Dto;
using TutorStudent.Domain.ProxyServices;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Text;
using Microsoft.AspNetCore.Http.Internal;
using System.IO;

namespace TutorStudent.Application.Services
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class MeetingAppService: ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Meeting> _repository;
        private readonly IRepository<TutorSchedule> _tutorSchedule;
        private readonly IRepository<Student> _student;
        private readonly IRepository<Tutor> _tutor;
        private readonly IRepository<User> _user;
        private readonly INotification<EmailContextDto> _notification;

        public MeetingAppService(IMapper mapper, IUnitOfWork unitOfWork, IRepository<Meeting> repository, 
            IRepository<TutorSchedule> tutorSchedule, IRepository<Student> student,
            IRepository<Tutor> tutor, IRepository<User> user,
            INotification<EmailContextDto> notification)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _repository = repository;
            _tutorSchedule = tutorSchedule;
            _student = student;
            _tutor = tutor;
            _user = user;
            _notification = notification;
        }
        
        [HttpPost("Meeting")]
        public async Task<IActionResult> CreateMeeting(Guid userId, Guid tutorScheduleId)
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
            
            var myTutorSchedule = await _tutorSchedule.GetByIdAsync(tutorScheduleId);
            if (myTutorSchedule is null)
            {
                return NotFound(new ResponseDto(Error.TutorScheduleNotFound));
            }

            if (myTutorSchedule.Remain <= 0)
            {
                return BadRequest(new ResponseDto(Error.RemainControl));
            }

            var myMeetings = await _repository.ListAsync(new GetMeetingByTutorScheduleId(tutorScheduleId));
            if(myMeetings.Where(x=> x.StudentId == myStudent.Id).Count() > 0)
            {
                return BadRequest(new ResponseDto(Error.DuplicateMeeting));
            }

            var myTutor = await _tutor.GetByIdAsync(myTutorSchedule.TutorId);

            if (myTutor is null)
            {
                return NotFound(new ResponseDto(Error.TutorNotFound));
            }

            var myTutorUser = await _user.GetByIdAsync(myTutor.UserId);

            if (myTutorUser is null)
            {
                return NotFound(new ResponseDto(Error.UserNotFound));
            }

            var myMeeting = new Meeting
            {
                Student = myStudent,
                StudentId = myStudent.Id,
                TutorScheduleId = myTutorSchedule.Id
            };
            
            _repository.Add(myMeeting);
            myTutorSchedule.Remain--;
            _tutorSchedule.Update(myTutorSchedule);
            
            await _unitOfWork.CompleteAsync();

            var emailContextDto1 = new EmailContextDto
            {
                To = myStudentUser.Email,
                Subject = "رزرو جلسه توسط دانشجو",
                Body = $"دانشجوی گرامی {myStudentUser.FirstName} {myStudentUser.LastName}، رزرو جلسه با استاد {myTutorUser.FirstName} {myTutorUser.LastName} تاریخ {myTutorSchedule.Date} بازه زمانی {myTutorSchedule.BeginHour} تا {myTutorSchedule.EndHour} با موفقیت انجام شد.",
                Attachment = CreateMeetingAttachment(myTutorSchedule)
            };

            _notification.Send(emailContextDto1);

            var emailContextDto2 = new EmailContextDto
            {
                To = myTutorUser.Email,
                Subject = "رزرو جلسه توسط دانشجو",
                Body = $"استاد گرامی {myTutorUser.FirstName} {myTutorUser.LastName}، دانشجوی {myStudentUser.FirstName} {myStudentUser.LastName} تاریخ {myTutorSchedule.Date} بازه زمانی {myTutorSchedule.BeginHour} تا {myTutorSchedule.EndHour} را به عنوان وقت جلسه رزرو کرد.",
                Attachment = CreateMeetingAttachment(myTutorSchedule)
            };

            _notification.Send(emailContextDto2);

            return Ok(_mapper.Map<MeetingDto>(myMeeting));
        }

        private Byte[] CreateMeetingAttachment(TutorSchedule myTutorSchedule)
        {
            //some variables for demo purposes
            DateTime DateStart = ShamsiToMiladi(myTutorSchedule.Date, myTutorSchedule.BeginHour);
            DateTime DateEnd = ShamsiToMiladi(myTutorSchedule.Date, myTutorSchedule.BeginHour);
            string Summary = "رزرو جلسه توسط دانشجو";

            //create a new stringbuilder instance
            StringBuilder sb = new StringBuilder();

            //start the calendar item
            sb.AppendLine("BEGIN:VCALENDAR");
            sb.AppendLine("VERSION:2.0");
            sb.AppendLine("PRODID:tutorStudent");
            sb.AppendLine("CALSCALE:GREGORIAN");
            sb.AppendLine("METHOD:PUBLISH");

            //create a time zone if needed, TZID to be used in the event itself
            sb.AppendLine("BEGIN:VTIMEZONE");
            sb.AppendLine("TZID:Europe/Amsterdam");
            sb.AppendLine("BEGIN:STANDARD");
            sb.AppendLine("TZOFFSETTO:+0100");
            sb.AppendLine("TZOFFSETFROM:+0100");
            sb.AppendLine("END:STANDARD");
            sb.AppendLine("END:VTIMEZONE");

            //add the event
            sb.AppendLine("BEGIN:VEVENT");

            //with time zone specified
            sb.AppendLine("DTSTART;TZID=Europe/Amsterdam:" + DateStart.ToString("yyyyMMddTHHmm00"));
            sb.AppendLine("DTEND;TZID=Europe/Amsterdam:" + DateEnd.ToString("yyyyMMddTHHmm00"));
            //or without
            sb.AppendLine("DTSTART:" + DateStart.ToString("yyyyMMddTHHmm00"));
            sb.AppendLine("DTEND:" + DateEnd.ToString("yyyyMMddTHHmm00"));

            sb.AppendLine("SUMMARY:" + Summary + "");
            sb.AppendLine("END:VEVENT");

            //end calendar item
            sb.AppendLine("END:VCALENDAR");

            //create a string from the stringbuilder
            string CalendarItem = sb.ToString();

            byte[] bytes = Encoding.ASCII.GetBytes(CalendarItem);
            //var stream = new MemoryStream(bytes);
            return bytes;
            //FormFile file = new FormFile(stream, 0, stream.Length, "name", "fileName.extension");
            //return file;

            /*//send the calendar item to the browser
            Response.ClearHeaders();
            Response.Clear();
            Response.Buffer = true;
            Response.ContentType = "text/calendar";
            Response.AddHeader("content-length", CalendarItem.Length.ToString());
            Response.AddHeader("content-disposition", "attachment; filename=\"" + FileName + ".ics\"");
            Response.Write(CalendarItem);
            Response.Flush();
            HttpContext.Current.ApplicationInstance.CompleteRequest();*/
        }

        private DateTime ShamsiToMiladi(string date, int hour)
        {
            var validDate = date.Substring(0, 4) + "/" + date.Substring(4, 2) + "/" + date.Substring(6, 2);
            DateTime dt = DateTime.Parse(validDate, new CultureInfo("fa-IR"));
            dt.AddHours(hour);
            return dt;
        }

        [HttpDelete("Meeting")]
        public async Task<IActionResult> DeleteMeeting(Guid userId, Guid id)
        {
            var myStudent = await _student.GetAsync(new GetStudentByUserId(userId));
            if (myStudent is null)
            {
                return NotFound(new ResponseDto(Error.StudentNotFound));
            }
            
            var myMeeting = await _repository.GetByIdAsync(id);
            if (myMeeting is null)
            {
                return NotFound(new ResponseDto(Error.MeetingNotFound));
            }
            
            if (myMeeting.StudentId != myStudent.Id)
            {
                return Unauthorized(new ResponseDto(Error.AccessDenied));
            }
            
            var myTutorSchedule = await _tutorSchedule.GetByIdAsync(myMeeting.TutorScheduleId);
            if (myTutorSchedule is null)
            {
                return NotFound(new ResponseDto(Error.TutorScheduleNotFound));
            }

            await _repository.DeleteAsync(myMeeting.Id);
            
            myTutorSchedule.Remain++;
            _tutorSchedule.Update(myTutorSchedule);
            
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }  
        
        [HttpGet("Meeting")]
        public async Task<IActionResult> GetMeeting(Guid tutorScheduleId)
        {
            var myMeetings = await _repository.ListAsync(new GetMeetingByTutorScheduleId(tutorScheduleId));
            if (myMeetings is null)
            {
                return NotFound(new ResponseDto(Error.MeetingNotFound));
            }

            foreach (var myMeeting in myMeetings)
            {
                myMeeting.Student = await _student.GetByIdAsync(myMeeting.StudentId);
            }
            
            return Ok(_mapper.Map<IList<MeetingDto>>(myMeetings));
        }   
        
        
        
    }
}