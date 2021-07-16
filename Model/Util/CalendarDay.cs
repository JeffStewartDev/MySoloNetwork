using MSN.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MSN.ViewModel.Util
{
    public class CalendarDay
    {
        public DateTime Date { get; set; }
        public int Day { get; set; }
        public IEnumerable<UpdatePostItem> Posts { get; set; } = new List<UpdatePostItem>();
    }
}
