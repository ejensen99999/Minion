using System;
using System.Collections.Generic;
using System.Linq;

namespace Minion.TestConsole.Test2
{
	public class FoodCubeTest
	{
		public static void Run(int iterations)
		{
			var start = DateTime.Now;
			var rand = new Random(DateTime.Now.Millisecond);
			var cube = new FoodCube();
			var list = new List<Order>();

			for (var i = 0; i < iterations; i++)
			{
				var userId = rand.Next(1000, 51000);
				var weekDay = (DayOfWeek)rand.Next(1, 7);
				var timeOfDay = TimeSpan.FromHours(rand.Next(7, 24));
				var cuisine = (CuisineType)rand.Next(1, 18);
				var food = (FoodType)rand.Next(1, 142);

				var order = new Order
				{
					UserId = userId,
					DayOfWeek = weekDay,
					TimeOfDay = timeOfDay,
					Cuisine = cuisine,
					Food = food
				};
				//
				cube[userId][weekDay][timeOfDay][cuisine][food].Set(order).Hit();

				//Not needed in regular use
				if (list.Count <= 2000)
				{
					list.Add(order);
				}

				if (i % 10000 == 0)
				{
					Console.WriteLine(i);
				}
			}

			Console.WriteLine("Load done: " + DateTime.Now.Subtract(start));

			Timer.Go("Find current preference", list.Take(1), x =>
			{
				var orders = cube[x.UserId][x.DayOfWeek][x.TimeOfDay].Slice();

				var topTen = orders
				    .OrderByDescending(y => y.Count)
				    .Take(10);

				DisplayTopTen(topTen);

				return orders.Count();
			});

			Console.ReadLine();
		}

		private static void DisplayTopTen(IEnumerable<dynamic> items)
		{

			foreach (var item in items)
			{
				var it = item.Data;

				var display = "";
				display += ((Order)it).UserId.ToString().PadRight(6);
				display += ((Order)it).DayOfWeek.ToString().PadRight(10);
				display += ((Order)it).TimeOfDay.ToString().PadRight(10);
				display += ((Order)it).Cuisine.ToString().PadRight(10);
				display += ((Order)it).Food.ToString().PadRight(15);
				display += item.Count.ToString().PadLeft(5);

				Console.WriteLine(display);
			}
		}
	}
}
