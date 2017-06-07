using System;

namespace Minion.Inject.Tests.AssortedTestClasses
{
    public interface ICubeTestParams
    {
    }

    public class CubeTestParams: ICubeTestParams
	{
		public static DateTime PayPeriod { get; private set; }

		static CubeTestParams()
		{
			PayPeriod = DateTime.Now.AddMonths(-2);
		}
	}
}
