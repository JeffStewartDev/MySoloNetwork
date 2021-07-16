using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSN.ViewModel.Util
{
    public class CalendarWeek
    {
        public int WeekNumber { get; set; }
        public CalendarDay Sunday { get; set; } = new CalendarDay();
        public CalendarDay Monday { get; set; } = new CalendarDay();
        public CalendarDay Tuesday { get; set; } = new CalendarDay();
        public CalendarDay Wednesday { get; set; } = new CalendarDay();
        public CalendarDay Thursday { get; set; } = new CalendarDay();
        public CalendarDay Friday { get; set; } = new CalendarDay();
        public CalendarDay Saturday { get; set; } = new CalendarDay();
    }
}
