using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace Minion.Inject
{
    public static class ContainerExtensions
    {
        public static Container AddConfiguration<TConfigure>(this Container container, IConfigurationRoot configuration)
            where TConfigure : class, new()
        {
            var section = configuration.GetSection(typeof(TConfigure).Name);
            var settings = new TConfigure();
            var config = new ConfigureFromConfigurationOptions<TConfigure>(section);

            container.AddTransient<TConfigure, TConfigure>(x =>
            {
                config.Configure(settings);
                return settings;
            });

            return container;
        }


        public static Container Add<TContract, TConcrete>(this Container container,
            ServiceLifetime lifetime,
            Func<Container, object> initializer,
            dynamic instance)
        {
            container.AddService(typeof(TContract), typeof(TConcrete), lifetime, initializer, instance);

            return container;
        }

        public static Container AddScoped<TContract, TConcrete>(this Container container,
            Func<Container, object> initializer = null,
            dynamic instance = null)
        {
            container.AddService(typeof(TContract), typeof(TConcrete), ServiceLifetime.Scoped, initializer, instance);

            return container;
        }

        public static Container AddSingleton<TContract, TConcrete>(this Container container,
                    Func<Container, object> initializer = null,
                    dynamic instance = null)
        {
            container.AddService(typeof(TContract), typeof(TConcrete), ServiceLifetime.Singleton, initializer, instance);

            return container;
        }

        public static Container AddTransient<TContract, TConcrete>(this Container container,
                    Func<Container, object> initializer = null,
                    dynamic instance = null)
        {
            container.AddService(typeof(TContract), typeof(TConcrete), ServiceLifetime.Transient, initializer, instance);

            return container;
        }

        public static TContract Get<TContract>(this Container container)
            where TContract : class
        {
            var output = container.GetService(typeof(TContract));

            return (TContract)output;
        }

        public static dynamic GetScoped<TContract>(this Container container)
        {
            return container.GetScoped(typeof(TContract));
        }

        public static dynamic GetScoped(this Container container, Type serviceType)
        {
            var output = default(object);

            try
            {
                container.SetContextId();
                output = container.GetService(serviceType);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                container.ClearContextId();
            }

            return output;
        }
    }
}
