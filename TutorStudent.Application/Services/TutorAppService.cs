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

        public TutorAppService(IMapper mapper, IUnitOfWork unitOfWork, IRepository<Tutor> repository,
            IRepository<User> users)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _repository = repository;
            _users = users;
        }
        
        [HttpPost("Tutor")]
        public async Task<IActionResult> CreateTutor(Guid managerId, TutorCreateDto input)
        {            
            var myManager = await _users.GetByIdAsync(managerId);
            if (myManager is null || myManager.Role != RoleType.Manager)
            {
                return Unauthorized();
            }
            
            var myTutor = _mapper.Map<Tutor>(input);
            myTutor.User.Role = RoleType.Tutor;
            _repository.Add(myTutor);
            await _unitOfWork.CompleteAsync();
            
            return Ok(_mapper.Map<TutorDto>(myTutor));
        }  
        
        [HttpPut("Tutor")]
        public async Task<IActionResult> UpdateTutor(Guid id, TutorUpdateDto input)
        {
            var myTutor = await _repository.GetAsync(new GetTutorByUserId(id));
            if (myTutor is null)
            {
                return NotFound();
            }
            var myUser = await _users.GetByIdAsync(myTutor.UserId);
            if (myUser is null)
            {
                return NotFound();
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
                return Unauthorized();
            }
            
            var myTutor = await _repository.GetAsync(new GetTutorByUserId(id));
            if (myTutor is null)
            {
                return NotFound();
            }
            var myUser = await _users.GetByIdAsync(myTutor.UserId);
            if (myUser is null)
            {
                return NotFound();
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
                return NotFound();
            }
            var myUser = await _users.GetByIdAsync(myTutor.UserId);
            if (myUser is null)
            {
                return NotFound();
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
                    return NotFound();
                }
                myTutor.User = myUser;
            }
            
            return Ok(_mapper.Map<IList<TutorDto>>(myTutors));
        }
        
        
    }
}