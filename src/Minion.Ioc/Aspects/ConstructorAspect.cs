using System;

namespace Minion.Ioc.Aspects
{
	[AttributeUsage(AttributeTargets.Constructor)]
	public class ConstructorAspect: BaseAspect
	{
		public ConstructorAspect()
		{
		}

		public ConstructorAspect(int order): base(order)
		{
		}

		public sealed override bool OnPropertySetting(AspectEventContext context, object valueObject)
		{
			return true;
		}

		public sealed override object OnPropertyGetting(AspectEventContext context, object returnValue)
		{
			return returnValue;
		}

		public sealed override bool OnMethodExecuting(AspectEventContext context, params object[] methodArgs)
		{
			return true;
		}

		public sealed override void OnMethodExecuted(AspectEventContext context, params object[] methodArgs)
		{
		}

		public sealed override object OnMethodReturning(AspectEventContext context, object returnValue)
		{
			return returnValue;
		}

		public sealed override void OnMethodReturned(AspectEventContext context, object returnValue)
		{
		}
	}
}
