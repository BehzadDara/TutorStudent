using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Humanizer;
using TutorStudent.Application.Contracts;
using TutorStudent.Domain.Enums;
using TutorStudent.Domain.Interfaces;
using TutorStudent.Domain.Models;
using TutorStudent.Domain.Specifications;

namespace TutorStudent.Application.Services
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class ApplyAppService: ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Apply> _repository;
        private readonly IRepository<Student> _students;
        private readonly IRepository<Tutor> _tutors;
        private readonly IRepository<Log> _logs;

        public ApplyAppService(IMapper mapper, IUnitOfWork unitOfWork, IRepository<Apply> repository,
            IRepository<Tutor> tutors, IRepository<Student> students, IRepository<Log> logs)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _repository = repository;
            _tutors = tutors;
            _students = students;
            _logs = logs;
        }
        
        [HttpPost("Apply")]
        public async Task<IActionResult> CreateApply(Guid userId, ApplyCreateDto input)
        {            
            var myStudent = await _students.GetAsync(new GetStudentByUserId(userId));
            if (myStudent is null)
            {
                return NotFound();
            }       
            var myTutor = await _tutors.GetByIdAsync(input.TutorId);
            if (myTutor is null)
            {
                return NotFound();
            }
            
            var myApply = _mapper.Map<Apply>(input);
            myApply.TutorId = myTutor.Id;
            myApply.Student = myStudent;
            myApply.StudentId = myStudent.Id;
            
            _repository.Add(myApply);
            await _unitOfWork.CompleteAsync();
            
            return Ok(_mapper.Map<ApplyDto>(myApply));
        }  
        
        
        [HttpDelete("Apply")]
        public async Task<IActionResult> DeleteApply(Guid userId, Guid id)
        {
            var myApply = await _repository.GetByIdAsync(id);
            if (myApply is null)
            {
                return NotFound();
            }
            var myStudent = await _students.GetAsync(new GetStudentByUserId(userId));
            if (myStudent is null)
            {
                return NotFound();
            }
            if (myStudent.Id != myApply.StudentId)
            {
                return Unauthorized();
            }

            await _repository.DeleteAsync(myApply.Id);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
        
                
        [HttpGet("ApplysByTutor")]
        public async Task<IActionResult> GetApplyByTutor(Guid userId)
        {
            var myTutor = await _tutors.GetAsync(new GetTutorByUserId(userId));
            if (myTutor is null)
            {
                return NotFound();
            }

            var myApplys = await _repository.GetAsync(new GetApplyByTutorId(myTutor.Id));
            
            return Ok(_mapper.Map<IList<ApplyDto>>(myApplys).OrderByDescending(x=>x.CreatedAtUtc));
        }   
        
        [HttpGet("ApplysByStudent")]
        public async Task<IActionResult> GetApplyByStudent(Guid userId)
        {
            var myStudent = await _students.GetAsync(new GetStudentByUserId(userId));
            if (myStudent is null)
            {
                return NotFound();
            }

            var myApplys = await _repository.GetAsync(new GetApplyByStudentId(myStudent.Id));
            
            return Ok(_mapper.Map<IList<ApplyDto>>(myApplys).OrderByDescending(x=>x.CreatedAtUtc));
        }

        [HttpGet("Apply")]
        public async Task<IActionResult> GetApply(Guid id)
        {
            var myApply = await _repository.GetByIdAsync(id);
            if (myApply is null)
            {
                return NotFound();
            }
            
            return Ok(_mapper.Map<ApplyDto>(myApply));
        }

        [HttpPut("Apply/Open")]
        public async Task<IActionResult> OpenApply(Guid id)
        {
            var myApply = await _repository.GetByIdAsync(id);
            await myApply.FireTrigger(TriggerType.Open, "");
            _repository.Update(myApply);

            var myLog = new Log
            {
                Apply = myApply,
                ApplyId = myApply.Id,
                Before = StateType.Unseen.Humanize(),
                After = StateType.Seen.Humanize(),
                Comment = ""
            };
            _logs.Add(myLog);
            
            return Ok(_mapper.Map<ApplyDto>(myApply));
        }
        
        [HttpPut("Apply/Confirm")]
        public async Task<IActionResult> ConfirmApply(Guid id , string comment)
        {
            var myApply = await _repository.GetByIdAsync(id);
            await myApply.FireTrigger(TriggerType.Confirm, comment);
            _repository.Update(myApply);
            
            var myLog = new Log
            {
                Apply = myApply,
                ApplyId = myApply.Id,
                Before = StateType.Seen.Humanize(),
                After = StateType.Confirmed.Humanize(),
                Comment = comment
            };
            _logs.Add(myLog);
            
            return Ok(_mapper.Map<ApplyDto>(myApply));
        }
        
        [HttpPut("Apply/Reject")]
        public async Task<IActionResult> RejectApply(Guid id , string comment)
        {
            var myApply = await _repository.GetByIdAsync(id);
            await myApply.FireTrigger(TriggerType.Reject, comment);
            _repository.Update(myApply);
            
            var myLog = new Log
            {
                Apply = myApply,
                ApplyId = myApply.Id,
                Before = StateType.Seen.Humanize(),
                After = StateType.Rejected.Humanize(),
                Comment = comment
            };
            _logs.Add(myLog);
            
            return Ok(_mapper.Map<ApplyDto>(myApply));
        }
        
        [HttpGet("Logs")]
        public async Task<IActionResult> GetLogs(Guid id)
        {
            var myApply = await _repository.GetByIdAsync(id);
            if (myApply is null)
            {
                return NotFound();
            }
            var myLogs = await _logs.GetAsync(new GetLogByApplyId(myApply.Id));
            return Ok(_mapper.Map<IList<LogDto>>(myLogs));
        }
        
        
    }
}