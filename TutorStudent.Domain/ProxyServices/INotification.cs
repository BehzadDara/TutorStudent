using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TutorStudent.Domain.Models;

namespace TutorStudent.Domain.ProxyServices
{
    public interface INotification<T> where T : class
    {
        Task<bool> Send(User user, T context);
    }
}
