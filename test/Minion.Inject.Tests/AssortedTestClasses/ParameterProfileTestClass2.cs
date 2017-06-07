namespace Minion.Inject.Tests.AssortedTestClasses
{
    public class ParameterProfileTestClass2 :ParameterProfileTestClass1
    {
        public readonly IOrder _order;

        public ParameterProfileTestClass2(IClockEvent clockEvent, IOrder order)
            : base(clockEvent)
        {
            _order = order;
        }
    }
}