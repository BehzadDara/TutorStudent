using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TutorStudent.Application.Contracts;
using TutorStudent.Domain.Interfaces;
using TutorStudent.Domain.Models;
using TutorStudent.Domain.Specifications;
using Hangfire;

namespace TutorStudent.Application.Services
{
    [ApiController]
    [Route("api/[controller]")]

    public class TutorWeeklyScheduleAppService : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<TutorWeeklySchedule> _repository;
        private readonly IRepository<Tutor> _tutors;
        private readonly IRepository<TutorSchedule> _tutorSchedules;
        private readonly IRecurringJobManager _recurringJobManager;

        public TutorWeeklyScheduleAppService(IMapper mapper, IUnitOfWork unitOfWork, IRepository<TutorWeeklySchedule> repository,
            IRepository<Tutor> tutors, IRecurringJobManager recurringJobManager,
            IRepository<TutorSchedule> tutorSchedules)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _repository = repository;
            _tutors = tutors;
            _recurringJobManager = recurringJobManager;
            _tutorSchedules = tutorSchedules;
        }

        [HttpPost("TutorWeeklySchedule")]
        public async Task<IActionResult> CreateTutorWeeklySchedule(Guid userId, TutorWeeklyScheduleCreateDto input)
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
            if (input.BeginHour >= input.EndHour)
            {
                return BadRequest(new ResponseDto(Error.DateControl));
            }

            var myTutorWeeklySchedule = _mapper.Map<TutorWeeklySchedule>(input);
            myTutorWeeklySchedule.Tutor = myTutor;
            myTutorWeeklySchedule.TutorId = myTutor.Id;

            _repository.Add(myTutorWeeklySchedule);
            await _unitOfWork.CompleteAsync();

            return Ok(_mapper.Map<TutorWeeklyScheduleDto>(myTutorWeeklySchedule));
        }

        [HttpPut("TutorWeeklySchedule")]
        public async Task<IActionResult> UpdateTutorWeeklySchedule(Guid userId, Guid id, TutorWeeklyScheduleUpdateDto input)
        {
            var myTutorWeeklySchedule = await _repository.GetByIdAsync(id);
            if (myTutorWeeklySchedule is null)
            {
                return NotFound(new ResponseDto(Error.TutorWeeklyScheduleNotFound));
            }
            var myTutor = await _tutors.GetAsync(new GetTutorByUserId(userId));
            if (myTutor is null)
            {
                return NotFound(new ResponseDto(Error.TutorNotFound));
            }
            if (myTutor.Id != myTutorWeeklySchedule.TutorId)
            {
                return Unauthorized(new ResponseDto(Error.AccessDenied));
            }

            myTutorWeeklySchedule.Capacity = input.Capacity;

            _repository.Update(myTutorWeeklySchedule);
            await _unitOfWork.CompleteAsync();

            return Ok(_mapper.Map<TutorWeeklyScheduleDto>(myTutorWeeklySchedule));
        }

        [HttpDelete("TutorWeeklySchedule")]
        public async Task<IActionResult> DeleteTutorWeeklySchedule(Guid userId, Guid id)
        {
            var myTutorWeeklySchedule = await _repository.GetByIdAsync(id);
            if (myTutorWeeklySchedule is null)
            {
                return NotFound(new ResponseDto(Error.TutorWeeklyScheduleNotFound));
            }
            var myTutor = await _tutors.GetAsync(new GetTutorByUserId(userId));
            if (myTutor is null)
            {
                return NotFound(new ResponseDto(Error.TutorNotFound));
            }
            if (myTutor.Id != myTutorWeeklySchedule.TutorId)
            {
                return Unauthorized(new ResponseDto(Error.AccessDenied));
            }

            await _repository.DeleteAsync(myTutorWeeklySchedule.Id);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        [HttpGet("TutorWeeklySchedules")]
        public async Task<IActionResult> GetTutorWeeklySchedules(Guid userId)
        {
            var myTutor = await _tutors.GetAsync(new GetTutorByUserId(userId));
            if (myTutor is null)
            {
                return NotFound(new ResponseDto(Error.TutorNotFound));
            }

            var myTutorSchedules = await _repository.ListAsync(new GetTutorWeeklyScheduleByTutorId(myTutor.Id));

            return Ok(_mapper.Map<IList<TutorWeeklyScheduleDto>>(myTutorSchedules));
        }

        [HttpGet("TutorWeeklySchedule")]
        public async Task<IActionResult> GetTutorWeeklySchedule(Guid id)
        {
            var myTutorWeeklySchedule = await _repository.GetByIdAsync(id);
            if (myTutorWeeklySchedule is null)
            {
                return NotFound(new ResponseDto(Error.TutorWeeklyScheduleNotFound));
            }


            return Ok(_mapper.Map<TutorWeeklyScheduleDto>(myTutorWeeklySchedule));
        }


        [HttpGet("TriggerWeeklyScheduleJob")]
        public void GetTriggerWeeklyScheduleJob()
        {
            _recurringJobManager.AddOrUpdate(
                "set tutor schedule every week",
                () => FillTutorSchedule(),
                "00 20 * * Thu" // every thursday at 20:00
            );


        }

        [HttpGet("FillTutorSchedule")]
        public async Task<IActionResult> FillTutorSchedule()
        {
            var tutorWeeklySchedules = await _repository.ListAllAsync();
            var tutorSchedules = _mapper.Map<IList<TutorSchedule>>(tutorWeeklySchedules);
            foreach(var tutorSchedule in tutorSchedules)
            {
                _tutorSchedules.Add(tutorSchedule);
                await _unitOfWork.CompleteAsync();
            }

            return Ok();
        }


    }
}
