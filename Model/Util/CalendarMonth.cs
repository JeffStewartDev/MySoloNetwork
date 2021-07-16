using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSN.ViewModel.Util
{
    public class CalendarMonth
    {
        public int year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; }
        public List<CalendarWeek> CalendarWeeks { get; set; } = new List<CalendarWeek>();

        internal void Clear()
        {
            CalendarWeeks = new List<CalendarWeek>();
        }
    }
}
