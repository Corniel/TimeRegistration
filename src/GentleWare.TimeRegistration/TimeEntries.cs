using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GentleWare.TimeRegistration
{
    public class TimeEntries : List<TimeEntry>
    {
        public TimeEntry NewEntry(Stopwatch sw)
        {
            return NewEntry(sw.Elapsed);
        }
        public TimeEntry NewEntry(TimeSpan time)
        {
            var index = Count == 0 ? 1 : this.Max(entry => entry.Index) + 1;
            return new TimeEntry(index, time);
        }
    }
}
