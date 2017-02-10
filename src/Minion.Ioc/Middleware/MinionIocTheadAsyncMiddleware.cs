using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Minion.Ioc.Middleware
{
    public class MinionIocTheadAsyncMiddleware : BaseMiddleWare<MinionIocTheadAsyncMiddleware>
    {
        private readonly Container _container;

        public MinionIocTheadAsyncMiddleware(ILoggerFactory logFactory, RequestDelegate next, Container container)
            : base(logFactory, next)
        {
            _container = container;
        }

        public override async Task Invoke(HttpContext httpContext)
        {
            try
            {
                _container.SetContextId();
                await _next.Invoke(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
            }
            finally
            {
                _container.ClearContextId();
            }
        }
    }
}
