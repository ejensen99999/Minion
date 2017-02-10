using System;
using Microsoft.Extensions.Logging;

namespace Minion
{
	public abstract class BaseService //: IHealthCheck
	{
		protected readonly ILogger<CoreConfiguration> _log;

		protected readonly ICoreConfiguration _config;

		public BaseService(ILogger<CoreConfiguration> log, ICoreConfiguration config)
		{
			_log = log;
			_config = config;
		}

		//public virtual HealthList HealthCheck()
		//{
		//	HealthList healthList;
		//	try
		//	{
		//		IHealthLogic instance = CommonCache.Instance.Container.GetInstance<IHealthLogic>();
		//		string str = string.Concat(GetType().FullName, "_healthcheck");
		//		HealthList healthList1 = CommonCache.Instance.TryGet<HealthList>(str, () => instance.GenerateHealthCheck(str), TimeSpan.FromSeconds(10), false);
		//		healthList = healthList1;
		//	}
		//	catch (Exception exception)
		//	{
		//		_log.Error("HealthCheck survey failed", exception);
		//		throw;
		//	}
		//	return healthList;
		//}
	}
}