using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Minion
{
    public abstract class BaseMiddleWare<TOutput>
    {
        protected readonly ILoggerFactory _logFactory;
        protected readonly RequestDelegate _next;
        protected readonly Type _serviceType;

        protected ILogger _logger;

        public BaseMiddleWare(ILoggerFactory logFactory, RequestDelegate next)
        {
            _logFactory = logFactory;
            _logger = logFactory.CreateLogger<TOutput>();
            _serviceType = typeof(TOutput);
            _next = next;
        }

        public abstract Task Invoke(HttpContext httpContext);
    }
}
