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
    
    public class AdvertisementAppService: ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<Advertisement> _repository;
        private readonly IRepository<Tutor> _tutors;

        public AdvertisementAppService(IMapper mapper, IUnitOfWork unitOfWork, IRepository<Advertisement> repository,
            IRepository<Tutor> tutors)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _repository = repository;
            _tutors = tutors;
        }
        
        [HttpPost("Advertisement")]
        public async Task<IActionResult> CreateAdvertisement(Guid userId, AdvertisementCreateDto input)
        {            
            var myTutor = await _tutors.GetAsync(new GetTutorByUserId(userId));
            if (myTutor is null)
            {
                return NotFound();
            }
            
            var myAdvertisement = _mapper.Map<Advertisement>(input);
            myAdvertisement.Tutor = myTutor;
            myAdvertisement.TutorId = myTutor.Id;
            
            _repository.Add(myAdvertisement);
            await _unitOfWork.CompleteAsync();
            
            return Ok(_mapper.Map<AdvertisementDto>(myAdvertisement));
        }  
        
        [HttpPut("Advertisement")]
        public async Task<IActionResult> UpdateAdvertisement(Guid userId, Guid id, AdvertisementUpdateDto input)
        {
            var myAdvertisement = await _repository.GetByIdAsync(id);
            if (myAdvertisement is null)
            {
                return NotFound();
            }
            var myTutor = await _tutors.GetAsync(new GetTutorByUserId(userId));
            if (myTutor is null)
            {
                return NotFound();
            }
            if (myTutor.Id != myAdvertisement.TutorId)
            {
                return Unauthorized();
            }

            myAdvertisement.Ticket = (TicketType) Enum.Parse(typeof(TicketType), input.Ticket, true);
            myAdvertisement.Duration = input.Duration;
            myAdvertisement.Description = input.Description;

            _repository.Update(myAdvertisement);
            await _unitOfWork.CompleteAsync();
            
            return Ok(_mapper.Map<AdvertisementDto>(myAdvertisement));
        }  
        
        [HttpDelete("Advertisement")]
        public async Task<IActionResult> DeleteAdvertisement(Guid userId, Guid id)
        {
            var myAdvertisement = await _repository.GetByIdAsync(id);
            if (myAdvertisement is null)
            {
                return NotFound();
            }
            var myTutor = await _tutors.GetAsync(new GetTutorByUserId(userId));
            if (myTutor is null)
            {
                return NotFound();
            }
            if (myTutor.Id != myAdvertisement.TutorId)
            {
                return Unauthorized();
            }

            await _repository.DeleteAsync(myAdvertisement.Id);
            await _unitOfWork.CompleteAsync();

            return NoContent();
        }
        
        [HttpGet("Advertisements")]
        public async Task<IActionResult> GetAdvertisements()
        {

            var myAdvertisements = await _repository.ListAllAsync();
            
            return Ok(_mapper.Map<IList<AdvertisementDto>>(myAdvertisements).OrderByDescending(x=>x.CreatedAtUtc));
        }
                
        [HttpGet("Advertisement/Tutor")]
        public async Task<IActionResult> GetAdvertisementByTutor(Guid userId)
        {
            var myTutor = await _tutors.GetAsync(new GetTutorByUserId(userId));
            if (myTutor is null)
            {
                return NotFound();
            }

            var myAdvertisements = await _repository.ListAsync(new GetAdvertisementByTutorId(myTutor.Id));
            
            return Ok(_mapper.Map<IList<AdvertisementDto>>(myAdvertisements).OrderByDescending(x=>x.CreatedAtUtc));
        }
                
        [HttpGet("Advertisement/Ticket")]
        public async Task<IActionResult> GetAdvertisementByTicket(string ticket)
        {

            var myAdvertisements = await _repository.ListAsync(new GetAdvertisementByTicket((TicketType) Enum.Parse(typeof(TicketType), ticket, true)));
            
            return Ok(_mapper.Map<IList<AdvertisementDto>>(myAdvertisements).OrderByDescending(x=>x.CreatedAtUtc));
        }
                
        [HttpGet("Advertisement")]
        public async Task<IActionResult> GetAdvertisement(Guid id)
        {
            var myAdvertisement = await _repository.GetByIdAsync(id);
            if (myAdvertisement is null)
            {
                return NotFound();
            }
            
            return Ok(_mapper.Map<AdvertisementDto>(myAdvertisement));
        }
        
        
    }
}