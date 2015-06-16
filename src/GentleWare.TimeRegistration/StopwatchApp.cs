using Qowaiv;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

namespace GentleWare.TimeRegistration
{

	public class StopwatchApp : Stopwatch, IDisposable
	{
		private StopwatchApp() { }
		protected StopwatchApp(TextWriter file, TextWriter console)
		{
			this.File = Guard.NotNull(file, "file");
			this.Console = Guard.NotNull(console, "console");
			this.Entries = new TimeEntries();

			this.Display = TimeEntry.Display.Hours;
			this.Precision = 2;
		}

		public TimeEntry.Display Display { get; set; }
		public Int32 Precision { get; set; }

		public DateTime StartTime { get; protected set; }

		protected TextWriter File { get; private set; }
		protected TextWriter Console { get; private set; }

		public TimeEntries Entries { get; private set; }

		protected void Run()
		{
			WriteHelp();

			ThreadPool.QueueUserWorkItem((waitpoint) =>
			{
				while (true)
				{
					if (IsRunning)
					{
						Entries.NewEntry(this).AppendToConsole(Console, Display, Precision);
					}
					Thread.Sleep(013);
				}
			});
		}

		protected void ClearConsole() { if (System.Console.Out == Console) { System.Console.Clear(); } }

		public void Apply(ConsoleKeyInfo cmd)
		{
			if (cmd.Key == ConsoleKey.F1) { WriteHelp(); }
			else if (cmd.KeyChar == 'c') { WriteHelp(); }
			else if (cmd.KeyChar == 'r') { RebuildConsole(Display, Precision); }
			else if (cmd.KeyChar == '1') { RebuildConsole(Display, 1); }
			else if (cmd.KeyChar == '2') { RebuildConsole(Display, 2); }
			else if (cmd.KeyChar == '3') { RebuildConsole(Display, 3); }

			else if (cmd.KeyChar == 's') { RebuildConsole(TimeEntry.Display.Seconds, Precision); }
			else if (cmd.KeyChar == 'm') { RebuildConsole(TimeEntry.Display.Minutes, Precision); }
			else if (cmd.KeyChar == 'h') { RebuildConsole(TimeEntry.Display.Hours, Precision); }
			else if (cmd.KeyChar == 'd') { RebuildConsole(TimeEntry.Display.Days, Precision); }

			else if (cmd.Key == ConsoleKey.Enter && IsRunning) { AddEntry(); }
			else if (cmd.Key == ConsoleKey.Enter && !IsRunning) { StartProcess(); }
		}

		protected void WriteHelp()
		{
			ClearConsole();
			Console.WriteLine("[enter]:    Start/Enter entry");
			Console.WriteLine("[c]:        Clear entries from screen");
			Console.WriteLine("[r]:        Show all entries to screen");
			Console.WriteLine("[1,2,3]:    Set precision");
			Console.WriteLine("[s,m,h,d]:  Set display (sec, min, hour, day)");
			Console.WriteLine();
			if (StartTime != DateTime.MinValue)
			{
				Console.WriteLine(GetStartTimeString());
				Console.WriteLine();
			}
		}

		private void StartProcess()
		{
			Start();
			StartTime = DateTime.Now;
			var str = GetStartTimeString();
			Console.WriteLine(str);
			Console.WriteLine();
			File.WriteLine(str);
		}

		private string GetStartTimeString()
		{
			return String.Format(CultureInfo.CurrentCulture, @"Started at {0:yyyy-MM-dd HH\:mm\:ss.fff}", StartTime);
		}

		protected void RebuildConsole(TimeEntry.Display display, int precision)
		{
			Display = display;
			Precision = precision;

			WriteHelp();

			foreach (var entry in Entries)
			{
				entry.AppendToConsole(Console, Display, Precision);
				Console.WriteLine();
			}
		}

		protected void AddEntry()
		{
			var entry = Entries.NewEntry(this);
			Entries.Add(entry);
			entry.AppendToConsole(Console, Display, Precision);
			Console.WriteLine();
			entry.AppendToFile(File);
			try
			{
				if (System.Console.BufferHeight < Entries.Count * 2)
				{
					System.Console.BufferHeight *= 2;
				}
			}
			catch { }
		}

		#region IDisposable

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		~StopwatchApp() { Dispose(false); }

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (File != null)
				{
					File.Dispose();
					File = null;
				}
			}
		}
		#endregion

		public static StopwatchApp Create(string logFileName)
		{
			Guard.NotNullOrEmpty(logFileName, "logFileName");
			var logFile = new FileInfo(logFileName);
			if (!logFile.Directory.Exists)
			{
				logFile.Directory.Create();
			}
			var file = new StreamWriter(logFile.FullName, true);
			var sw = new StopwatchApp(file, System.Console.Out);
			sw.Run();

			return sw;
		}
	}
}
