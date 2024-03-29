﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace TutorStudent.Domain.ProxyServices.Dto
{
    public class EmailContextDto
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public IFormFile Attachment { get; set; }
    }
}
