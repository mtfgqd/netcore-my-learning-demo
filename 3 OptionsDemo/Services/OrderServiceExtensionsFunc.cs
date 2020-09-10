using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OptionsDemo.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OrderServiceExtensionsFunc
    {
        public static IServiceCollection AddOrderService(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<OrderServicePropertyExtension>(configuration.GetSection(OrderServicePropertyExtension.SectionName));

            #region 读取配置后在内存中对某个配置项再进行处理 先处理完之后再注册服务

            services.PostConfigure<OrderServicePropertyExtension>(options =>
            {
                options.MaxOrderCountExtension += 20;
            });

            services.AddSingleton<IOrderServiceExtension, OrderServiceExtension>();

            #endregion


            #region 方式1 添加验证 只能用AddOptions方法

            //services.AddOptions<OrderServiceOptionValidateProperty>().Configure(options =>
            //{
            //    configuration.GetSection(OrderServiceOptionValidateProperty.SectionName).Bind(options);//等价于services.Configure<OrderServiceOptions>(configuration);
            //                                                                                           //Configuration.GetSection("OrderServiceExtension")
            //}).Validate(options => { return options.MaxOrderCountValidate <= 800; }, "直接在Validate中定义校验方法 MaxOrderCount 不能大于800");
            //services.AddSingleton<IOrderServiceOptionValidate, OrderServiceOptionValidate>();


            #endregion

            #region 方式2 ValidateDataAnnotations 
            //services.AddOptions<OrderServiceOptionValidateProperty>().Configure(options =>
            //{
            //    configuration.GetSection(OrderServiceOptionValidateProperty.SectionName).Bind(options);
            //}).ValidateDataAnnotations();//用Annotations验证OrderServiceOptions中的字段
            //services.AddSingleton<IOrderServiceOptionValidate, OrderServiceOptionValidate>();
            #endregion

            #region 方式3  自定义校验类 1=2 等价
            // 1 
            services.AddOptions<OrderServiceOptionValidateProperty>().Configure(options =>
            {
                configuration.GetSection(OrderServiceOptionValidateProperty.SectionName).Bind(options);
            }).Services.AddSingleton<IValidateOptions<OrderServiceOptionValidateProperty>>(new OrderServiceOptionValidateOptions());
            services.AddSingleton<IOrderServiceOptionValidate, OrderServiceOptionValidate>();

            // 2
            //services.AddOptions<OrderServiceOptionProperty>().Configure(options =>
            //{
            //    configuration.Bind(options);
            //}).Services.AddSingleton<IValidateOptions<OrderServiceOptionValidateProperty>, OrderServiceOptionValidateOptions>();
            // services.AddSingleton<IOrderServiceOptionValidate, OrderServiceOptionValidate>();
            #endregion


            return services;
        }

        public static IServiceCollection AddOrderService(this IServiceCollection services, Action<OrderServiceOptionProperty> setup)
        {
            services.Configure<OrderServiceOptionProperty>(setup);
            services.AddScoped<IOrderService, OrderService>();
            return services;
        }


        public static IServiceCollection AddOrderServiceWithV(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<OrderServiceOptionProperty>(configuration);
            services.AddSingleton<IOrderService, OrderService>();
            return services;
        }
    }
}
