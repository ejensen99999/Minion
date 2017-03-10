using Minion.Ioc.Aspects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Minion.Web.TestObjs
{
    public class MyPropAttribute : PropertyAspect
    {
        public MyPropAttribute(int order) : base(order)
        {
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
