using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace OptionsDemo
{
    /// <summary>
    /// 
    /// 选项模式支持
    /// 1 单例模式读取配置
    /// 2 支持快照
    /// 3 支持变更通知
    /// 4 运行时动态修改配置项
    /// 设计时
    /// 1 使用XXXOptions
    /// 2 使用IOptions<XXXOptions>  
    /// 3 范围作用域使用IOptionsSnapshot<XXXOptions>  
    /// 4 单例服务使用IOptionsMonitor<XXXOptions> 作为构造参数
    ///     IOptions<TOptions>：

    /*IOptions<TOptions>：
        不支持：
        在应用启动后读取配置数据。
        命名选项
        注册为单一实例且可以注入到任何服务生存期。
    IOptionsSnapshot<TOptions>：
        在每次请求时应重新计算选项的方案中有用。 有关详细信息，请参阅使用 IOptionsSnapshot 读取已更新的数据。
        注册为范围内，因此无法注入到单一实例服务。
        支持命名选项
    IOptionsMonitor<TOptions>：
            用于检索选项并管理 TOptions 实例的选项通知。
            注册为单一实例且可以注入到任何服务生存期。
        支持：
            更改通知
            命名选项
            可重载配置
            选择性选项失效 (IOptionsMonitorCache<TOptions>)
    */

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
