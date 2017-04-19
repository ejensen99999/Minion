using System;

namespace Minion.Ioc.Aspects
{
	[AttributeUsage(AttributeTargets.ReturnValue)]
	public class MethodReturnAspect: BaseAspect
	{
		public MethodReturnAspect()
		{
		}

		public MethodReturnAspect(int order)
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

		public sealed override bool OnMethodExecuting(AspectEventContext context, params object[] methodArgs)
		{
			return true;
		}

		public sealed override void OnMethodExecuted(AspectEventContext context, params object[] methodArgs)
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
