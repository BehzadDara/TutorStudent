using System;
using System.ComponentModel.DataAnnotations;

namespace TutorStudent.Application.Contracts
{
    public class LogDto : EntityDto<Guid>
    {
        [Required] public string Before { get; set; }
        [Required] public string After { get; set; }
        [Required] public string Comment { get; set; }
    }
}