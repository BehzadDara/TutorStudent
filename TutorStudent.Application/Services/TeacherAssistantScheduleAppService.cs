using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
    
    public class TeacherAssistantScheduleAppService: ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<TeacherAssistantSchedule> _repository;
        private readonly IRepository<TeacherAssistant> _teacherAssistants;
        private readonly IRepository<TeacherAssistantMeeting> _teacherAssistantMeeting;

        public TeacherAssistantScheduleAppService(IMapper mapper, IUnitOfWork unitOfWork, IRepository<TeacherAssistantSchedule> repository,
            IRepository<TeacherAssistant> teacherAssistants, IRepository<TeacherAssistantMeeting> teacherAssistantMeeting)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _repository = repository;
            _teacherAssistants = teacherAssistants;
            _teacherAssistantMeeting = teacherAssistantMeeting;
        }
        
        [HttpPost("TeacherAssistantSchedule")]
        public async Task<IActionResult> CreateTeacherAssistantSchedule(Guid userId, TeacherAssistantScheduleCreateDto input)
        {            
            var myTeacherAssistant = await _teacherAssistants.GetAsync(new GetTeacherAssistantByUserId(userId));
            if (myTeacherAssistant is null)
            {
                return Unauthorized(new ResponseDto(Error.AccessDenied));
            }
            if (input.Capacity <= 0)
            {
                return BadRequest(new ResponseDto(Error.CapacityControl));
            }
            if (input.BeginHour >= input.EndHour)
            {
                return BadRequest(new ResponseDto(Error.DateControl));
            }
            
            var myTeacherAssistantSchedule = _mapper.Map<TeacherAssistantSchedule>(input);
            myTeacherAssistantSchedule.TeacherAssistant = myTeacherAssistant;
            myTeacherAssistantSchedule.TeacherAssistantId = myTeacherAssistant.Id;
            
            _repository.Add(myTeacherAssistantSchedule);
            await _unitOfWork.CompleteAsync();
            
            return Ok(_mapper.Map<TeacherAssistantScheduleDto>(myTeacherAssistantSchedule));
        }  
        
        [HttpPut("TeacherAssistantSchedule")]
        public async Task<IActionResult> UpdateTeacherAssistantSchedule(Guid userId, Guid id, TeacherAssistantScheduleUpdateDto input)
        {
            var myTeacherAssistantSchedule = await _repository.GetByIdAsync(id);
            if (myTeacherAssistantSchedule is null)
            {
                return NotFound(new ResponseDto(Error.TeacherAssistantScheduleNotFound));
            }
            var myTeacherAssistant = await _teacherAssistants.GetAsync(new GetTeacherAssistantByUserId(userId));
            if (myTeacherAssistant is null)
            {
                return NotFound(new ResponseDto(Error.TeacherAssistantNotFound));
            }
            if (myTeacherAssistant.Id != myTeacherAssistantSchedule.TeacherAssistantId)
            {
                return Unauthorized(new ResponseDto(Error.AccessDenied));
            }

            if (input.Capacity < myTeacherAssistantSchedule.Capacity - myTeacherAssistantSchedule.Remain)
            {
                return BadRequest(new ResponseDto(Error.CapacityControl2));
            }

            var use = myTeacherAssistantSchedule.Capacity - myTeacherAssistantSchedule.Remain;

            myTeacherAssistantSchedule.Capacity = input.Capacity;
            myTeacherAssistantSchedule.Remain = myTeacherAssistantSchedule.Capacity - use;
            
            _repository.Update(myTeacherAssistantSchedule);
            await _unitOfWork.CompleteAsync();
            
            return Ok(_mapper.Map<TeacherAssistantScheduleDto>(myTeacherAssistantSchedule));
        }  
        
        [HttpDelete("TeacherAssistantSchedule")]
        public async Task<IActionResult> DeleteTeacherAssistantSchedule(Guid userId, Guid id)
        {
            var myTeacherAssistantSchedule = await _repository.GetByIdAsync(id);
            if (myTeacherAssistantSchedule is null)
            {
                return NotFound(new ResponseDto(Error.TeacherAssistantScheduleNotFound));
            }
            var myTeacherAssistant = await _teacherAssistants.GetAsync(new GetTeacherAssistantByUserId(userId));
            if (myTeacherAssistant is null)
            {
                return NotFound(new ResponseDto(Error.TeacherAssistantNotFound));
            }
            if (myTeacherAssistant.Id != myTeacherAssistantSchedule.TeacherAssistantId)
            {
                return Unauthorized(new ResponseDto(Error.AccessDenied));
            }
            var mytTeacherAssistantMeeting = await _teacherAssistantMeeting.ListAsync(new GetTeacherAssistantMeetingByTeacherAssistantScheduleId(myTeacherAssistantSchedule.Id));
            if (mytTeacherAssistantMeeting.Count != 0)
            {
                return Unauthorized(new ResponseDto(Error.TeacherAssistantScheduleInUse));
            }

            await _repository.DeleteAsync(myTeacherAssistantSchedule.Id);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
        
        [HttpGet("TeacherAssistantSchedules")]
        public async Task<IActionResult> GetSTeacherAssistantSchedules(Guid userId)
        {
            var myTeacherAssistant = await _teacherAssistants.GetAsync(new GetTeacherAssistantByUserId(userId));
            if (myTeacherAssistant is null)
            {
                return NotFound(new ResponseDto(Error.TeacherAssistantNotFound));
            }

            var myTeacherAssistantSchedules = await _repository.ListAsync(new GetTeacherAssistantScheduleByTeacherAssistantId(myTeacherAssistant.Id));
            
            return Ok(_mapper.Map<IList<TeacherAssistantScheduleDto>>(myTeacherAssistantSchedules).Where(y=>CheckDate(y.Date)).OrderBy(x=>x.Date));
        }

        [HttpGet("TeacherAssistantSchedule")]
        public async Task<IActionResult> GetSTeacherAssistantSchedule(Guid id)
        {
            var myTeacherAssistantSchedule = await _repository.GetByIdAsync(id);
            if (myTeacherAssistantSchedule is null)
            {
                return NotFound(new ResponseDto(Error.TeacherAssistantScheduleNotFound));
            }

            
            return Ok(_mapper.Map<TeacherAssistantScheduleDto>(myTeacherAssistantSchedule));
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
                persianCalendar.GetYear(DateTime.Now).ToString().Substring(0,4) +
                persianCalendar.GetMonth(DateTime.Now).ToString().PadLeft(2,'0') +
                persianCalendar.GetDayOfMonth(DateTime.Now).ToString().PadLeft(2,'0');
            return solarDate;
        }
    }
}