using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.FileProviders;
namespace StaticFilesDemo
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
            //services.AddDirectoryBrowser();//3 允许访问静态文件目录  在Configure中也需要加入  app.UseDirectoryBrowser();
            // 但是要去掉 app.UseDefaultFiles();对默认页的设置
        }
        const int BufferSize = 64 * 1024;
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            //app.UseDirectoryBrowser();//中间件调用顺序 去掉访问默认页设置,否则会直接到默认页上

            //2 设置默认访问页: index/default
            //app.UseDefaultFiles();


            //1 定义两个静态文件目录时,会先找wwwroot找不到再找file,但是定义了StaticFileOptions 的path之后,只找file的映射目录

            app.UseStaticFiles();

            //1.1 映射出file目录
            app.UseStaticFiles(new StaticFileOptions
            {
               // RequestPath = "/files",// RequestPath 是url地址， 缺省该参数，默认映射到站点根地址，请求 https://localhost:5001，将请求映射到file文件夹
                                       //定义了RequestPath之后 请求 https://localhost:5001/files, 将映射到file文件夹
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "file")) // 文件系统中的文件结构，物理文件路径 file/page.html
            });

            //app.UseFileServer();

            app.MapWhen(context =>
            {
                return !context.Request.Path.Value.StartsWith("/api");// 不是API的请求都重定向到 index.html
            }, appBuilder =>
            {
                var option = new RewriteOptions();
                option.AddRewrite(".*", "/index.html", true);
                appBuilder.UseRewriter(option);

                appBuilder.UseStaticFiles(); // 然后再使用静态文件

                //直接输出文件的方式中的HttpRequest 的header是不一样的
                //appBuilder.Run(async c =>
                //{
                //    var file = env.WebRootFileProvider.GetFileInfo("index.html");

                //    c.Response.ContentType = "text/html";
                //    using (var fileStream = new FileStream(file.PhysicalPath, FileMode.Open, FileAccess.Read))
                //    {
                //        await StreamCopyOperation.CopyToAsync(fileStream, c.Response.Body, null, BufferSize, c.RequestAborted);
                //    }
                //});
            });



            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
