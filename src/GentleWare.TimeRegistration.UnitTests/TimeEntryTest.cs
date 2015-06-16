using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GentleWare.TimeRegistration.UnitTests
{
	[TestFixture]
	public class TimeEntryTest
	{
		[Test]
		public void ToString_25h48m7s180f()
		{
			var ts = new TimeSpan(1, 01, 48, 07, 180);
			var entry = new TimeEntry(1, ts);
			var precision = 3;
			var culture = CultureInfo.InvariantCulture;

			var s = entry.ToString(TimeEntry.Display.Seconds, precision, culture);
			var m = entry.ToString(TimeEntry.Display.Minutes, precision -1, culture);
			var h = entry.ToString(TimeEntry.Display.Hours, precision, culture);
			var d = entry.ToString(TimeEntry.Display.Days, precision, culture);

			Assert.AreEqual("001     92,887.180", s, "Seconds");
			Assert.AreEqual("001   1,548:07.18", m, "Minutes");
			Assert.AreEqual("001   25:48:07.180", h, "Hours");
			Assert.AreEqual("001 1:01:48:07.180", d, "Days");
		}
	}
}
