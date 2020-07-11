using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CoreDemo
{
    /// <summary>
    /// IStartupFilter
    /// 在应用的 Configure 中间件管道的开头或末尾配置中间件，而无需显式调用 Use{Middleware}。 
    /// IStartupFilter 由 ASP.NET Core 用于将默认值添加到管道的开头，而无需使应用作者显式注册默认中间件。
    /// IStartupFilter 允许代表应用作者使用不同的组件调用 Use{Middleware}。
    /// 创建 Configure 方法的管道。 IStartupFilter.Configure 可以将中间件设置为在库添加的中间件之前或之后运行
    /// </summary>
    public class StartupFilterDemo
    {
    }

    public class RequestSetOptionsMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestSetOptionsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        // Test with https://localhost:5001/Privacy/?option=Hello
        public async Task Invoke(HttpContext httpContext)
        {
            var option = httpContext.Request.Query["option"];

            if (!string.IsNullOrWhiteSpace(option))
            {
                httpContext.Items["option"] = WebUtility.HtmlEncode(option);
            }

            await _next(httpContext);
        }
    }
    public class RequestSetOptionsStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                builder.UseMiddleware<RequestSetOptionsMiddleware>();
                next(builder);
            };
        }
    }
}
