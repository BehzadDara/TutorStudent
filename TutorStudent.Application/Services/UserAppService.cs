using System;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TutorStudent.Application.Contracts;
using TutorStudent.Domain.Enums;
using TutorStudent.Domain.Implementations;
using TutorStudent.Domain.Interfaces;
using TutorStudent.Domain.Models;
using TutorStudent.Domain.Specifications;

namespace TutorStudent.Application.Services
{
    [ApiController]
    [Route("api/[controller]")]

    public class UserAppService : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<User> _repository;
        private readonly IRepository<Tutor> _tutors;
        private readonly IRepository<Student> _students;
        private readonly IRepository<TeacherAssistant> _teacherAssistant;

        public UserAppService(IMapper mapper, IUnitOfWork unitOfWork, IRepository<User> repository, 
            IRepository<Tutor> tutors, IRepository<Student> students, IRepository<TeacherAssistant> teacherAssistant)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _repository = repository;
            _tutors = tutors;
            _students = students;
            _teacherAssistant = teacherAssistant;
        }


        [HttpGet("Login")]
        public async Task<IActionResult> Login(string userName, string password)
        {
            var myUser = await _repository.GetAsync(new GetUserByUserNameAndPassword(userName, password));
            if (myUser is null)
            {
                return NotFound(new ResponseDto(Error.LoginError));
            }

            var myLogin = new LoginDto
            {
                Id = myUser.Id
            };
            return Ok(myLogin);
        }
        
        [HttpGet("User")]
        public async Task<IActionResult> GetUserById(Guid id)
        {
            var myUser = await _repository.GetByIdAsync(id);
            if (myUser is null)
            {
                return NotFound(new ResponseDto(Error.UserNotFound));
            }

            switch (myUser.Role)
            {
                case RoleType.Manager:
                case RoleType.FacultyManagement:
                    var myManagerDto = new ManagerDto
                    {
                        User = _mapper.Map<UserDto>(myUser)
                    };
                    return Ok(myManagerDto);
                case RoleType.Tutor:
                    var myTutor = await _tutors.GetAsync(new GetTutorByUserId(myUser.Id));
                    if (myTutor is null)
                    {
                        return NotFound(new ResponseDto(Error.UserNotFound));
                    }

                    myTutor.User = myUser;
                    return Ok(_mapper.Map<TutorDto>(myTutor));
                case RoleType.Student:
                    var myStudent = await _students.GetAsync(new GetStudentByUserId(myUser.Id));
                    if (myStudent is null)
                    {
                        return NotFound(new ResponseDto(Error.UserNotFound));
                    }

                    myStudent.User = myUser;
                    return Ok(_mapper.Map<StudentDto>(myStudent));

                default:
                    return NoContent();
            }
            
        }
        
        [HttpPost("Manager")]
        public async Task<IActionResult> CreateManager(string adminKey, UserCreateDto input)
        {
            if (adminKey != "b123@123")
            {
                return Unauthorized(new ResponseDto(Error.AccessDenied));
            }

            var myUser = _mapper.Map<User>(input);
            myUser.Role = RoleType.Manager;
            _repository.Add(myUser);
            await _unitOfWork.CompleteAsync();
            
            return Ok(_mapper.Map<UserDto>(myUser));
        }

        [HttpPost("FacultyManagement")]
        public async Task<IActionResult> CreateFacultyManagement(Guid managerId, UserCreateDto input)
        {
            var myManager = await _repository.GetByIdAsync(managerId);
            if (myManager is null || myManager.Role != RoleType.Manager)
            {
                return Unauthorized(new ResponseDto(Error.AccessDenied));
            }

            var myUser = _mapper.Map<User>(input);
            myUser.Role = RoleType.FacultyManagement;
            _repository.Add(myUser);
            await _unitOfWork.CompleteAsync();

            return Ok(_mapper.Map<UserDto>(myUser));
        }

        [HttpDelete("FacultyManagement")]
        public async Task<IActionResult> DeleteFacultyManagement(Guid managerId, Guid id)
        {
            var myManager = await _repository.GetByIdAsync(managerId);
            if (myManager is null || myManager.Role != RoleType.Manager)
            {
                return Unauthorized(new ResponseDto(Error.AccessDenied));
            }

            var myUser = await _repository.GetByIdAsync(id);
            if (myUser is null)
            {
                return NotFound(new ResponseDto(Error.FacultyManagementNotFound));
            }

            await _repository.DeleteAsync(myUser.Id);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        [HttpPut("User")]
        public async Task<IActionResult> UpdateUser(Guid managerId, Guid id, UserUpdateDto input)
        {
            var myManager = await _repository.GetByIdAsync(managerId);
            if (myManager is null || myManager.Role != RoleType.Manager)
            {
                return Unauthorized(new ResponseDto(Error.AccessDenied));
            }
            
            var myUser = await _repository.GetByIdAsync(id);
            if (myUser is null)
            {
                return NotFound(new ResponseDto(Error.UserNotFound));
            }

            myUser.UserName = input.UserName;
            myUser.FirstName = input.FirstName;
            myUser.LastName = input.LastName;
            myUser.Gender = (GenderType) Enum.Parse(typeof(GenderType), input.Gender, true);
            myUser.Role = (RoleType) Enum.Parse(typeof(RoleType), input.Role, true);            
            if (!string.IsNullOrEmpty(input.Password))
            {
                myUser.Password = Comb.HashPassword(myUser.UserName + input.Password + Error.PasswordTemp);
            }
            
            _repository.Update(myUser);
            await _unitOfWork.CompleteAsync();
            
            return Ok(_mapper.Map<UserDto>(myUser));
        } 
        
        [HttpPut("Password")]
        public async Task<IActionResult> UpdatePassword(Guid id, ChangePasswordDto input)
        {
            var myUser = await _repository.GetByIdAsync(id);
            if (myUser is null)
            {
                return NotFound(new ResponseDto(Error.UserNotFound));
            }

            if (myUser.Password != Comb.HashPassword(myUser.UserName + input.OldPassword + Error.PasswordTemp))
            {
                return BadRequest(new ResponseDto(Error.WrongPassword));
            }
            
            
            myUser.Password = Comb.HashPassword(myUser.UserName + input.NewPassword + Error.PasswordTemp);
            
            _repository.Update(myUser);
            await _unitOfWork.CompleteAsync();
            
            return Ok(_mapper.Map<UserDto>(myUser));
        }
        
        
    }
}