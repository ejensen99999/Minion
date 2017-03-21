using Microsoft.AspNetCore.Builder;

namespace Minion.Ioc.Middleware
{
    public static class MinionIocTheadAsyncExtensions
    {
        public static IApplicationBuilder UseMinionThreadedIoc(this IApplicationBuilder builder, Container container)
        {
            return builder.UseMiddleware<MinionIocTheadAsyncMiddleware>(container);
        }
    }
}