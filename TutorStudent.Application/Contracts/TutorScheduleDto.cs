﻿using System;
using System.ComponentModel.DataAnnotations;

namespace TutorStudent.Application.Contracts
{
    public class TutorScheduleDto :EntityDto<Guid>
    {
        [Required] public Guid TutorId { get; set; }
        [Required] public string Date { get; set; }
        [Required] public int BeginHour { get; set; }
        [Required] public int EndHour { get; set; }
        [Required] public string Meeting { get; set; }
        [Required] public string MeetingValue { get; set; }
        [Required] public int Capacity { get; set; }
        [Required] public int Remain { get; set; }
    }
}