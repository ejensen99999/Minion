using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Minion.Ioc.Middleware
{
    public class MinionIocMiddleware : BaseMiddleWare<MinionIocMiddleware>
    {
        public MinionIocMiddleware(ILoggerFactory logFactory, RequestDelegate next, Container container)
            : base(logFactory, next)
        {
        }

        public override async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next.Invoke(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            finally
            {
                
            }
        }
    }
}
