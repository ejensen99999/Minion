using Minion.Inject.Aspects;

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
