using System;
using System.Globalization;
using System.Linq.Expressions;
using TutorStudent.Domain.Interfaces;
using TutorStudent.Domain.Models;

namespace TutorStudent.Domain.Specifications
{
    public class GetTutorScheduleByTutorId : Specification<TutorSchedule>
    {

        private readonly Guid _tutorId;

        public GetTutorScheduleByTutorId(Guid tutorId)
        {
            _tutorId = tutorId;
        }
        
        private int ParseToSolar(DateTime dateTime)
        {
            PersianCalendar persianCalendar = new PersianCalendar();
            var solarDate = 
                persianCalendar.GetYear(DateTime.Now).ToString().Substring(0,4) +
                persianCalendar.GetMonth(DateTime.Now).ToString().PadLeft(2,'0') +
                persianCalendar.GetDayOfMonth(DateTime.Now).ToString().PadLeft(2,'0');
            return Convert.ToInt32(solarDate);
        }

        public override Expression<Func<TutorSchedule, bool>> Criteria =>
            myTutorSchedule => myTutorSchedule.TutorId == _tutorId && Convert.ToInt32(myTutorSchedule.Date) >= ParseToSolar(DateTime.Now);
    }
    
    
}