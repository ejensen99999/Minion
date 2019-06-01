using System.Diagnostics;

namespace Minion.CP.Abstraction
{
	public static class HelpfulExtensions
	{
		public static bool IsExpired(this Stopwatch watch, long milliseconds)
		{
			var output = false;

			if (!watch.IsRunning)
			{
				watch.Start();
			}

			if (watch.ElapsedMilliseconds > milliseconds)
			{
				watch.Stop();
				output = true;
			}

			return output;
		}
	}
}
