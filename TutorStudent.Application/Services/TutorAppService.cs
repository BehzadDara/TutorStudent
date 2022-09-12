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
    
    public class TutorAppService: ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Tutor> _repository;
        private readonly IRepository<User> _users;
        private readonly INotification<EmailContextDto> _notification;

        public TutorAppService(IMapper mapper, IUnitOfWork unitOfWork, IRepository<Tutor> repository,
            IRepository<User> users, INotification<EmailContextDto> notification)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _repository = repository;
            _users = users;
            _notification = notification;
        }
        
        [HttpPost("Tutor")]
        public async Task<IActionResult> CreateTutor(Guid managerId, TutorCreateDto input)
        {            
            var myManager = await _users.GetByIdAsync(managerId);
            if (myManager is null || myManager.Role != RoleType.Manager)
            {
                return Unauthorized(new ResponseDto(Error.AccessDenied));
            }
            
            var myTutor = _mapper.Map<Tutor>(input);
            myTutor.User.Role = RoleType.Tutor;
            _repository.Add(myTutor);
            await _unitOfWork.CompleteAsync();

            var emailContextDto = new EmailContextDto
            {
                To = input.User.Email,
                Subject = "اطلاعات حساب کاربری سامانه تعامل استاد و دانشجو",
                Body = $"استاد گرامی {myTutor.User.FirstName} {myTutor.User.LastName}، شما با رمز عبور {input.User.Password} به سامانه تعامل استاد و دانشجو اضافه شدید."
            };

            _notification.Send(emailContextDto);

            return Ok(_mapper.Map<TutorDto>(myTutor));
        }  
        
        [HttpPut("Tutor")]
        public async Task<IActionResult> UpdateTutor(Guid id, TutorUpdateDto input)
        {
            var myTutor = await _repository.GetAsync(new GetTutorByUserId(id));
            if (myTutor is null)
            {
                return NotFound(new ResponseDto(Error.TutorNotFound));
            }
            var myUser = await _users.GetByIdAsync(myTutor.UserId);
            if (myUser is null)
            {
                return NotFound(new ResponseDto(Error.TutorNotFound));
            }

            myTutor.SkypeId = input.SkypeId;
            
            myUser.Email = input.ChangeInfo.Email;
            myUser.PhoneNumber = input.ChangeInfo.PhoneNumber;
            myUser.Address = input.ChangeInfo.Address;

            _repository.Update(myTutor);
            _users.Update(myUser);
            
            myTutor.User = myUser;
            await _unitOfWork.CompleteAsync();
            
            return Ok(_mapper.Map<TutorDto>(myTutor));
        }  
        
        [HttpDelete("Tutor")]
        public async Task<IActionResult> DeleteTutor(Guid managerId, Guid id)
        {
            var myManager = await _users.GetByIdAsync(managerId);
            if (myManager is null || myManager.Role != RoleType.Manager)
            {
                return Unauthorized(new ResponseDto(Error.AccessDenied));
            }
            
            var myTutor = await _repository.GetAsync(new GetTutorByUserId(id));
            if (myTutor is null)
            {
                return NotFound(new ResponseDto(Error.TutorNotFound));
            }
            var myUser = await _users.GetByIdAsync(myTutor.UserId);
            if (myUser is null)
            {
                return NotFound(new ResponseDto(Error.TutorNotFound));
            }

            await _repository.DeleteAsync(myTutor.Id);
            await _users.DeleteAsync(myUser.Id);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }  
        
        [HttpGet("Tutor")]
        public async Task<IActionResult> GetTutor(Guid id)
        {
            var myTutor = await _repository.GetAsync(new GetTutorByUserId(id));
            if (myTutor is null)
            { 
                return NotFound(new ResponseDto(Error.TutorNotFound));
            }
            var myUser = await _users.GetByIdAsync(myTutor.UserId);
            if (myUser is null)
            {
                return NotFound(new ResponseDto(Error.TutorNotFound));
            }
            myTutor.User = myUser;
            return Ok(_mapper.Map<TutorDto>(myTutor));
        }   
        
        [HttpGet("Tutors")]
        public async Task<IActionResult> GetTutors()
        {
            var myTutors = await _repository.ListAllAsync();

            foreach (var myTutor in myTutors)
            {
                var myUser = await _users.GetByIdAsync(myTutor.UserId);
                if (myUser is null)
                {
                    return NotFound(new ResponseDto(Error.TutorNotFound));
                }
                myTutor.User = myUser;
            }
            
            return Ok(_mapper.Map<IList<TutorDto>>(myTutors));
        }
        
        
    }
}