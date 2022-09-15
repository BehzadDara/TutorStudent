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
using TutorStudent.Domain.ProxyServices;
using TutorStudent.Domain.Specifications;
using TutorStudent.Domain.ProxyServices.Dto;

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
        private readonly ITrackingCode _trackingCode;
        private readonly INotification<EmailContextDto> _notification;
        private readonly IRepository<User> _user;

        public ApplyAppService(IMapper mapper, IUnitOfWork unitOfWork, IRepository<Apply> repository,
            IRepository<Tutor> tutors, IRepository<Student> students, IRepository<Log> logs, ITrackingCode trackingCode,
            INotification<EmailContextDto> notificatio, IRepository<User> user)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _repository = repository;
            _tutors = tutors;
            _students = students;
            _logs = logs;
            _trackingCode = trackingCode;
            _notification = notificatio;
            _user = user;
        }
        
        [HttpPost("Apply")]
        public async Task<IActionResult> CreateApply(Guid userId, ApplyCreateDto input)
        {            
            var myStudent = await _students.GetAsync(new GetStudentByUserId(userId));
            if (myStudent is null)
            {
                return NotFound(new ResponseDto(Error.StudentNotFound));
            }

            var myStudentUser = await _user.GetByIdAsync(myStudent.UserId);
            if (myStudentUser is null)
            {
                return NotFound(new ResponseDto(Error.UserNotFound));
            }

            var myTutor = await _tutors.GetByIdAsync(input.TutorId);
            if (myTutor is null)
            {
                return NotFound(new ResponseDto(Error.TutorNotFound));
            }

            var myTutorUser = await _user.GetByIdAsync(myTutor.UserId);

            if (myTutorUser is null)
            {
                return NotFound(new ResponseDto(Error.UserNotFound));
            }

            var myApply = _mapper.Map<Apply>(input);
            myApply.TutorId = myTutor.Id;
            myApply.Student = myStudent;
            myApply.StudentId = myStudent.Id;
            myApply.GetTrackingCode(_trackingCode);
            
            _repository.Add(myApply);
            await _unitOfWork.CompleteAsync();

            var emailContextDto = new EmailContextDto
            {
                To = myTutorUser.Email,
                Subject = "درخواست دانشجو به آگهی",
                Body = $"استاد گرامی {myTutorUser.FirstName} {myTutorUser.LastName}، درخواست با موضوع {myApply.Ticket.Humanize()} توسط {myStudentUser.FirstName} {myStudentUser.LastName} ثبت شد."
            };

            await _notification.Send(emailContextDto);

            return Ok(_mapper.Map<ApplyDto>(myApply));
        }  
        
        
        [HttpDelete("Apply")]
        public async Task<IActionResult> DeleteApply(Guid userId, Guid id)
        {
            var myApply = await _repository.GetByIdAsync(id);
            if (myApply is null)
            {
                return NotFound(new ResponseDto(Error.ApplyNotFound));
            }
            var myStudent = await _students.GetAsync(new GetStudentByUserId(userId));
            if (myStudent is null)
            {
                return NotFound(new ResponseDto(Error.StudentNotFound));
            }
            if (myStudent.Id != myApply.StudentId)
            {
                return Unauthorized(new ResponseDto(Error.AccessDenied));
            }

            await _repository.DeleteAsync(myApply.Id);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
        
                
        [HttpGet("Apply/Tutor")]
        public async Task<IActionResult> GetApplyByTutor(Guid userId)
        {
            var myTutor = await _tutors.GetAsync(new GetTutorByUserId(userId));
            if (myTutor is null)
            {
                return NotFound(new ResponseDto(Error.TutorNotFound));
            }

            var myApplys = await _repository.ListAsync(new GetApplyByTutorId(myTutor.Id));

            var myApplysDto = _mapper.Map<IList<ApplyDto>>(myApplys).OrderByDescending(x => x.CreatedAtUtc);
            
            foreach (var myApplyDto in myApplysDto)
            {
                var myStudent = await _students.GetByIdAsync(myApplyDto.StudentId);
                if (myStudent is null)
                {
                    return NotFound(Error.StudentNotFound);
                }

                myApplyDto.Student = _mapper.Map<StudentDto>(myStudent);
            }
            
            return Ok(myApplysDto);
        }   
        
        [HttpGet("Apply/Student")]
        public async Task<IActionResult> GetApplyByStudent(Guid userId)
        {
            var myStudent = await _students.GetAsync(new GetStudentByUserId(userId));
            if (myStudent is null)
            {
                return NotFound(new ResponseDto(Error.StudentNotFound));
            }

            var myApplys = await _repository.ListAsync(new GetApplyByStudentId(myStudent.Id));

            var myApplysDto = _mapper.Map<IList<ApplyDto>>(myApplys).OrderByDescending(x => x.CreatedAtUtc);
            
            foreach (var myApplyDto in myApplysDto)
            {
                var myTutor = await _tutors.GetByIdAsync(myApplyDto.TutorId);
                if (myTutor is null)
                {
                    return NotFound(Error.TutorNotFound);
                }
            
                myApplyDto.Tutor = _mapper.Map<TutorDto>(myTutor);
            }
            
            return Ok(myApplysDto);
        }

        [HttpGet("Apply/TrackingCode")]
        public async Task<IActionResult> GetApplyByTrackingCode(string trackingCode)
        {
            var myApply = await _repository.GetAsync(new GetApplyByTrackingCode(trackingCode));
            if (myApply is null)
            {
                return NotFound(new ResponseDto(Error.ApplyNotFound));
            }

            var myApplyDto = _mapper.Map<ApplyDto>(myApply);

            var myStudent = await _students.GetByIdAsync(myApplyDto.StudentId);
            if (myStudent is null)
            {
                return NotFound(Error.StudentNotFound);
            }

            myApplyDto.Student = _mapper.Map<StudentDto>(myStudent);

            var myTutor = await _tutors.GetByIdAsync(myApplyDto.TutorId);
            if (myTutor is null)
            {
                return NotFound(Error.TutorNotFound);
            }

            myApplyDto.Tutor = _mapper.Map<TutorDto>(myTutor);
            
            return Ok(myApplyDto);
        }

        [HttpGet("Apply")]
        public async Task<IActionResult> GetApply(Guid id)
        {
            var myApply = await _repository.GetByIdAsync(id);
            if (myApply is null)
            {
                return NotFound(new ResponseDto(Error.ApplyNotFound));
            }
            
            var myApplyDto = _mapper.Map<ApplyDto>(myApply);

            var myStudent = await _students.GetByIdAsync(myApplyDto.StudentId);
            if (myStudent is null)
            {
                return NotFound(Error.StudentNotFound);
            }

            myApplyDto.Student = _mapper.Map<StudentDto>(myStudent);

            var myTutor = await _tutors.GetByIdAsync(myApplyDto.TutorId);
            if (myTutor is null)
            {
                return NotFound(Error.TutorNotFound);
            }

            myApplyDto.Tutor = _mapper.Map<TutorDto>(myTutor);
            
            return Ok(myApplyDto);
        }

        [HttpPut("Apply/Open")]
        public async Task<IActionResult> OpenApply(Guid userId, Guid id)
        {
            var myApply = await _repository.GetByIdAsync(id);
            if (myApply is null)
            {
                return NotFound(new ResponseDto(Error.ApplyNotFound));
            }

            var myTutor = await _tutors.GetAsync(new GetTutorByUserId(userId));
            if (myTutor is null)
            {
                return NotFound(new ResponseDto(Error.TutorNotFound));
            }

            if (myTutor.Id != myApply.TutorId)
            {
                return Unauthorized(new ResponseDto(Error.AccessDenied));
            }

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
            await _unitOfWork.CompleteAsync();
            
            return Ok(_mapper.Map<ApplyDto>(myApply));
        }
        
        [HttpPut("Apply/Confirm")]
        public async Task<IActionResult> ConfirmApply(Guid userId, Guid id , string comment)
        {
            var myApply = await _repository.GetByIdAsync(id);
            if (myApply is null)
            {
                return NotFound(new ResponseDto(Error.ApplyNotFound));
            }
            
            var myTutor = await _tutors.GetAsync(new GetTutorByUserId(userId));
            if (myTutor is null)
            {
                return NotFound(new ResponseDto(Error.TutorNotFound));
            }

            if (myTutor.Id != myApply.TutorId)
            {
                return Unauthorized(new ResponseDto(Error.AccessDenied));
            }

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
            await _unitOfWork.CompleteAsync();
            
            return Ok(_mapper.Map<ApplyDto>(myApply));
        }
        
        [HttpPut("Apply/Reject")]
        public async Task<IActionResult> RejectApply(Guid userId, Guid id , string comment)
        {
            var myApply = await _repository.GetByIdAsync(id);
            if (myApply is null)
            {
                return NotFound(new ResponseDto(Error.ApplyNotFound));
            }

            var myTutor = await _tutors.GetAsync(new GetTutorByUserId(userId));
            if (myTutor is null)
            {
                return NotFound(new ResponseDto(Error.TutorNotFound));
            }

            if (myTutor.Id != myApply.TutorId)
            {
                return Unauthorized(new ResponseDto(Error.AccessDenied));
            }

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
            await _unitOfWork.CompleteAsync();
            
            return Ok(_mapper.Map<ApplyDto>(myApply));
        }
        
        [HttpGet("Log")]
        public async Task<IActionResult> GetLogs(Guid id)
        {
            var myApply = await _repository.GetByIdAsync(id);
            if (myApply is null)
            {
                return NotFound(new ResponseDto(Error.LogNotFound));
            }
            var myLogs = await _logs.ListAsync(new GetLogByApplyId(myApply.Id));
            return Ok(_mapper.Map<IList<LogDto>>(myLogs).OrderByDescending(x=>x.CreatedAtUtc));
        }
        
        
    }
}