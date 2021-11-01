using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TokenAuthAPI.Middlewares
{
    public class RequestLogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLogMiddleware> _logger;

        public RequestLogMiddleware(RequestDelegate next, ILogger<RequestLogMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            context.Request?.EnableBuffering();

            var reader = new StreamReader(context.Request?.Body);
            string body = await reader.ReadToEndAsync();

            context.Request.Body.Position = 0;

            await _next(context);

            using (LogContext.PushProperty("Body-Content", body))
            {
                _logger.LogInformation($"Request {context.Request?.Method} {context.Request?.Path.Value} - HttpStatusCode {context.Response?.StatusCode}");
            }
        }
    }
}
