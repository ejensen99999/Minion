using Minion.Inject.Aspects;

namespace Minion.Web.TestObjs
{
    public class MyMethodAspect2Attribute : MethodExecutionAspect
    {
        public MyMethodAspect2Attribute(int order) : base(order)
        {
        }

        public override void OnMethodExecuted(AspectEventContext context, params object[] methodArgs)
        {
            base.OnMethodExecuted(context, methodArgs);
        }

        public override bool OnMethodExecuting(AspectEventContext context, params object[] methodArgs)
        {
            return base.OnMethodExecuting(context, methodArgs);
        }
    }
}
