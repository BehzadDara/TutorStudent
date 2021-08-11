using System;
using System.ComponentModel.DataAnnotations;

namespace TutorStudent.Application.Contracts
{
    public class AdvertisementDto :EntityDto<Guid>
    {
        [Required] public TutorDto Tutor { get; set; }
        [Required] public string Ticket { get; set; }
        [Required] public string TicketValue { get; set; }
        [Required] public string Duration { get; set; }
        [Required] public string Description { get; set; }
    }
}