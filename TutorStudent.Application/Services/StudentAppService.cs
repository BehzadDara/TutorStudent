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
using TutorStudent.Domain.ProxyServices.Dto;
using TutorStudent.Domain.ProxyServices;

namespace TutorStudent.Application.Services
{
    [ApiController]
    [Route("api/[controller]")]
    
    public class StudentAppService: ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Student> _repository;
        private readonly IRepository<User> _users;
        private readonly INotification<EmailContextDto> _notification;

        public StudentAppService(IMapper mapper, IUnitOfWork unitOfWork, IRepository<Student> repository,
            IRepository<User> users, INotification<EmailContextDto> notification)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _repository = repository;
            _users = users;
            _notification = notification;
        }
        
        [HttpPost("Student")]
        public async Task<IActionResult> CreateStudent(Guid managerId, StudentCreateDto input)
        {            
            var myManager = await _users.GetByIdAsync(managerId);
            if (myManager is null || myManager.Role != RoleType.Manager)
            {
                return Unauthorized(new ResponseDto(Error.AccessDenied));
            }
            
            var myStudent = _mapper.Map<Student>(input);
            myStudent.User.Role = RoleType.Student;
            _repository.Add(myStudent);
            await _unitOfWork.CompleteAsync(); 
            
            var emailContextDto = new EmailContextDto
            {
                To = input.User.Email,
                Subject = "اطلاعات حساب کاربری سامانه تعامل استاد و دانشجو",
                Body = $"دانشجوی گرامی {myStudent.User.FirstName} {myStudent.User.LastName}، شما با رمز عبور {input.User.Password} به سامانه تعامل استاد و دانشجو اضافه شدید."
            };

            _notification.Send(emailContextDto);

            return Ok(_mapper.Map<StudentDto>(myStudent));
        }  
        
        [HttpPut("Student")]
        public async Task<IActionResult> UpdateStudent(Guid id, StudentUpdateDto input)
        {
            var myStudent = await _repository.GetAsync(new GetStudentByUserId(id));
            if (myStudent is null)
            {
                return NotFound(new ResponseDto(Error.StudentNotFound));
            }
            var myUser = await _users.GetByIdAsync(myStudent.UserId);
            if (myUser is null)
            {
                return NotFound(new ResponseDto(Error.StudentNotFound));
            }

            myStudent.StudentNumber = input.StudentNumber;
            
            myUser.Email = input.ChangeInfo.Email;
            myUser.PhoneNumber = input.ChangeInfo.PhoneNumber;
            myUser.Address = input.ChangeInfo.Address;
            
            _repository.Update(myStudent);
            _users.Update(myUser);
            
            myStudent.User = myUser;
            await _unitOfWork.CompleteAsync();
            
            return Ok(_mapper.Map<StudentDto>(myStudent));
        }  
        
        [HttpDelete("Student")]
        public async Task<IActionResult> DeleteStudent(Guid managerId, Guid id)
        {
            var myManager = await _users.GetByIdAsync(managerId);
            if (myManager is null || myManager.Role != RoleType.Manager)
            {
                return Unauthorized(new ResponseDto(Error.AccessDenied));
            }
            
            var myStudent = await _repository.GetAsync(new GetStudentByUserId(id));
            if (myStudent is null)
            {
                return NotFound(new ResponseDto(Error.StudentNotFound));
            }
            var myUser = await _users.GetByIdAsync(myStudent.UserId);
            if (myUser is null)
            {
                return NotFound(new ResponseDto(Error.StudentNotFound));
            }

            await _repository.DeleteAsync(myStudent.Id);
            await _users.DeleteAsync(myUser.Id);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }  
        
        [HttpGet("Student")]
        public async Task<IActionResult> GetStudent(Guid id)
        {
            var myStudent = await _repository.GetAsync(new GetStudentByUserId(id));
            if (myStudent is null)
            {
                return NotFound(new ResponseDto(Error.StudentNotFound));
            }
            var myUser = await _users.GetByIdAsync(myStudent.UserId);
            if (myUser is null)
            {
                return NotFound(new ResponseDto(Error.StudentNotFound));
            }
            myStudent.User = myUser;
            return Ok(_mapper.Map<StudentDto>(myStudent));
        }   
        
        [HttpGet("Students")]
        public async Task<IActionResult> GetStudents()
        {
            var myStudents = await _repository.ListAllAsync();

            foreach (var myStudent in myStudents)
            {
                var myUser = await _users.GetByIdAsync(myStudent.UserId);
                if (myUser is null)
                {
                    return NotFound(new ResponseDto(Error.StudentNotFound));
                }
                myStudent.User = myUser;
            }
            
            return Ok(_mapper.Map<IList<StudentDto>>(myStudents));
        }
        
        
    }
}