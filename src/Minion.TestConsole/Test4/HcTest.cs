using System;
using System.Collections.Generic;
using System.Linq;
using Minion.TestConsole.Test1;
using Minion.DataControl;

namespace Minion.TestConsole.Test4
{
	public class HcTest
	{
		private static DateTime _time = DateTime.Now;

		public static void Run(int iterations)
		{
			var start = DateTime.Now;

            var cube = new HyperCube<ClockEvent>(
				x => x.CompanyId,
				x => x.TimeCard,
				x => x.SupervisorId,
				x => x.EmployeeId);

			var count = 0;
			var list = new List<ClockEvent>();
			var rand = new Random(DateTime.Now.Millisecond);

			var sids = new []
			{
				Guid.NewGuid(),
				Guid.NewGuid(),
				Guid.NewGuid(),
				Guid.NewGuid(),
				Guid.NewGuid(),
				Guid.NewGuid(),
				Guid.NewGuid(),
				Guid.NewGuid(),
				Guid.NewGuid(),
				Guid.NewGuid()
			};

			var eids = new []
			{
				Guid.NewGuid(),
				Guid.NewGuid(),
				Guid.NewGuid(),
				Guid.NewGuid(),
				Guid.NewGuid(),
				Guid.NewGuid(),
				Guid.NewGuid(),
				Guid.NewGuid(),
				Guid.NewGuid(),
				Guid.NewGuid()
			};

			for (var i = 0; i < iterations; i++)
			{
				var cid = rand.Next(100, 5100); //5000
				var sid = sids[rand.Next(0, 10)]; //10
				var eid = eids[rand.Next(0, 10)]; //10
				var date = DateTime.Now.AddHours(0 - (rand.Next(1, 8760)));
				var ppDay = 15 <= date.Day ? 14 : 1;
				var payperiod = new DateTime(date.Year, date.Month, ppDay); //24
				//12 million possible combinations of data

				if (i % 10000 == 0)
				{
					Console.WriteLine(i);
				}

				var ev = new ClockEvent
				{
					ClockId = i,
					CompanyId = cid,
					SupervisorId = sid,
					EmployeeId = eid,
					Type = ClockEventTypes.BreakStart,
					Event = date,
					TimeCard = payperiod
				};

                list.Add(ev);
				cube.Load(ev);

				count++;
			}

			var samples = list.Take(1000).ToList();

            Console.WriteLine("Total time: " + DateTime.Now.Subtract(start) + ", Count: " + count);

			//Dim1(samples, cube, list);
			//Dim2(samples, cube, list);
			Dim3(samples, cube, list);
		}

		public static void Dim1(List<ClockEvent> samples, HyperCube<ClockEvent> cube, List<ClockEvent> list)
		{
			//Cube data
			Timer.Go("Dimension 1 hypercube", samples, x =>
			{
				var results = cube.Slice(x, 
					y => y.CompanyId);

				return results.Count();
			});

			//List data
			Timer.Go("Dimension 1 list search", samples, x =>
			{
				var results = list.Where(y => y.CompanyId == x.CompanyId)
					.ToList();

				return results.Count;
			});
		}

		public static void Dim2(List<ClockEvent> samples, HyperCube<ClockEvent> cube, List<ClockEvent> list)
		{
			//Cube data
			Timer.Go("Dimension 2 hypercube", samples, x =>
			{
				var results = cube.Slice(x,
					y => y.CompanyId,
					y => y.TimeCard);

				return results.Count();
			});

			//List data
			Timer.Go("Dimension 2 list search", samples, x =>
			{
				var results = list
					.Where(y => y.CompanyId == x.CompanyId && y.TimeCard == x.TimeCard)
					.ToList();

				return results.Count;
			});
		}

		public static void Dim3(List<ClockEvent> samples, HyperCube<ClockEvent> cube, List<ClockEvent> list)
		{
			//Cube data
			Timer.Go("Dimension 3 hypercube", samples, x =>
			{
				var results = cube.Slice(x,
					y => y.SupervisorId,
					y => y.TimeCard,
					y => y.CompanyId);

				return results.Count();
			});

			//List data
			Timer.Go("Dimension 3 list search", samples, x =>
			{
				var results = list
				    .Where(y => y.CompanyId == x.CompanyId && y.TimeCard == x.TimeCard && y.SupervisorId == x.SupervisorId)
					 .ToList();

				return results.Count;
			});
		}
	}
}

