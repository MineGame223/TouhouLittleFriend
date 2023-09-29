using System;
using System.Globalization;

namespace TouhouPets
{
    /// <summary>
    /// 时间表
    /// </summary>
    public static class TimeTable
    {
        private readonly static ChineseLunisolarCalendar china = new ();
        /// <summary>
        /// 检查当前农历日期是否匹配输入日期
        /// </summary>
        /// <param name="month">农历月</param>
        /// <param name="minDay">农历日最小值</param>
        /// <param name="maxDay">农历日最大值，默认等于最小值</param>
        /// <returns></returns>
        public static bool CurrentTimeOfChineseLunisolarCalendar(int month, int minDay, int maxDay = -1)
        {
            if (maxDay < minDay)
                maxDay = minDay;
            DateTime now = DateTime.Now;
            int _year = china.GetYear(now);
            int _month = china.GetMonth(now);
            int _day = china.GetDaysInMonth(_year, _month);
            int leapMonth = china.GetLeapMonth(_year);
            if (leapMonth > 0 && leapMonth < month)
            {
                month--;
            }

            return _month == month && _day >= minDay && _day <= maxDay;
        }
        /// <summary>
        /// 是否处于农历新年期间（正月初一 至 正月十五）
        /// </summary>
        public static bool DuringNewYear
        {
            get
            {
                return CurrentTimeOfChineseLunisolarCalendar(1, 1, 15);
            }
        }
    }
}
