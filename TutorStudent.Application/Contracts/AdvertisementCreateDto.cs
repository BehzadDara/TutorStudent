using System.ComponentModel.DataAnnotations;

namespace TutorStudent.Application.Contracts
{
    public class AdvertisementCreateDto
    {
        [Required] public string Ticket { get; set; }
        [Required] public string Duration { get; set; }
        [Required] public string Description { get; set; }
    }
}