using System;

namespace GentleWare.TimeRegistration
{
	public class Program
	{
		private static bool KeepRunning = true;

		public static void Main(string[] args)
		{
			Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);

			using(var app = StopwatchApp.Create("time.log"))
			{
				while (KeepRunning)
				{
					app.Apply(Console.ReadKey(true));
				}
			}
		}

		public static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
		{
			Console.WriteLine();
			Console.WriteLine("Stopped...");
			KeepRunning = false;
		}
	}
}
