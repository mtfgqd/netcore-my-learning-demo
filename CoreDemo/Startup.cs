using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using CoreDemo.IOC;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CoreDemo
{
    public class Startup
    {
        private readonly IHostEnvironment hostEnvironment;
        private readonly IWebHostEnvironment webHostEnvironment;

        /// <summary>
        /// IHostBuilder时，只能将以下服务类型注入 Startup 构造函数：
        /// IWebHostEnvironment 
        /// IHostEnvironment 
        /// IConfiguration
        /// </summary>
        /// <param name="configuration"></param>
        /// <param name="webHostEnvironment"></param>
        /// <param name="hostEnvironment"></param>
        public Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment, IHostEnvironment hostEnvironment)
        {
            Console.WriteLine("4 Startup");
            Configuration = configuration;
            //var builder = new ConfigurationBuilder().SetBasePath(hostEnvironment.ContentRootPath).AddJsonFile("appsettings.json",optional: true, reloadOnChange: true);
            //Configuration = builder.Build();
            this.webHostEnvironment = webHostEnvironment;
            this.hostEnvironment = hostEnvironment;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // Defines the Services used by your app (for example, ASP.NET Core MVC, Entity Framework Core, Identity)
        public void ConfigureServices(IServiceCollection services)
        {
            // 注入应用的组件/服务 如日志
            Console.WriteLine("4 Startup.ConfigureServices");

            services.AddDirectoryBrowser();//启用目录浏览

            //services.AddMvc();
            //services.AddAuthentication();
            //services.AddAuthorization();
            services.AddControllers();
            services.AddDbContext<DWQueueContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DemoDatabase")));
            //services.AddDbContext<DWQueueContext>(options => options.UseSqlServer(Configuration.GetConnectionString("Database")));

            //使用选项模式时的替代方法是绑定 Position 部分并将它添加到依赖项注入服务容器。
            services.Configure<ConfigDemo.PositionOptions>(Configuration.GetSection("Position"));

            //services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            //.AddEntityFrameworkStores<DWQueueContext>();

            #region 服务生存期 
            //服务生存期  https://docs.microsoft.com/zh-cn/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-3.1
            //暂时   暂时生存期服务 (AddTransient) 是每次从服务容器进行请求时创建的。 这种生存期适合轻量级、 无状态的服务。
            //范围内  作用域生存期服务 (AddScoped) 以每个客户端请求（连接）一次的方式创建。
            //单例   单一实例生存期服务 (AddSingleton) 是在第一次请求时（或者在运行 Startup.ConfigureServices 并且使用服务注册指定实例时）创建的。 每个后续请求都使用相同的实例。 如果应用需要单一实例行为，建议允许服务容器管理服务的生存期。 不要实现单一实例设计模式并提供用户代码来管理对象在类中的生存期。
            services.AddScoped<IMyDependency, MyDependency>();
            services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));//注册泛型类


            //TryAdd{ LIFETIME}       方法仅当尚未注册实现时，注册该服务。
            // The following line has no effect:因为 IMyDependency 已有一个已注册的实现
            services.TryAddSingleton<IMyDependency, DifMyDependency>();


            services.AddTransient<IOperationTransient, Operation>();
            services.AddScoped<IOperationScoped, Operation>();
            services.AddSingleton<IOperationSingleton, Operation>();
            services.AddSingleton<IOperationSingletonInstance>(new Operation(Guid.Empty));

            // OperationService depends on each of the other Operation types.
            services.AddTransient<OperationService, OperationService>();
            #endregion
        }

        private static void HandleMapTest1(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync("Map Test 1");
            });
        }

        private static void HandleMapTest2(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync("Map Test 2");
            });
        }
        private static void HandleMultiSeg(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                await context.Response.WriteAsync("Map multiple segments.");
            });
        }
        private static void HandleBranch(IApplicationBuilder app)
        {
            app.Run(async context =>
            {
                var branchVer = context.Request.Query["branch"];
                await context.Response.WriteAsync($"Branch used = {branchVer}");
            });
        }

        private void HandleBranchAndRejoin(IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                var branchVer = context.Request.Query["branch"];
                //_logger.LogInformation("Branch used = {branchVer}", branchVer);

                // Do work that doesn't write to the Response.
                await next();
                // Do other work that doesn't write to the Response.
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        // Defines the middleware for the request pipeline
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            #region 对中间件管道进行分支.Map 扩展用作约定来创建管道分支。Map 基于给定请求路径的匹配项来创建请求管道分支。 如果请求路径以给定路径开头，则执行分支。
            app.Map("/map1", HandleMapTest1);

            app.Map("/map2", HandleMapTest2);

            //Map 支持嵌套
            app.Map("/level1", level1App => {
                level1App.Map("/level2a", level2AApp => {
                    // "/level1/level2a" processing
                });
                level1App.Map("/level2b", level2BApp => {
                    // "/level1/level2b" processing
                });
            });
            //Map 还可同时匹配多个段
            app.Map("/map1/seg1", HandleMultiSeg);
            //MapWhen 基于给定谓词的结果创建请求管道分支
            app.MapWhen(context => context.Request.Query.ContainsKey("branch"),
                               HandleBranch);

            #endregion
            //UseWhen 也基于给定谓词的结果创建请求管道分支。 与 MapWhen 不同的是，如果这个分支不发生短路或包含终端中间件，则会重新加入主管道
            app.UseWhen(context => context.Request.Query.ContainsKey("branch"),
                             HandleBranchAndRejoin);

            #region autofac 解析出对象
            this.AutofacContainer = app.ApplicationServices.GetAutofacRoot();

            var servicenamed = this.AutofacContainer.Resolve<IMyService>();
            servicenamed.ShowCode();


            var service = this.AutofacContainer.ResolveNamed<IMyService>("service2");
            service.ShowCode();

            #region 子容器

            using (var myscope = AutofacContainer.BeginLifetimeScope("myscope"))
            {
                var service0 = myscope.Resolve<MyNameService>();
                using (var scope = myscope.BeginLifetimeScope())
                {
                    var service1 = scope.Resolve<MyNameService>();
                    var service2 = scope.Resolve<MyNameService>();
                    Console.WriteLine($"service1=service2:{service1 == service2}");
                    Console.WriteLine($"service1=service0:{service1 == service0}");
                }
            }
            #endregion
            #endregion
            //中间件顺序
            //https://docs.microsoft.com/zh-cn/aspnet/core/fundamentals/middleware/?view=aspnetcore-3.1


            // 注入中间件 处理HttpContext请求
            Console.WriteLine("5 Startup.Configure");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }


            app.UseHttpsRedirection();

            app.UseWebSockets();


            //要提供默认文件，必须在 UseStaticFiles 前调用 UseDefaultFiles。 
            //UseDefaultFiles 实际上用于重写 URL，不提供文件。 
            //通过 UseStaticFiles 启用静态文件中间件来提供文件。
            app.UseDefaultFiles();//在请求的文件夹中搜索default.htm、  default.html、index.htm、index.html
                                  //http://<server_address>/StaticFiles

            DefaultFilesOptions options = new DefaultFilesOptions();
            options.DefaultFileNames.Clear();
            options.DefaultFileNames.Add("mydefault.html");// 将mydefault.html设置为默认页
            app.UseDefaultFiles(options);

            ////wwwroot
            //    //css
            //    //images
            //    //js
            ////MyStaticFiles
            //    //images
            //        //banner1.svg
            app.UseStaticFiles(); // For the wwwroot folder
            //< img src = "~/images/banner1.svg" alt = "ASP.NET" class="img-responsive" />
            //等价于   wwwroot/images/banner1.svg 


            //http://<server_address>/StaticFiles/images/banner1.svg 前端请求的地址
            //应用重定向StaticFiles到MyStaticFiles
            //< img src = "~/StaticFiles/images/banner1.svg" alt = "ASP.NET" class="img-responsive" />
            //等价于    MyStaticFiles/images/banner1.svg 
            var cachePeriod = env.IsDevelopment() ? "600" : "604800";

            var provider = new FileExtensionContentTypeProvider();
            // Add new mappings
            provider.Mappings[".myapp"] = "application/x-msdownload";
            provider.Mappings[".htm3"] = "text/html";
            provider.Mappings[".image"] = "image/png";
            // Replace an existing mapping
            provider.Mappings[".rtf"] = "application/x-msdownload";
            // Remove MP4 videos.
            provider.Mappings.Remove(".mp4");

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "MyStaticFiles")),
                RequestPath = "/StaticFiles",
                OnPrepareResponse = ctx =>
                    {
                        // Requires the following import:
                        // using Microsoft.AspNetCore.Http;
                        ctx.Context.Response.Headers.Append("Cache-Control", $"public, max-age={cachePeriod}");
                    },
                ContentTypeProvider = provider
                //静态文件授权
                //var file = Path.Combine(Directory.GetCurrentDirectory(),
                //        "MyStaticFiles", "images", "banner1.svg");

                //return PhysicalFile(file, "image/svg+xml");
            });

            //启用目录浏览 出于安全考虑默认关闭
            //同时需要调用 Startup.ConfigureServices 中的services.AddDirectoryBrowser(); 方法来添加所需服务
            app.UseDirectoryBrowser(new DirectoryBrowserOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images")),
                RequestPath = "/MyImages"
            });


            //UseFileServer 结合了 UseStaticFiles、UseDefaultFiles 和 UseDirectoryBrowser（可选）的功能
            app.UseFileServer();//以下代码提供静态文件和默认文件。 未启用目录浏览。

            app.UseFileServer(enableDirectoryBrowsing: true);//通过启用目录浏览基于无参数重载进行构建：

            app.UseFileServer(new FileServerOptions
            {
                FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "MyStaticFiles")),
                RequestPath = "/StaticFiles",
                EnableDirectoryBrowsing = true
            });

            app.UseRouting();
            // app.UseRequestLocalization();
            // app.UseCors();
            app.UseAuthentication();

            app.UseAuthorization();
            // app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                                        name: "default",
                                        pattern: "{controller=Home}/{action=Index}/{id?}");
            });


            //Run 委托不会收到 next 参数。 第一个 Run 委托始终为终端，用于终止管道。
            //Run 是一种约定。 某些中间件组件可能会公开在管道末尾运行的 Run[Middleware] 方法
            //用 Use 将多个请求委托链接在一起。 next 参数表示管道中的下一个委托。 可通过不 调用 next 参数使管道短路。 
            app.Use(async (context, next) =>
            {
                // Do work that doesn't write to the Response.
                await next.Invoke();
                // Do logging or other work that doesn't write to the Response.
            });

            app.Run(async context =>
            {
                await context.Response.WriteAsync("Hello, World!");
            });

        }

        public ILifetimeScope AutofacContainer { get; private set; }

        /// <summary>
        /// autofac 会取代ConfigureServices中内置的依赖注入框架
        /// </summary>
        /// <param name="builder"></param>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            //builder.RegisterType<MyService>().As<IMyService>();
            #region 命名注册
            //builder.RegisterType<MyServiceV2>().Named<IMyService>("service2");
            #endregion

            #region 属性注册
            //builder.RegisterType<MyNameService>();
            //builder.RegisterType<MyServiceV2>().As<IMyService>().PropertiesAutowired();
            #endregion

            #region AOP
            builder.RegisterType<MyInterceptor>();
            builder.RegisterType<MyNameService>();
            builder.RegisterType<MyServiceV2>().As<IMyService>().PropertiesAutowired().InterceptedBy(typeof(MyInterceptor)).EnableInterfaceInterceptors();
            #endregion

            #region 子容器
            builder.RegisterType<MyNameService>().InstancePerLifetimeScope();
            #endregion

        }
    }
}
