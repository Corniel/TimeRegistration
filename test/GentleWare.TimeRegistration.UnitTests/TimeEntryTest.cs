using NUnit.Framework;
using System;
using System.Globalization;

namespace GentleWare.TimeRegistration.UnitTests
{
    public class TimeEntryTest
    {
        [TestCase("001     92,887.180", 3, TimeEntry.Display.Seconds)]
        [TestCase("001   1,548:07.18", 2, TimeEntry.Display.Minutes)]
        [TestCase("001   25:48:07.180", 3, TimeEntry.Display.Hours)]
        [TestCase("001 1:01:48:07.180", 3, TimeEntry.Display.Days)]
        public void ToString_25h48m7s180f(string expected, int precision, TimeEntry.Display display)
        {
            var ts = new TimeSpan(1, 01, 48, 07, 180);
            var entry = new TimeEntry(1, ts);
            
            var str = entry.ToString(display, precision, CultureInfo.InvariantCulture);

            Assert.AreEqual(expected, str);
        }
    }
}
