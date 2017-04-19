using System;

namespace Minion.Ioc.Aspects
{
	[System.AttributeUsage(AttributeTargets.Method)]
	public class MethodAspect: BaseAspect
	{
		public MethodAspect()
		{
		}

		public MethodAspect(int order)
			: base(order)
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
		
		public override bool OnMethodExecuting(AspectEventContext context, params object[] methodArgs)
		{
			return true;
		}

		public override void OnMethodExecuted(AspectEventContext context, params object[] methodArgs)
		{
		}

		public override object OnMethodReturning(AspectEventContext context, object returnValue)
		{
			return returnValue;
		}

		public override void OnMethodReturned(AspectEventContext context, object returnValue)
		{
		}
	}
}
