using System.Collections.Generic;

namespace Minion.Tests.AssortedTestClasses
{
    public class ParameterProfileTestClass4: ParameterProfileTestClass3
    {
        public readonly List<string> _list;

        public ParameterProfileTestClass4(IClockEvent clockEvent, IOrder order, ICubeTestParams cube, List<string> list)
            :base(clockEvent, order, cube)
        {
            _list = list;
        }
    }
}