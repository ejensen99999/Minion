namespace Minion.Inject.Tests.AssortedTestClasses
{
    public class ParameterProfileTestClass3: ParameterProfileTestClass2
    {
        public readonly ICubeTestParams _cube;

        public ParameterProfileTestClass3(IClockEvent clockEvent, IOrder order, ICubeTestParams cube)
            :base(clockEvent, order)
        {
            _cube = cube;
        }
    }
}