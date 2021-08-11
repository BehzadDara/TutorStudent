using System.ComponentModel.DataAnnotations;

namespace TutorStudent.Application.Contracts
{
    public class AdvertisementUpdateDto
    {
        [Required] public string Ticket { get; set; }
        [Required] public string Duration { get; set; }
        [Required] public string Description { get; set; }
    }
}