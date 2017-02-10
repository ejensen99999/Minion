using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Minion.TestConsole.Test4;
using Minion.Ioc;
using Minion.TestConsole.Emit;
using Minion.TestConsole.Test1;
using Minion.TestConsole.Test2;
using Minion.Ioc.Profiler;
using System.Reflection;

namespace Minion.TestConsole
{
	public class Program
	{
		public static void Main(string[] args)
		{
			//var iterations = 1000000;

			//CubePerspectiveTest.Run(iterations);

			//PresentationTest.Run(iterations);

			//HcTest.Run(iterations);

			//var cont = ContainerManager
			//    .GetContainer()
			//        .Add<ITestClass, TestClass>()
			//        .Add<IClockEvent, ClockEvent>(ServiceLifetime.Transient)
			//        .AddTransient<IOrder, Order>(x => new Order())
			//        .AddSingleton<ICubeTestParams, CubeTestParams>();

			//for (var i = 0; i < iterations; i++)
			//{
			//var t = cont.Get<ITestClass>();
			//}

			//var v = cont.Get<ITestClass>();

			//t.Clock.CompanyId = 123456798;
		    var ctors = typeof(TestClass).GetConstructors();
            var ctor = ctors.First();

		    var pp = new ParameterProfiler(ctor);
  

            var stuff = ConstructorEmitter.Emit(typeof(TestClass), pp);
			var t = stuff.Construct(new List<object> { DateTime.Now, 52, true});


			Console.ReadLine();
		}
	}
}
