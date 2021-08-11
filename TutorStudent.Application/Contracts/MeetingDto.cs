﻿using System;
using System.ComponentModel.DataAnnotations;

namespace TutorStudent.Application.Contracts
{
    public class MeetingDto :EntityDto<Guid>
    {
        [Required] public StudentDto Student { get; set; }

    }
}