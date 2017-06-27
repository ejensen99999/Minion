using Minion.Inject.Aspects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Minion.Web.Presentation
{
    public class AuditAspectAttribute: BaseAspect
    {

        public override void OnMethodExecuted(AspectEventContext context, params object[] methodArgs)
        {
            base.OnMethodExecuted(context, methodArgs);
        }

        public override bool OnMethodExecuting(AspectEventContext context, params object[] methodArgs)
        {
            var httpContext = new ControllerContext().HttpContext;

            return base.OnMethodExecuting(context, methodArgs);
        }

        public override void OnMethodReturned(AspectEventContext context, object returnValue)
        {
            base.OnMethodReturned(context, returnValue);
        }

        public override object OnMethodReturning(AspectEventContext context, object returnValue)
        {
            return base.OnMethodReturning(context, returnValue);
        }

        public override object OnPropertyGetting(AspectEventContext context, object returnValue)
        {
            return base.OnPropertyGetting(context, returnValue);
        }

        public override bool OnPropertySetting(AspectEventContext context, object valueObject)
        {
            return base.OnPropertySetting(context, valueObject);
        }
    }
}
