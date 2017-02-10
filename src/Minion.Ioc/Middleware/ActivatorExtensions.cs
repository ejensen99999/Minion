using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.OptionsModel;
using Minion.Ioc.Profiler;

namespace Minion.Ioc.Middleware
{
    public static class ActivatorExtensions
    {
        public static ILogger GetLog(this Container container)
        {
            var log = container.Get<ILoggerFactory>()?.CreateLogger("ActivatorExtensions");

            return log;
        }

        public static IServiceCollection AddMinionActivator(this IServiceCollection services, Container container)
        {
            var activator = new MinionIocActivator(container);
            services.AddSingleton<IControllerActivator>(activator);
            services.AddSingleton<IViewComponentActivator>(activator);

            container.Populate(services);

            return services;
        }

        public static Container AddConfiguration<TConfigure>(this Container container, IConfigurationRoot configuration)
            where TConfigure : class, new()
        {
            var section = configuration.GetSection(typeof(TConfigure).Name);
            var settings = new TConfigure();
            var config = new ConfigureFromConfigurationOptions<TConfigure>(section);

            config.Configure(settings);

            container.AddSingleton(settings);

            return container;
        }

        public static Container RegisterComponents(this Container container, IApplicationBuilder applicationBuilder)
        {
            container.RegisterMvcControllers(applicationBuilder);
            container.RegisterMvcViewComponents(applicationBuilder);

            return container;
        }

        public static void RegisterMvcControllers(this Container container, IApplicationBuilder applicationBuilder)
        {
            try
            {
                var controllerFeature = new ControllerFeature();
                var requiredService = ServiceProviderServiceExtensions
                    .GetRequiredService<ApplicationPartManager>(applicationBuilder.ApplicationServices);

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

        public static void RegisterMvcViewComponents(this Container container,
            IApplicationBuilder applicationBuilder)
        {
            try
            {
                var service = ServiceProviderServiceExtensions
                    .GetService<IViewComponentDescriptorProvider>(applicationBuilder.ApplicationServices);

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
                container.Profiler.SetMapping(type, type, Lifetime.Transient, null);
            }
        }

        public static Container Populate(this Container container, IServiceCollection services)
        {
            var profiler = container.Profiler;

            foreach (var service in services)
            {
                try
                {
                    var serviceType = service.ServiceType;
                    var instance = service.ImplementationInstance;
                    var instanceType = instance == null ? typeof(Nullable) : instance.GetType();
                    var lifetime = service.Lifetime.ToLifetime();

                    var serviceProvider = services.BuildServiceProvider();


                    if (service.ImplementationType != null)
                    {
                        if (IntrospectionExtensions.GetTypeInfo(service.ServiceType)
                            .IsGenericType)
                        {
                            profiler.SetMapping(serviceType, service.ImplementationType, lifetime,
                                x => serviceProvider.GetService(serviceType));
                        }
                    }
                    else if (service.ImplementationFactory != null)
                    {
                        profiler.SetMapping(serviceType, service.ImplementationType, lifetime,
                            x => service.ImplementationFactory(x));
                    }
                    else if (instanceType.InheritsFrom(serviceType))
                    {
                        profiler.SetMapping(serviceType, service.ImplementationType, lifetime,
                            x => service.ImplementationInstance);
                    }
                }
                catch (Exception ex)
                {
                    var log = container.Get<ILoggerFactory>()?.CreateLogger("ActivatorExtensions");
                    log?.LogError("Populate method found an issue with a registration", ex);
                }
            }

            return container;
        }

        public static bool InheritsFrom(this Type obj1, Type obj2)
        {
            bool output;

            var info1 = obj1.GetTypeInfo();

            output = info1.ImplementedInterfaces.Contains(obj2);

            return output;
        }
    }
}