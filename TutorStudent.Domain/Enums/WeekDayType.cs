using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace TutorStudent.Domain.Enums
{
    public enum WeekDayType
    {
        [Description("شنبه")]
        Saturday,
        [Description("یک شنبه")]
        Sunday,
        [Description("دو شنبه")]
        Monday,
        [Description("سه شنبه")]
        tuesday,
        [Description("چهار شنبه")]
        Wednesday,
        [Description("پنج شنبه")]
        Thursday,
        [Description("جمعه")]
        Friday,
    }
}
