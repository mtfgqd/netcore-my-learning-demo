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
using DependencyInjectionDemo.Services;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace DependencyInjectionDemo
{
    public class Startup
    {
        //1 服务的注册方式 直接注入 工厂方式 try方式 泛型 替换和移除
        //2 服务的生命周期 单例 作用域 瞬时
        //3 服务的获取方式 在controller里

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region 注册服务不同生命周期的服务
            services.AddSingleton<IMySingletonService, MySingletonService>();//根容器内是单例

            //作用域：容器或者子容器的生存周期内是单例的，容器被释放掉，对象也会被释放掉
            services.AddScoped<IMyScopedService, MyScopedService>();

            services.AddTransient<IMyTransientService, MyTransientService>();// 瞬时
            #endregion

            #region 花式注册 1、手动创建实例后注入 2、容器注入 3、工厂方法
            services.AddSingleton<ISingletonOrderService>(new SingletonOrderService());  //手动创建对象，放入容器,容器不负责释放
            services.AddSingleton<ISingletonOrderService, SingletonOrderServiceEx>(); //由容器负责创建

            // 工厂方式注入
            services.AddSingleton<ISingletonOrderService>(serviceProvider =>
            {
                // 可以从容器中获取子元素，去拼装工厂方法的实例
                var dependencyClass = serviceProvider.GetService<IMyTransientService>();
                return new SingletonOrderServiceFactory();
            });

            services.AddScoped<IScopeOrderService>(serviceProvider =>
            {
                return new ScopeOrderService();
            });
            #endregion

            #region 尝试注册
            //服务（接口）注册过就不再注册了
            services.TryAddSingleton<ISingletonOrderService, SingletonOrderServiceEx>();

            // 只要服务（接口）的实现不同就会被注册，相同的实现类不会被注册
            services.TryAddEnumerable(ServiceDescriptor.Singleton<ISingletonOrderService, SingletonOrderServiceTryAdd>());
            //services.TryAddEnumerable(ServiceDescriptor.Singleton<ISingletonOrderService, SingletonOrderServiceTryAdd>());
            //services.TryAddEnumerable(ServiceDescriptor.Singleton<ISingletonOrderService>(new SingletonOrderServiceTryAdd()));
            services.TryAddEnumerable(ServiceDescriptor.Singleton<IOrderService>(p =>
            {
                return new OrderService();
            }));
            #endregion

            #region 移除和替换注册
            // OrderServiceEx 会替换掉IOrderService的实现
            services.Replace(ServiceDescriptor.Singleton<IOrderService, OrderServiceEx>());

            //删除所有IScopeOrderService的实现
            services.RemoveAll<IScopeOrderService>();
            #endregion

            #region 注册泛型模板
            services.AddSingleton(typeof(IGenericService<>), typeof(GenericService<>));
            #endregion




            #region 对象释放行为
            // 实现IDisposeable接口的对象，
            // 1 容器只释放由其创建的对象，由我们自己创建后被放进容器的，容器不负责释放
            // 2 在容器或者子容器被释放时，才会释放由其创建的对象
            // 3 完全让容器创建和释放对象
            // 4 不要在根容器中创建被注册为瞬时服务的对象

            //TransitDispose 接口，每个对象会被分别释放,但是不要在根容器中创建对象
            services.AddTransient<ITransitDisposableOrderService, TransitDisposableOrderService>();

            //ScopeDispose 接口，每个作用域内都只能获取一个相同的对象，接口执行完毕后几个作用域就释放几个对象
            services.AddScoped<IScopeDisposableOrderService>(p => new ScopeDisposableOrderService());

            //SingletonDispose 接口，整个生命周期内只有一个对象，接口执行完毕后对象也不会被释放
            services.AddSingleton<ISingletonDisposableOrderService, SingletonDisposableOrderService>();

            #region DisposeLifeTime接口 自己创建实例后注册，容器不会帮我们管理对象的生命周期
            var service = new DisposableOrderService();
            services.AddSingleton<IDisposableOrderService>(service);
            #endregion

            #endregion

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //从根容器中获取瞬时服务，所有这些从根容器中获取的服务都会驻留在根容器中，
            //而根容器中的对象只会在应用程序关闭时回收，所以这种操作会很耗内存
            //var dd = app.ApplicationServices.GetService<ITransitDisposableOrderService>();


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
