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
using Microsoft.Extensions.Primitives;
using OptionsDemo.Services;

namespace OptionsDemo
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
            #region 普通方式注入
            services.AddSingleton<OrderServiceProperty>();
            services.AddSingleton<IOrderService, OrderService>();
            #endregion

            #region 选项方式注入，使用Configure方法，只关心配置项的值是多少，不关心配置项从哪来
            services.Configure<OrderServiceOptionProperty>(Configuration.GetSection(OrderServiceOptionProperty.SectionName));
            services.AddSingleton<IOrderServiceOption, OrderServiceOption>();

            //->IOptionsSnapshot 范围作用域使用IOptionsSnapshot 只能注册为作用域的，注册为单例的会报错
            services.Configure<OrderServiceOptionPropertySnapshot>(Configuration.GetSection(OrderServiceOptionPropertySnapshot.SectionName));
            services.AddScoped<IOrderServiceOptionSnapshot, OrderServiceOptionSnapshot>();

            //->IOptionsMonitor 单例服务使用IOptionsMonitor
            services.Configure<OrderServiceOptionPropertyMonitor>(Configuration.GetSection(OrderServiceOptionPropertyMonitor.SectionName));
            services.AddSingleton<IOrderServiceOptionMonitor, OrderServiceOptionMonitor>();
          

            ChangeToken.OnChange(() => Configuration.GetReloadToken(), () =>
            {
                Console.WriteLine("重新加载配置");
            });
            #endregion

            //AddOrderService 作为IServiceCollection的扩展方法，将IConfiguration作为扩展方法的参数
            services.AddOrderService(Configuration);

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
