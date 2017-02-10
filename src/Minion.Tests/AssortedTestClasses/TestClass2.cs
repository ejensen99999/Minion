using Minion.Ioc;
using System.Collections.Generic;

namespace Minion.Tests.AssortedTestClasses
{
    public class TestClass2: ITestClass
    {
        public IClockEvent Clock { get; }
        public IOrder Order { get; }
        public ICubeTestParams Params { get; }
        public List<string> List { get; }

        public TestClass2()
        {
        }

        public TestClass2(IClockEvent test1)
        {
            Clock = test1;
        }

        public TestClass2(IClockEvent test1, IOrder persp)
            :this(test1)
        {
            Order = persp;
        }

        public TestClass2(IClockEvent test1, IOrder persp, ICubeTestParams parameters)
            :this(test1, persp)
        {
            Params = parameters;
        }

        [PreferredConstructor]
        public TestClass2(IOrder persp, ICubeTestParams parameters, IClockEvent test1)
        : this(test1, persp)
        {
            Params = parameters;
        }

        public TestClass2(IOrder persp, ICubeTestParams parameters, IClockEvent test1, List<string> list)
        : this( persp, parameters, test1)
        {
            List = list;
        }

        public TestClass2(ICubeTestParams parameters, IClockEvent test1, IOrder persp)
        : this(test1, persp)
        {
            Params = parameters;
        }
    }
}
