using System;

namespace TutorStudent.Domain.ProxyServices.Dto
{
    public class EmailContextDto
    {
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public Byte[] Attachment { get; set; }
    }
}
