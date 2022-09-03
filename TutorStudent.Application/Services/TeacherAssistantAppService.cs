﻿using System;
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

    public class TeacherAssistantAppService : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<TeacherAssistant> _repository;
        private readonly IRepository<User> _users;

        public TeacherAssistantAppService(IMapper mapper, IUnitOfWork unitOfWork, IRepository<TeacherAssistant> repository,
            IRepository<User> users)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _repository = repository;
            _users = users;
        }

        [HttpPost("TeacherAssistant")]
        public async Task<IActionResult> CreateTeacherAssistant(Guid tutorId, TeacherAssistantCreateDto input)
        {
            var myTutor = await _users.GetByIdAsync(tutorId);
            if (myTutor is null || myTutor.Role != RoleType.Tutor)
            {
                return Unauthorized(new ResponseDto(Error.AccessDenied));
            }

            var myTeacherAssistant = _mapper.Map<TeacherAssistant>(input);
            myTeacherAssistant.User.Role = RoleType.TeacherAssistant;
            _repository.Add(myTeacherAssistant);
            await _unitOfWork.CompleteAsync();

            return Ok(_mapper.Map<TeacherAssistantDto>(myTeacherAssistant));
        }

        [HttpPut("TeacherAssistant")]
        public async Task<IActionResult> UpdateTeacherAssistant(Guid id, TeacherAssistantUpdateDto input)
        {
            var myTeacherAssistant = await _repository.GetAsync(new GetTeacherAssistantByUserId(id));
            if (myTeacherAssistant is null)
            {
                return NotFound(new ResponseDto(Error.TeacherAssistantNotFound));
            }
            var myUser = await _users.GetByIdAsync(myTeacherAssistant.UserId);
            if (myUser is null)
            {
                return NotFound(new ResponseDto(Error.TeacherAssistantNotFound));
            }

            myTeacherAssistant.SkypeId = input.SkypeId;

            myUser.Email = input.ChangeInfo.Email;
            myUser.PhoneNumber = input.ChangeInfo.PhoneNumber;
            myUser.Address = input.ChangeInfo.Address;

            _repository.Update(myTeacherAssistant);
            _users.Update(myUser);

            myTeacherAssistant.User = myUser;
            await _unitOfWork.CompleteAsync();

            return Ok(_mapper.Map<TeacherAssistantDto>(myTeacherAssistant));
        }

        [HttpDelete("TeacherAssistant")]
        public async Task<IActionResult> DeleteTeacherAssistant(Guid tutorId, Guid id)
        {
            var myTutor = await _users.GetByIdAsync(tutorId);
            if (myTutor is null || myTutor.Role != RoleType.Tutor)
            {
                return Unauthorized(new ResponseDto(Error.AccessDenied));
            }

            var myTeacherAssistant = await _repository.GetAsync(new GetTeacherAssistantByUserId(id));
            if (myTeacherAssistant is null)
            {
                return NotFound(new ResponseDto(Error.TeacherAssistantNotFound));
            }
            var myUser = await _users.GetByIdAsync(myTeacherAssistant.UserId);
            if (myUser is null)
            {
                return NotFound(new ResponseDto(Error.TeacherAssistantNotFound));
            }

            await _repository.DeleteAsync(myTeacherAssistant.Id);
            await _users.DeleteAsync(myUser.Id);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }

        [HttpGet("TeacherAssistant")]
        public async Task<IActionResult> GetTutor(Guid id)
        {
            var myTeacherAssistant = await _repository.GetAsync(new GetTeacherAssistantByUserId(id));
            if (myTeacherAssistant is null)
            {
                return NotFound(new ResponseDto(Error.TeacherAssistantNotFound));
            }
            var myUser = await _users.GetByIdAsync(myTeacherAssistant.UserId);
            if (myUser is null)
            {
                return NotFound(new ResponseDto(Error.TeacherAssistantNotFound));
            }
            myTeacherAssistant.User = myUser;
            return Ok(_mapper.Map<TeacherAssistantDto>(myTeacherAssistant));
        }

        [HttpGet("TeacherAssistants")]
        public async Task<IActionResult> GetTutors()
        {
            var myTeacherAssistants = await _repository.ListAllAsync();

            foreach (var myTeacherAssistant in myTeacherAssistants)
            {
                var myUser = await _users.GetByIdAsync(myTeacherAssistant.UserId);
                if (myUser is null)
                {
                    return NotFound(new ResponseDto(Error.TeacherAssistantNotFound));
                }
                myTeacherAssistant.User = myUser;
            }

            return Ok(_mapper.Map<IList<TeacherAssistantDto>>(myTeacherAssistants));
        }


    }
}