using System;
using System.Collections.Generic;

namespace Minion.TestConsole
{
	public class Timer
	{
		public static void Go(string testName, int iterations, Func<int> test)
		{
			var total = 0;
			var start = DateTime.Now;
			for (var i = 0; i < iterations; i++)
			{
				total += test();
			}

			Console.WriteLine(testName + ": " + DateTime.Now.Subtract(start));
			Console.WriteLine(testName + " total: " + total);

		}

		public static void Go<TObject>(string testName, IEnumerable<TObject> items, Func<TObject, int> test)
		{
			var total = 0;
			var start = DateTime.Now;
			foreach (var item in items)
			{
				total += test(item);
			}

			Console.WriteLine(testName + ": " + DateTime.Now.Subtract(start));
			Console.WriteLine(testName + " total: " + total);

		}
	}
}
