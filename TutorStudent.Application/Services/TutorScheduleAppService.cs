using System;
using System.Collections.Generic;
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
                return Unauthorized();
            }
            if (input.Capacity <= 0)
            {
                return BadRequest();
            }
            if (Convert.ToInt32(input.BeginHour) >= Convert.ToInt32(input.EndHour))
            {
                return BadRequest();
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
                return NotFound();
            }
            var myTutor = await _tutors.GetAsync(new GetTutorByUserId(userId));
            if (myTutor is null)
            {
                return NotFound();
            }
            if (myTutor.Id != myTutorSchedule.TutorId)
            {
                return Unauthorized();
            }

            if (input.Capacity < myTutorSchedule.Capacity - myTutorSchedule.Remain)
            {
                return BadRequest();
            }

            myTutorSchedule.Capacity = input.Capacity;
            
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
                return NotFound();
            }
            var myTutor = await _tutors.GetAsync(new GetTutorByUserId(userId));
            if (myTutor is null)
            {
                return NotFound();
            }
            if (myTutor.Id != myTutorSchedule.TutorId)
            {
                return Unauthorized();
            }
            var myMeeting = await _meeting.GetAsync(new GetMeetingByTutorScheduleId(myTutorSchedule.Id));
            if (!(myMeeting is null))
            {
                return Unauthorized();
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
                return NotFound();
            }

            var myTutorSchedules = await _repository.GetAsync(new GetTutorScheduleByTutorId(myTutor.Id));
            
            return Ok(_mapper.Map<IList<TutorScheduleDto>>(myTutorSchedules).OrderBy(x=>x.Date));
        }
        
        [HttpGet("TutorSchedule")]
        public async Task<IActionResult> GetSTutorSchedule(Guid id)
        {
            var myTutorSchedule = await _repository.GetByIdAsync(id);
            if (myTutorSchedule is null)
            {
                return NotFound();
            }

            
            return Ok(_mapper.Map<TutorScheduleDto>(myTutorSchedule));
        }
        
        
    }
}