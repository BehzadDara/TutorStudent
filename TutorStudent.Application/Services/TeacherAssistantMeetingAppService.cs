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

        public TeacherAssistantMeetingAppService(IMapper mapper, IUnitOfWork unitOfWork, IRepository<TeacherAssistantMeeting> repository, 
            IRepository<TeacherAssistantSchedule> teacherAssistantSchedule, IRepository<Student> student)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _repository = repository;
            _teacherAssistantSchedule = teacherAssistantSchedule;
            _student = student;
        }
        
        [HttpPost("TeacherAssistantMeeting")]
        public async Task<IActionResult> CreateTeacherAssistantMeeting(Guid userId, Guid TeacherAssistantScheduleId)
        {            
            var myStudent = await _student.GetAsync(new GetStudentByUserId(userId));
            if (myStudent is null)
            {
                return NotFound(new ResponseDto(Error.StudentNotFound));
            }   
            
            var myTeacherAssistantSchedule = await _teacherAssistantSchedule.GetByIdAsync(TeacherAssistantScheduleId);
            if (myTeacherAssistantSchedule is null)
            {
                return NotFound(new ResponseDto(Error.TeacherAssistantScheduleNotFound));
            }

            if (myTeacherAssistantSchedule.Remain <= 0)
            {
                return BadRequest(new ResponseDto(Error.RemainControl));
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