using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Minion.Inject.Middleware
{
    public class MinionInjectScopedMiddleware : BaseMiddleWare<MinionInjectScopedMiddleware>
    {
        private readonly Container _container;

        public MinionInjectScopedMiddleware(ILoggerFactory logFactory, RequestDelegate next, Container container)
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
