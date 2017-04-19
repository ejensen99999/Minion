using System;

namespace Minion.Ioc.Aspects
{
	[System.AttributeUsage(AttributeTargets.Property)]
	public class PropertyAspect : BaseAspect
	{
		public PropertyAspect()
		{
		}

		public PropertyAspect(int order)
			: base(order)
		{
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

		public override bool OnPropertySetting(AspectEventContext context, object valueObject)
		{
			return true;
		}

		public override object OnPropertyGetting(AspectEventContext context, object returnValue)
		{
			return returnValue;
		}
	}
}
