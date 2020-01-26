using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SG.TestRunService.Common
{
    public static class Helpers
    {
        public const string TimeFormat = "HH:mm:ss.fff";
        public const string DTFormat = "yyyy/MM/dd - " + TimeFormat;

        public static string ToPersianDateStr(this DateTime dateTime)
        {
            var cal = new PersianCalendar();
            return $"{cal.GetYear(dateTime)}/{cal.GetMonth(dateTime)}/{cal.GetDayOfMonth(dateTime)} - " +
                dateTime.ToString(TimeFormat);
        }
    }
}
