using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Logging;

namespace DevTrace.UI.Middleware
{
    public class DevTraceUIMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly StaticFileMiddleware _staticFileMiddleware;

        private const string EmbeddedFileNamespace = "DevTrace.UI.wwwroot";

        public DevTraceUIMiddleware(RequestDelegate next, 
            Microsoft.AspNetCore.Hosting.IWebHostEnvironment env,
            ILoggerFactory loggerFactory)
        {
            _next = next;

            var staticFileOptions = new StaticFileOptions
            {
                RequestPath = "/devtrace",
                FileProvider = new EmbeddedFileProvider(typeof(DevTraceUIMiddleware).GetTypeInfo().Assembly, EmbeddedFileNamespace)
            };

            _staticFileMiddleware = new StaticFileMiddleware(next, env, Options.Create(staticFileOptions), loggerFactory);
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value;

            if (Regex.IsMatch(path, @"^/devtrace(/)?$", RegexOptions.IgnoreCase))
            {
                context.Response.Redirect("/devtrace/index.html");
                return;
            }

            await _staticFileMiddleware.Invoke(context);
        }
    }
}