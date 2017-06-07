using Microsoft.Extensions.DependencyInjection;

namespace Minion.Inject.Tests.AssortedTestClasses
{
    public class NoValidConstructor
    {
        public NoValidConstructor(ServiceLifetime life)
        {
        }
    }
}
