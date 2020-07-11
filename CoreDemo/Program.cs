using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore;
using System.IO;
using CoreDemo.ConfigDemo;
using CoreDemo.IOC;
using Autofac.Extensions.DependencyInjection;

namespace CoreDemo
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //调用 CreateHostBuilder 方法以创建和配置生成器对象。
            //对生成器对象调用 Build 和 Run 方法
            var host = CreateHostBuilder(args).Build();

            using (var serviceScope = host.Services.CreateScope())
            {
                var services = serviceScope.ServiceProvider;

                try
                {
                    //使用 IServiceScopeFactory.CreateScope 创建 IServiceScope 以解析应用范围内的已设置范围的服务。
                    //此方法可以用于在启动时访问有作用域的服务以便运行初始化任务。
                     var serviceContext = services.GetRequiredService<OperationService>();
                    // Use the context here
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred.");
                }
            }

            host.Run();
        }

        // 关于应用程序的执行顺序 
        // 1(ConfigureWebHostDefaults)->2(ConfigureHostConfiguration)->3(ConfigureAppConfiguration)-4(ConfigureServices/ConfigureLogging/Startup->Startup.ConfigureServices)->5(Startup.Configure)
        // 至于 ConfigureServices 和 Startup->Startup.ConfigureServices的执行顺序取决于webBuilder.UseStartup<Startup>()的书写位置
        // 如果webBuilder.UseStartup<Startup>()先于ConfigureServices那么Startup->Startup.ConfigureServices先执行
        // 但是两个方法的作用是一样的都是注入应用的组件所以在Startup.ConfigureServices中去配置服务就可以了
        // 在Program.CreateHostBuilder()中也可以直接ConfigureServices() 和 Configure() 如下面的示例
        public static IHostBuilder CreateHostBuilder(string[] args) =>
              Host.CreateDefaultBuilder(args)// 按照以下顺序为应用提供默认配置：
                                             // ChainedConfigurationProvider：添加现有 IConfiguration 作为源。 在默认配置示例中，添加主机配置，并将它设置为应用 配置的第一个源。
                                             //使用 JSON 配置提供程序通过 appsettings.json 提供。
                                             //使用 JSON 配置提供程序通过 appsettings.Environment.json 提供 。 例如，appsettings.Production.json 和 appsettings.Development.json 。
                                             //应用在 Development 环境中运行时的应用机密。
                                             //使用环境变量配置提供程序通过环境变量提供。
                                             //使用命令行配置提供程序通过命令行参数提供。
                                             //实例代码详见CoreDemo.Model.ConfigurationDemo.cs
             .UseServiceProviderFactory(new AutofacServiceProviderFactory())//注册第三方容器的入口
             .ConfigureWebHostDefaults(webBuilder =>
             {
                 Console.WriteLine("1 ConfigureWebHostDefaults");
                 // 应用程序必要组件如配置/容器
                 webBuilder.UseStartup<Startup>();
                 #region webBuilder.UseStartup<Startup>() 调用Startup.ConfigureServices()和Startup.Configure()
                 //// 等价于Startup.ConfigureServices()
                 //webBuilder.ConfigureServices(services =>
                 //{
                 //    Console.WriteLine();
                 //    services.AddControllers();
                 //});
                 //// 等价于Startup.Configure()
                 //webBuilder.Configure(app =>
                 //{
                 //    app.UseHttpsRedirection();

                 //    app.UseRouting();

                 //    app.UseAuthorization();

                 //    app.UseEndpoints(endpoints =>
                 //    {
                 //        endpoints.MapControllers();
                 //    });
                 //});
                 #endregion
             })
            .ConfigureHostConfiguration(builder =>
            {
                Console.WriteLine("2 ConfigureHostConfiguration");
                // 应用程序启动时必要的配置如监听的端口/URL
            })
            .ConfigureAppConfiguration((hostingContext, config) =>
            {
                Console.WriteLine("3 ConfigureAppConfiguration");
                // 嵌入配置文件供应用程序读取

                config.Sources.Clear();

                var env = hostingContext.HostingEnvironment;

                //IniConfigurationProvider 在运行时从 INI 文件键值对加载配置
                //config.AddIniFile("MyIniConfig.ini", optional: true, reloadOnChange: true)
                //     .AddIniFile($"MyIniConfig.{env.EnvironmentName}.ini",optional: true, reloadOnChange: true);


                //XmlConfigurationProvider 在运行时从 XML 文件键值对加载配置
                //config.AddXmlFile("MyXMLFile.xml", optional: true, reloadOnChange: true)
                //     .AddXmlFile($"MyXMLFile.{env.EnvironmentName}.xml",optional: true, reloadOnChange: true);


                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                      .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);

                config.AddJsonFile("MyConfig.json", optional: true, reloadOnChange: true)
                      .AddJsonFile($"MyConfig.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);


                //AddEFConfiguration 为自定义的配置提供程序        using CoreDemo.ConfigDemo;
                //config.AddEFConfiguration(options => options.UseInMemoryDatabase("InMemoryDb"));

                config.AddEnvironmentVariables();

                //MyConfig.json 和 MyConfig.Environment.json 文件中的设置 ：
                //会替代 appsettings.json 和 appsettings.Environment.json 文件中的设置 。
                //会被环境变量配置提供程序和命令行配置提供程序中的设置所替代。

                //KeyPerFileConfigurationProvider 使用目录的文件作为配置键值对。 该键是文件名。 该值包含文件的内容。 Key-per-file 配置提供程序用于 Docker 托管方案。
                var path = Path.Combine(Directory.GetCurrentDirectory(), "path/to/files");
                config.AddKeyPerFile(directoryPath: path, optional: true);

                var Dict = new Dictionary<string, string>
                {
                     {"MyKey", "Dictionary MyKey Value"},
                     {"Position:Title", "Dictionary_Title"},
                     {"Position:Name", "Dictionary_Name" },
                     {"Logging:LogLevel:Default", "Warning"}
                };
                config.AddInMemoryCollection(Dict);

                if (args != null)
                {
                    config.AddCommandLine(args);
                }
            })
            .ConfigureServices(services =>
            {
                Console.WriteLine("4 ConfigureServices");
                //注册 IStartupFilter
                services.AddTransient<IStartupFilter,RequestSetOptionsStartupFilter>();
            });

        public static IWebHost BuildWebHost(string[] args) =>
                        WebHost.CreateDefaultBuilder(args)
                        .UseStartup<Startup>()
                        .Build();
    }
}