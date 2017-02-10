using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;

namespace Minion.Ioc.Middleware
{
    public class MinionIocActivator : IControllerActivator, IViewComponentActivator
    {
        private readonly Container _container;

        public MinionIocActivator(Container container)
        {
            _container = container;
        }

        public object Create(ControllerContext context)
        {
            return _container.GetThreadedService(context.ActionDescriptor.ControllerTypeInfo.AsType());
        }

        public object Create(ViewComponentContext context)
        {
            return _container.GetThreadedService(context.ViewComponentDescriptor.TypeInfo.AsType());
        }

        public void Release(ControllerContext context, object controller)
        {
        }

        public void Release(ViewComponentContext context, object viewComponent)
        {
        }
    }
}
