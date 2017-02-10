using System;
using System.Collections.Generic;
using System.Linq;

namespace Minion.TestConsole.Test1
{
	public class CubePerspectiveTest
	{
		public static void Run(int iterations)
		{
			var start = DateTime.Now;
			var cube = new CubePerspective();
			var list = new List<ClockEvent>();
			var rand = new Random(DateTime.Now.Millisecond);

			var sids = new Guid[]
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

			var eids = new Guid[]
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
                cube[ev.CompanyId][ev.TimeCard][ev.SupervisorId][ev.EmployeeId].Add(ev);
			}

			var samples = list.Take<ClockEvent>(1000).ToList();

			Console.WriteLine("Total time: " + DateTime.Now.Subtract(start));
		    Console.ReadLine();

			//Dim1(samples, cube, list);
			//Dim2(samples, cube, list);
			Dim3(samples, cube, list);

			Console.ReadLine();
		}

		public static void Dim1(List<ClockEvent> samples, CubePerspective cube, List<ClockEvent> list)
		{
			//Cube Test
			Timer.Go<ClockEvent>("Dimension 1 perspective", samples, x =>
			{
				var results = cube[x.CompanyId].Slice();

				return results.Count();
			});

			//List Test
			Timer.Go<ClockEvent>("Dimension 1 list search", samples, x =>
			{
				var results = list.Where<ClockEvent>(y => y.CompanyId == x.CompanyId)
					.ToList();

				return results.Count;
			});
		}

		public static void Dim2(List<ClockEvent> samples, CubePerspective cube, List<ClockEvent> list)
		{
			//Cube data
			Timer.Go<ClockEvent>("Dimension 2 perspective", samples, x =>
			{
				var results = cube[x.CompanyId][x.TimeCard].Slice();

				return results.Count();
			});

			//List data
			Timer.Go<ClockEvent>("Dimension 2 list search", samples, x =>
			{
				var results = list
					.Where<ClockEvent>(y => y.CompanyId == x.CompanyId && y.TimeCard == x.TimeCard)
					.ToList();

				return results.Count;
			});
		}

		public static void Dim3(List<ClockEvent> samples, CubePerspective cube, List<ClockEvent> list)
		{
			//Cube data
			Timer.Go<ClockEvent>("Dimension 3 perspective", samples, x =>
			{
				var results = cube[x.CompanyId][x.TimeCard][x.SupervisorId].Slice();
				//var results = cube.Find(x.CompanyId, x.TimeCard, x.SupervisorId).Slice();

				return results.Count();
			});

			//List data
			Timer.Go<ClockEvent>("Dimension 3 list search", samples, x =>
			{
				var results = list
				    .Where<ClockEvent>(y => y.CompanyId == x.CompanyId && y.TimeCard == x.TimeCard && y.SupervisorId == x.SupervisorId)
					 .ToList();

				return results.Count;
			});
		}
	}
}