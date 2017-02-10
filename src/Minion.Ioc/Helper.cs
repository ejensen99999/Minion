using System;
using Microsoft.Extensions.DependencyInjection;

namespace Minion.Ioc
{
    public static class Helper
    {

        public static TContract Get<TContract>(this IServiceProvider provider)
        {
            var output = provider.GetService(typeof (TContract));

            return (TContract) output;
        }

        public static ServiceLifetime ToServiceLifetime(this Lifetime life)
        {
            var output = ServiceLifetime.Transient;

            switch (life)
            {
                case Lifetime.Scoped:
                    output = ServiceLifetime.Scoped;
                    break;
                case Lifetime.Singleton:
                    output = ServiceLifetime.Singleton;
                    break;
                case Lifetime.ThreadAsync:
                case Lifetime.Transient:
                default:
                    output = ServiceLifetime.Transient;
                    break;
            }

            return output;
        }

        public static Lifetime ToLifetime(this ServiceLifetime life)
        {
            var output = Lifetime.Transient;

            switch (life)
            {
                case ServiceLifetime.Scoped:
                    output = Lifetime.ThreadAsync;
                    break;
                case ServiceLifetime.Singleton:
                    output = Lifetime.Singleton;
                    break;
                case ServiceLifetime.Transient:
                default:
                    output = Lifetime.Transient;
                    break;
            }

            return output;
        }
    }
}

