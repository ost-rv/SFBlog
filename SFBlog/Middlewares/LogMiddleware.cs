using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using System;
using Newtonsoft.Json;
using System.Net;

namespace SFBlog.Middlewares
{
   
    public class LogMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LogMiddleware> _logger;

        public LogMiddleware(RequestDelegate next, ILogger<LogMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            string userName = string.IsNullOrEmpty(httpContext.User.Identity.Name) ? "Anonymous" : httpContext.User.Identity.Name;
            
            _logger.LogInformation($"Метод: {httpContext.Request.Method}; Путь: {httpContext.Request.Path}; Пользователь: {userName};");
            await _next.Invoke(httpContext);
        }

    }

    // Метод расширения для добовление этого промежуточного ПО в конвеер обработки запроса
    public static class LogExtensions
    {
        public static IApplicationBuilder UseLog(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LogMiddleware>();
        }
    }
}
