using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ExceptionDemo.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ExceptionDemo
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(mvcOptions =>
            {
                //4 在MVC controller的中间件内的异常进行处理，而不是对中间件整体的异常处理，区别于前三种
                mvcOptions.Filters.Add<MyExceptionFilter>();//对所有controller都适用的全局异常处理

                //MyExceptionFilterAttribute也实现了IExceptionFilter也可注册为全局的，也可以只添加在要处理的controller上 实现更细粒度的控制
                mvcOptions.Filters.Add<MyExceptionFilterAttribute>();
            }).AddJsonOptions(jsonoptions =>
            {
                jsonoptions.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                //1 开发人员异常页 显示请求异常的详细信息。置于要捕获其异常的任何中间件前面
                //该页包括关于异常和请求的以下信息：
                //堆栈跟踪
                //查询字符串参数（如果有）
                //Cookie（如果有）
                //标头
                app.UseDeveloperExceptionPage();
            }
            else
            {
                //2 异常处理程序页
                //为生产环境配置自定义错误处理页，请使用异常处理中间件。
                //中间件
                //捕获并记录异常。
                //在备用管道中为指定的页或控制器重新执行请求。 如果响应已启动，则不会重新执行请求。
                app.UseExceptionHandler("/error");
                app.UseHsts();
            }

            app.UseExceptionHandler(errApp =>
            {
                errApp.Run(async context =>
                {
                    //3 访问异常lambda
                    //使用 IExceptionHandlerPathFeature 访问错误处理程序控制器或页中的异常和原始请求路径：
                    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                    IKnownException knownException = exceptionHandlerPathFeature.Error as IKnownException;
                    if (knownException == null)
                    {
                        var logger = context.RequestServices.GetService<ILogger<MyExceptionFilterAttribute>>();
                        logger.LogError(exceptionHandlerPathFeature.Error, exceptionHandlerPathFeature.Error.Message);
                        knownException = KnownException.Unknown;
                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    }
                    else
                    {
                        knownException = KnownException.FromKnownException(knownException);
                        context.Response.StatusCode = StatusCodes.Status200OK;
                    }
                    var jsonOptions = context.RequestServices.GetService<IOptions<JsonOptions>>();
                    context.Response.ContentType = "application/json; charset=utf-8";
                    await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(knownException, jsonOptions.Value.JsonSerializerOptions));
                });
            });

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
