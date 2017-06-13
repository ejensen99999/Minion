using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace Minion.Inject.Middleware
{
    public static class ActivatorExtensions
    {
        private static string _containerName;

        public static Container AddMinionInject(this IServiceCollection services, string containerName = null)
        {
            _containerName = containerName;
            var container = ContainerManager.GetContainer(_containerName);
            var activator = new MinionInjectActivator(container);
            services.AddSingleton<IControllerActivator>(activator);
            services.AddSingleton<IViewComponentActivator>(activator);

            container.Populate(services);
            container.RegisterMvcControllers();
            container.RegisterMvcViewComponents();

            return container;
        }

        public static IApplicationBuilder UseMinionInject(this IApplicationBuilder app)
        {
            var container = ContainerManager.GetContainer(_containerName);
            return app.UseMiddleware<MinionInjectScopedMiddleware>(container);
        }

        public static ILogger GetLog(this Container container)
        {
            var log = container.Get<ILoggerFactory>()?.CreateLogger("ActivatorExtensions");

            return log;
        }

        public static Container Populate(this Container container, IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            ILogger log = null;

            foreach (var service in services)
            {
                try
                {
                    var serviceType = service.ServiceType;
                    var instance = service.ImplementationInstance;
                    var instanceType = instance == null ? typeof(Nullable) : instance.GetType();

                    if (serviceType.GetTypeInfo().IsGenericType
                        && service.ImplementationType == null
                        && service.ImplementationFactory == null)
                    {
                        serviceType = serviceType.GetDefinedType();
                    }

                    if (service.ImplementationType != null)
                    {
                        container.AddService(serviceType, service.ImplementationType, service.Lifetime,
                            x => serviceProvider.GetService(serviceType), null);
                    }
                    else if (service.ImplementationFactory != null)
                    {
                        container.AddService(serviceType, service.ImplementationType, service.Lifetime,
                            x => service.ImplementationFactory(x), null);
                    }
                    else
                    {
                        container.AddService(serviceType, service.ImplementationType, service.Lifetime,
                            null,
                            service.ImplementationInstance);
                    }
                }
                catch (Exception ex)
                {
                    log = log ?? container.Get<ILoggerFactory>()?.CreateLogger("ActivatorExtensions");
                    log?.LogError("Populate method found an issue with a registration", ex);
                }
            }

            return container;
        }

        public static void RegisterMvcControllers(this Container container)
        {
            try
            {
                var controllerFeature = new ControllerFeature();
                var requiredService = container.Get<ApplicationPartManager>();

                requiredService.PopulateFeature(controllerFeature);

                RegisterTypes(container, controllerFeature.Controllers.Select(x => x.AsType()));
            }
            catch (Exception ex)
            {
                var log = container.GetLog();
                log.LogError("An error was thrown registering the mvc controllers", ex);
                throw;
            }
        }

        public static void RegisterMvcViewComponents(this Container container)
        {
            try
            {
                var service = container.Get<IViewComponentDescriptorProvider>();

                if (service == null)
                {
                    throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture,
                        "A registration for the {0} is missing from the ASP.NET Core configuration system.\n" +
                        "Make sure it is registered or pass it in using the RegisterMvcViewComponents overload that accepts {1}." +
                        "You can ensure that {1} is registered by either calling .AddMvc() or .AddViews() on the" +
                        "IServiceCollection class in the ConfigureServices method. Do note that calling .AddMvcCore() will not" +
                        "result in a registered {1}.",
                        typeof(IViewComponentDescriptorProvider).FullName,
                        typeof(IViewComponentDescriptorProvider).Name));
                }

                RegisterTypes(container, service.GetViewComponents()
                    .Select(x => x.TypeInfo.AsType()));
            }
            catch (Exception ex)
            {
                var log = container.GetLog();
                log.LogError("An error was thrown registering the mvc view components", ex);
                throw;
            }
        }

        private static void RegisterTypes(this Container container, IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                container.AddService(type, type, ServiceLifetime.Transient, null, null);
            }
        }


    }
}