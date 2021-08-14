
namespace TutorStudent.Domain.Models
{
    public static class Error
    {
        public const string LoginError = "نام کاربری یا رمز عبور اشتباه است";
        public const string WrongPassword = "رمز عبور اشتباه است";
        public const string UserNotFound = "کاربر یافت نشد";
        public const string TutorNotFound = "استاد یافت نشد";
        public const string StudentNotFound = "دانشجو یافت نشد";
        public const string AccessDenied = "شما دسترسی انجام این کار را ندارید";
        public const string CapacityControl = "ظرفیت باید بزرگتر از صفر باشد";
        public const string DateControl = "تاریخ شروع باید قبل از تاریخ پایان باشد";
        public const string TutorScheduleNotFound = "زمان بندی مد نظر یافت نشد";
        public const string CapacityControl2 = "مقدار ظرفیت جدید باید بیش از تعداد رزرو انجام شده باشد";
        public const string TutorScheduleInUse = "زمان بندی مد نظر دارای رزرو فعال است";
        public const string RemainControl = "ظرفیت زمان بندی مد نظر به تمام رسیده است";
        public const string MeetingNotFound = "رزرو مد نظر یافت نشد";
        public const string AdvertisementNotFound = "آگهی مد نظر یافت نشد";
        public const string ApplyNotFound = "درخواست مد نظر یافت نشد";
        public const string LogNotFound = "سابقه ای یافت نشد";
    }
}