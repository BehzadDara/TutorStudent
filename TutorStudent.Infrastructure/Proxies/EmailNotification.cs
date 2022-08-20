using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TutorStudent.Domain.Models;
using TutorStudent.Domain.ProxyServices;
using TutorStudent.Domain.ProxyServices.Dto;

namespace TutorStudent.Infrastructure.Proxies
{
    public class EmailNotification<T> : INotification<T> where T : EmailContextDto
    {
        public Task<bool> Send(User user, T context)
        {
            // send email

            return Task.FromResult(true);
        }
    }
}
