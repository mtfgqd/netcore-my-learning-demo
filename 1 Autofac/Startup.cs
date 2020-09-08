using System;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;
using DependencyInjectionAutofacDemo.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace _1_Autofac
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // 默认的容器
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddControllersAsServices();
        }

        /// 第三方IOC容器服务注册
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.RegisterType<MyService>().As<IMyService>();//一般注入

            #region 命名注册
            builder.RegisterType<NamedService>().Named<IMyService>("NamedService");
            #endregion

            #region 属性注册
            builder.RegisterType<WiredProperty>();//注册类型,PropertyAutoWiredService中使用到了WiredProperty，需要先将其注入
            builder.RegisterType<PropertyAutoWiredService>().As<IMyServiceV2>().PropertiesAutowired();
            #endregion

            #region AOP 在不改变原有逻辑的基础上，在执行的切面上添加逻辑
            builder.RegisterType<MyInterceptor>(); //1 注册拦截器
            builder.RegisterType<AOPService>().As<IAOPService>()
                .PropertiesAutowired()//需要属性注入的时候
                .InterceptedBy(typeof(MyInterceptor)) // 使用的拦截器
                .EnableInterfaceInterceptors();//启用接口拦截器（常用）服务的类型是接口   还有类拦截器
            #endregion

            #region 子容器 给子容器命名为myscope，在其他的子容器中是获取不到这个服务的
            builder.RegisterType<ScopedService>().InstancePerMatchingLifetimeScope("myscope");
            #endregion

        }
        public ILifetimeScope AutofacContainer { get; private set; }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            this.AutofacContainer = app.ApplicationServices.GetAutofacRoot();

            var service = this.AutofacContainer.Resolve<IMyService>();//使用一般服务
            service.ShowCode();

            #region 使用命名服务
            var namedService = this.AutofacContainer.ResolveNamed<IMyService>("NamedService");
            namedService.ShowCode();
            #endregion

            #region 使用属性注入的服务
            var autoWiredService = this.AutofacContainer.Resolve<IMyServiceV2>();
            autoWiredService.ShowCode();
            #endregion

            #region AOP 使用拦截器
            var aopService = this.AutofacContainer.Resolve<IAOPService>();//使用一般服务
            aopService.ShowCode();
            #endregion

            #region 使用子容器
            //子容器名为myscope，在这个子容器以及这个子容器的子容器得到的都是同一个对象，在其他的子容器中是获取不到这个服务的,
            using (var myscope = this.AutofacContainer.BeginLifetimeScope("myscope"))
            {
                var service0 = myscope.Resolve<ScopedService>();
                using (var scope = myscope.BeginLifetimeScope())
                {
                    var service1 = scope.Resolve<ScopedService>();
                    var service2 = scope.Resolve<ScopedService>();
                    Console.WriteLine($"service1=service2:{service1 == service2}"); //true
                    Console.WriteLine($"service1=service0:{service1 == service0}"); //true
                }
            }
            #endregion


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

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
