using Microsoft.AspNetCore.Builder;

namespace Minion.Ioc.Middleware
{
    public static class MinionIocTheadAsyncExtensions
    {
        public static IApplicationBuilder UseMinionIocThreaded(this IApplicationBuilder builder, Container container)
        {
            return builder.UseMiddleware<MinionIocTheadAsyncMiddleware>(container);
        }
    }
}