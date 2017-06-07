namespace Minion.Inject.Tests.AssortedTestClasses
{
    public class ParameterProfileTestClass1
    {
        public readonly IClockEvent _clockEvent;

        public ParameterProfileTestClass1(IClockEvent clockEvent)
        {
            _clockEvent = clockEvent;
        }
    }
}