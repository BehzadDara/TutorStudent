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

        public MeetingAppService(IMapper mapper, IUnitOfWork unitOfWork, IRepository<Meeting> repository, 
            IRepository<TutorSchedule> tutorSchedule, IRepository<Student> student)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _repository = repository;
            _tutorSchedule = tutorSchedule;
            _student = student;
        }
        
        [HttpPost("Meeting")]
        public async Task<IActionResult> CreateMeeting(Guid userId, Guid tutorScheduleId)
        {            
            var myStudent = await _student.GetAsync(new GetStudentByUserId(userId));
            if (myStudent is null)
            {
                return NotFound(new ResponseDto(Error.StudentNotFound));
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
            
            return Ok(_mapper.Map<MeetingDto>(myMeeting));
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