using System;

namespace Minion.Emit.Aspects
{
	[AttributeUsage(AttributeTargets.All, AllowMultiple=true)]
	public class BaseAspect : Attribute
	{
		public int Order = 99;
		
		public BaseAspect()
		{
			Order = 99;
		}

		public BaseAspect(int order)
		{
			Order = order;
		}

		public virtual bool OnMethodExecuting(AspectEventContext context, params object[] methodArgs)
		{
			return true;
		}

		public virtual object OnMethodReturning(AspectEventContext context, object returnValue)
		{
			return returnValue;
		}

		public virtual bool OnPropertySetting(AspectEventContext context, object valueObject)
		{
			return true;
		}

		public virtual object OnPropertyGetting(AspectEventContext context, object returnValue)
		{
			return returnValue;
		}

		public virtual void OnMethodExecuted(AspectEventContext context, params object[] methodArgs)
		{

		}

		public virtual void OnMethodReturned(AspectEventContext context, object returnValue)
		{

		}
	}
}