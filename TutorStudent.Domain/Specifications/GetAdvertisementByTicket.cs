using System;
using System.Globalization;
using System.Linq.Expressions;
using TutorStudent.Domain.Enums;
using TutorStudent.Domain.Interfaces;
using TutorStudent.Domain.Models;

namespace TutorStudent.Domain.Specifications
{
    public class GetAdvertisementByTicket : Specification<Advertisement>
    {

        private readonly TicketType _ticket;

        public GetAdvertisementByTicket(TicketType ticket)
        {
            _ticket = ticket;
        }

        public override Expression<Func<Advertisement, bool>> Criteria =>
            myAdvertisement => myAdvertisement.Ticket == _ticket && !myAdvertisement.IsDeleted;
    }
    
    
}