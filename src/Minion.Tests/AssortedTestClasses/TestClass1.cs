using System.Collections.Generic;

namespace Minion.Tests.AssortedTestClasses
{
    public interface ITestClass
    {
        IClockEvent Clock { get; }
        IOrder Order { get; }
        ICubeTestParams Params { get; }
        List<string> List { get; }
    }
    public class TestClass1: ITestClass
    {
        public IClockEvent Clock { get; }
        public IOrder Order { get; }
        public ICubeTestParams Params { get; }
        public List<string> List { get; }

        public TestClass1()
        {
        }

        public TestClass1(IClockEvent test1)
        {
            Clock = test1;
        }

        public TestClass1(IClockEvent test1, IOrder persp)
            :this(test1)
        {
            Order = persp;
        }

        public TestClass1(IClockEvent test1, IOrder persp, ICubeTestParams parameters)
            :this(test1, persp)
        {
            Params = parameters;
        }

        public TestClass1(IOrder persp, ICubeTestParams parameters, IClockEvent test1)
        : this(test1, persp)
        {
            Params = parameters;
        }

        public TestClass1(IOrder persp, ICubeTestParams parameters, IClockEvent test1, List<string> list)
        : this( persp, parameters, test1)
        {
            List = list;
        }

        public TestClass1(ICubeTestParams parameters, IClockEvent test1, IOrder persp)
        : this(test1, persp)
        {
            Params = parameters;
        }
    }
}
