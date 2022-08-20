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

namespace TeacherAssistantStudent.Application.Services
{
    [ApiController]
    [Route("api/[controller]")]

    public class TeacherAssistantWeeklyScheduleAppService : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<TeacherAssistantWeeklySchedule> _repository;
        private readonly IRepository<TeacherAssistant> _teacherAssistants;
        private readonly IRepository<TeacherAssistantSchedule> _teacherAssistantSchedules;
        private readonly IRecurringJobManager _recurringJobManager;

        public TeacherAssistantWeeklyScheduleAppService(IMapper mapper, IUnitOfWork unitOfWork, IRepository<TeacherAssistantWeeklySchedule> repository,
            IRepository<TeacherAssistant> teacherAssistants, IRecurringJobManager recurringJobManager,
            IRepository<TeacherAssistantSchedule> teacherAssistantSchedules)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _repository = repository;
            _teacherAssistants = teacherAssistants;
            _recurringJobManager = recurringJobManager;
            _teacherAssistantSchedules = teacherAssistantSchedules;
        }

        [HttpPost("TeacherAssistantWeeklySchedule")]
        public async Task<IActionResult> CreateTeacherAssistantWeeklySchedule(Guid userId, TeacherAssistantWeeklyScheduleCreateDto input)
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

            var myTeacherAssistantWeeklySchedule = _mapper.Map<TeacherAssistantWeeklySchedule>(input);
            myTeacherAssistantWeeklySchedule.TeacherAssistant = myTeacherAssistant;
            myTeacherAssistantWeeklySchedule.TeacherAssistantId = myTeacherAssistant.Id;

            _repository.Add(myTeacherAssistantWeeklySchedule);
            await _unitOfWork.CompleteAsync();

            return Ok(_mapper.Map<TeacherAssistantWeeklyScheduleDto>(myTeacherAssistantWeeklySchedule));
        }

        [HttpPut("TeacherAssistantWeeklySchedule")]
        public async Task<IActionResult> UpdateTeacherAssistantWeeklySchedule(Guid userId, Guid id, TeacherAssistantWeeklyScheduleUpdateDto input)
        {
            var myTeacherAssistantWeeklySchedule = await _repository.GetByIdAsync(id);
            if (myTeacherAssistantWeeklySchedule is null)
            {
                return NotFound(new ResponseDto(Error.TeacherAssistantWeeklyScheduleNotFound));
            }
            var myTeacherAssistant = await _teacherAssistants.GetAsync(new GetTeacherAssistantByUserId(userId));
            if (myTeacherAssistant is null)
            {
                return NotFound(new ResponseDto(Error.TeacherAssistantNotFound));
            }
            if (myTeacherAssistant.Id != myTeacherAssistantWeeklySchedule.TeacherAssistantId)
            {
                return Unauthorized(new ResponseDto(Error.AccessDenied));
            }

            myTeacherAssistantWeeklySchedule.Capacity = input.Capacity;

            _repository.Update(myTeacherAssistantWeeklySchedule);
            await _unitOfWork.CompleteAsync();

            return Ok(_mapper.Map<TeacherAssistantWeeklyScheduleDto>(myTeacherAssistantWeeklySchedule));
        }

        [HttpDelete("TeacherAssistantWeeklySchedule")]
        public async Task<IActionResult> DeleteTeacherAssistantWeeklySchedule(Guid userId, Guid id)
        {
            var myTeacherAssistantWeeklySchedule = await _repository.GetByIdAsync(id);
            if (myTeacherAssistantWeeklySchedule is null)
            {
                return NotFound(new ResponseDto(Error.TeacherAssistantWeeklyScheduleNotFound));
            }
            var myTeacherAssistant = await _teacherAssistants.GetAsync(new GetTeacherAssistantByUserId(userId));
            if (myTeacherAssistant is null)
            {
                return NotFound(new ResponseDto(Error.TeacherAssistantNotFound));
            }
            if (myTeacherAssistant.Id != myTeacherAssistantWeeklySchedule.TeacherAssistantId)
            {
                return Unauthorized(new ResponseDto(Error.AccessDenied));
            }

            await _repository.DeleteAsync(myTeacherAssistantWeeklySchedule.Id);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        [HttpGet("TeacherAssistantWeeklySchedules")]
        public async Task<IActionResult> GetTeacherAssistantWeeklySchedules(Guid userId)
        {
            var myTeacherAssistant = await _teacherAssistants.GetAsync(new GetTeacherAssistantByUserId(userId));
            if (myTeacherAssistant is null)
            {
                return NotFound(new ResponseDto(Error.TeacherAssistantNotFound));
            }

            var myTeacherAssistantSchedules = await _repository.ListAsync(new GetTeacherAssistantWeeklyScheduleByTeacherAssistantId(myTeacherAssistant.Id));

            return Ok(_mapper.Map<IList<TeacherAssistantWeeklyScheduleDto>>(myTeacherAssistantSchedules));
        }

        [HttpGet("TeacherAssistantWeeklySchedule")]
        public async Task<IActionResult> GetTeacherAssistantWeeklySchedule(Guid id)
        {
            var myTeacherAssistantWeeklySchedule = await _repository.GetByIdAsync(id);
            if (myTeacherAssistantWeeklySchedule is null)
            {
                return NotFound(new ResponseDto(Error.TeacherAssistantWeeklyScheduleNotFound));
            }


            return Ok(_mapper.Map<TeacherAssistantWeeklyScheduleDto>(myTeacherAssistantWeeklySchedule));
        }


        [HttpGet("TriggerWeeklyScheduleJob")]
        public void GetTriggerWeeklyScheduleJob()
        {
            _recurringJobManager.AddOrUpdate(
                "set TeacherAssistant schedule every week",
                () => FillTeacherAssistantSchedule(),
                "00 20 * * Thu" // every thursday at 20:00
            );


        }

        [HttpGet("FillTeacherAssistantSchedule")]
        public async Task<IActionResult> FillTeacherAssistantSchedule()
        {
            var TeacherAssistantWeeklySchedules = await _repository.ListAllAsync();
            var TeacherAssistantSchedules = _mapper.Map<IList<TeacherAssistantSchedule>>(TeacherAssistantWeeklySchedules);
            foreach(var TeacherAssistantSchedule in TeacherAssistantSchedules)
            {
                _teacherAssistantSchedules.Add(TeacherAssistantSchedule);
                await _unitOfWork.CompleteAsync();
            }

            return Ok();
        }


    }
}
