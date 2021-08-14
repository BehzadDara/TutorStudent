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
    
    public class TutorScheduleAppService: ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<TutorSchedule> _repository;
        private readonly IRepository<Tutor> _tutors;
        private readonly IRepository<Meeting> _meeting;

        public TutorScheduleAppService(IMapper mapper, IUnitOfWork unitOfWork, IRepository<TutorSchedule> repository,
            IRepository<Tutor> tutors, IRepository<Meeting> meeting)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _repository = repository;
            _tutors = tutors;
            _meeting = meeting;
        }
        
        [HttpPost("TutorSchedule")]
        public async Task<IActionResult> CreateTutorSchedule(Guid userId, TutorScheduleCreateDto input)
        {            
            var myTutor = await _tutors.GetAsync(new GetTutorByUserId(userId));
            if (myTutor is null)
            {
                return Unauthorized(new ResponseDto(Error.AccessDenied));
            }
            if (input.Capacity <= 0)
            {
                return BadRequest(new ResponseDto(Error.CapacityControl));
            }
            if (Convert.ToInt32(input.BeginHour) >= Convert.ToInt32(input.EndHour))
            {
                return BadRequest(new ResponseDto(Error.DateControl));
            }
            
            var myTutorSchedule = _mapper.Map<TutorSchedule>(input);
            myTutorSchedule.Tutor = myTutor;
            myTutorSchedule.TutorId = myTutor.Id;
            
            _repository.Add(myTutorSchedule);
            await _unitOfWork.CompleteAsync();
            
            return Ok(_mapper.Map<TutorScheduleDto>(myTutorSchedule));
        }  
        
        [HttpPut("TutorSchedule")]
        public async Task<IActionResult> UpdateTutorSchedule(Guid userId, Guid id, TutorScheduleUpdateDto input)
        {
            var myTutorSchedule = await _repository.GetByIdAsync(id);
            if (myTutorSchedule is null)
            {
                return NotFound(new ResponseDto(Error.TutorScheduleNotFound));
            }
            var myTutor = await _tutors.GetAsync(new GetTutorByUserId(userId));
            if (myTutor is null)
            {
                return NotFound(new ResponseDto(Error.TutorNotFound));
            }
            if (myTutor.Id != myTutorSchedule.TutorId)
            {
                return Unauthorized(new ResponseDto(Error.AccessDenied));
            }

            if (input.Capacity < myTutorSchedule.Capacity - myTutorSchedule.Remain)
            {
                return BadRequest(new ResponseDto(Error.CapacityControl2));
            }

            var use = myTutorSchedule.Capacity - myTutorSchedule.Remain;

            myTutorSchedule.Capacity = input.Capacity;
            myTutorSchedule.Remain = myTutorSchedule.Capacity - use;
            
            _repository.Update(myTutorSchedule);
            await _unitOfWork.CompleteAsync();
            
            return Ok(_mapper.Map<TutorScheduleDto>(myTutorSchedule));
        }  
        
        [HttpDelete("TutorSchedule")]
        public async Task<IActionResult> DeleteTutorSchedule(Guid userId, Guid id)
        {
            var myTutorSchedule = await _repository.GetByIdAsync(id);
            if (myTutorSchedule is null)
            {
                return NotFound(new ResponseDto(Error.TutorScheduleNotFound));
            }
            var myTutor = await _tutors.GetAsync(new GetTutorByUserId(userId));
            if (myTutor is null)
            {
                return NotFound(new ResponseDto(Error.TutorNotFound));
            }
            if (myTutor.Id != myTutorSchedule.TutorId)
            {
                return Unauthorized(new ResponseDto(Error.AccessDenied));
            }
            var myMeeting = await _meeting.ListAsync(new GetMeetingByTutorScheduleId(myTutorSchedule.Id));
            if (myMeeting.Count != 0)
            {
                return Unauthorized(new ResponseDto(Error.TutorScheduleInUse));
            }

            await _repository.DeleteAsync(myTutorSchedule.Id);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
        
        [HttpGet("TutorSchedules")]
        public async Task<IActionResult> GetSTutorSchedules(Guid userId)
        {
            var myTutor = await _tutors.GetAsync(new GetTutorByUserId(userId));
            if (myTutor is null)
            {
                return NotFound(new ResponseDto(Error.TutorNotFound));
            }

            var myTutorSchedules = await _repository.ListAsync(new GetTutorScheduleByTutorId(myTutor.Id));
            
            return Ok(_mapper.Map<IList<TutorScheduleDto>>(myTutorSchedules).Where(y=>CheckDate(y.Date)).OrderBy(x=>x.Date));
        }

        [HttpGet("TutorSchedule")]
        public async Task<IActionResult> GetSTutorSchedule(Guid id)
        {
            var myTutorSchedule = await _repository.GetByIdAsync(id);
            if (myTutorSchedule is null)
            {
                return NotFound(new ResponseDto(Error.TutorScheduleNotFound));
            }

            
            return Ok(_mapper.Map<TutorScheduleDto>(myTutorSchedule));
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