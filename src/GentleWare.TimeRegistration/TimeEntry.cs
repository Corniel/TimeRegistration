using Qowaiv;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Text;

namespace GentleWare.TimeRegistration
{
	[DebuggerDisplay("{DebuggerDisplay}")]
	public struct TimeEntry
	{
		public enum Display
		{
			Seconds = 0,
			Minutes,
			Hours,
			Days,
		}

		private Int32 m_Index;
		private TimeSpan m_Time;

		public TimeEntry(Int32 index, TimeSpan time)
		{
			m_Index = index;
			m_Time = time;
		}

		public Int32 Index { get { return m_Index; } }
		public TimeSpan Time { get { return m_Time; } }

		public void AppendToConsole(TextWriter writer, Display display, int precision)
		{
			Guard.NotNull(writer, "writer");
			writer.Write("\r" + ToString(display, precision));
		}

		public void AppendToFile(TextWriter writer)
		{
			Guard.NotNull(writer, "writer");

			var sb =  new StringBuilder();
			writer.WriteLine("{0}\t{1}", Index, WriteTime(new StringBuilder(), Display.Days, 3, CultureInfo.InvariantCulture));
			writer.Flush();
		}

		public override string ToString() { return ToString(Display.Hours); }

		public string ToString(Display display, int precision = 3, CultureInfo culture = null)
		{
			culture = culture ?? CultureInfo.CurrentCulture;
			precision = Math.Min(3, Math.Max(1, precision));

			var sb = new StringBuilder();
			WriteTime(sb, display, precision, culture);

			if (sb.Length < 14)
			{
				var space = new String(' ', 11 + precision - sb.Length);
				sb.Insert(0, space);
			}
			sb.Insert(0, Index.ToString("000", culture) + ' ');

			return sb.ToString();
		}
		private StringBuilder WriteTime(StringBuilder sb, Display display, int precision, CultureInfo culture)
		{
			var endFormat = "#,##0";
			var midFormat = "00";
			var secformat = precision > 0 ? '.' + new String('0', precision) : "";
			var tSeperator = culture.DateTimeFormat.TimeSeparator;

			decimal s = (decimal)Time.Ticks / (decimal)TimeSpan.TicksPerSecond;
			var sec = Math.Round(s, precision);

			if (display == Display.Seconds || sec < 60)
			{
				sb.Append(sec.ToString(endFormat + secformat, culture));
				return sb;
			}
			else
			{
				sb.Append((sec % 60).ToString(midFormat + secformat, culture));
			}

			sb.Insert(0, tSeperator);
			int min = (int)sec / 60;

			if (display == Display.Minutes || min < 60)
			{
				sb.Insert(0, min.ToString(endFormat, culture));
				return sb;
			}
			else
			{
				sb.Insert(0, (min % 60).ToString(midFormat, culture));
			}

			sb.Insert(0, tSeperator);
			var hours = min / 60;

			if (display == Display.Hours || hours < 24)
			{
				sb.Insert(0, hours.ToString(endFormat, culture));
				return sb;
			}
			else
			{
				sb.Insert(0, (hours % 24).ToString(midFormat));
			}

			sb.Insert(0, tSeperator);
			var days = hours / 24;
			sb.Insert(0, days.ToString(endFormat));

			return sb;
		}

		[DebuggerBrowsable(DebuggerBrowsableState.Never), ExcludeFromCodeCoverage]
		private string DebuggerDisplay{get{return ToString(Display.Minutes, 3, CultureInfo.InvariantCulture); }}
	}
}
