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
using Microsoft.AspNetCore.Http;
namespace MiddlewareDemo
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
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// <summary>
        /// 中间件的注册和调用过程，最早注册的使用范围最广，最早发挥作用,注意注册顺序
        /// https://docs.microsoft.com/zh-cn/aspnet/core/fundamentals/middleware/?view=aspnetcore-3.1
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //用 Use 将多个请求委托链接在一起。 next 参数表示管道中的下一个委托。 可通过不调用 next 参数使管道短路。
            app.Use(async (context, next) =>
            {
                //在向客户端发送响应后，请勿调用 next.Invoke。 响应启动后，针对 HttpResponse 的更改将引发异常。
                //如果向客户端发送响应，向Response头中写了hello，调用next()会报错。可以通过context.Response.HasStarted做判断，判断是否开始向客户端发送响应
                // await context.Response.WriteAsync("Hello");后不能再调用  await next()
                //await context.Response.WriteAsync("Hello");
                await next();
                if (context.Response.HasStarted)
                {
                    //一旦已经开始输出，则不能再修改响应头的内容
                }
                await context.Response.WriteAsync("Hello Use");
            });

            // Map 对特殊的路由进行处理 等价于 context.Request.Query.Keys.Equals("abc")
            app.Map("/abc", abcBuilder =>
            {
                // Use 类似于 app.Use
                abcBuilder.Use(async (context, next) =>
                {
                    await next();
                    await context.Response.WriteAsync("Hello Map");
                });
            });

            // MapWhen 对满足某个特定条件的请求进行处理
            app.MapWhen(context =>
            {
                return context.Request.Path.Value.Contains("bbc");//路由中包含
                //return context.Request.Query.Keys.Contains("abc");// query string 中包好
            }, builder =>
            {
                //run 是中间件调用链的末端，不会再调用后续中间件
                builder.Run(async context =>
                {
                    await context.Response.WriteAsync("Hello MapWhen" + context.Request.Path.Value);
                });

            });


            app.UseMyMiddleware();//自定义中间件


            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            #region
            //app.Use(async (context, next) =>
            //{
            //    await next();
            //    await context.Response.WriteAsync("Hi ");
            //});


            //app.UseMyMiddleware();

            //app.Run(async context =>
            //{
            //    await context.Response.WriteAsync("Hello");
            //});

            //app.Map("/abc", builder =>
            //{


            //});

            //app.MapWhen(context => context.Request.IsHttps, builder =>
            //{

            //    builder.Run(async context2 => {

            //        await context2.Response.WriteAsync("is https");

            //    });
            //});
            #endregion
        }
    }
}
