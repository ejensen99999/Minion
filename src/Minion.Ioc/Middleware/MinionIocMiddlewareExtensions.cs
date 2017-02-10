using Microsoft.AspNetCore.Builder;

namespace Minion.Ioc.Middleware
{
    public static class MinionIocMiddlewareExtensions
    {
        public static IApplicationBuilder UseMinionIoc(this IApplicationBuilder builder, Container container)
        {
            return builder.UseMiddleware<MinionIocMiddleware>(container);
        }
    }
}