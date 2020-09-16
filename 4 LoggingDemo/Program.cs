using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Compact;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace LoggingDemo
{
    public class Program
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
           .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
           .AddEnvironmentVariables()
           .Build();

        // 日志级别
        // 日志过滤配置
        // 日志对象获取
        // 日志记录的方法
        public static void Main(string[] args)
        {
            // LogDemo(args);
            ShowSerilog(args);
        }

        private static void LogDemo(string[] args)
        {
            MySimpleLog(args);
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        public static void MySimpleLog(string[] args)
        {
            IConfigurationBuilder configBuilder = new ConfigurationBuilder();
            configBuilder.AddCommandLine(args);
            configBuilder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            var config = configBuilder.Build();

            IServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IConfiguration>(p => config); //用工厂模式将配置对象注册到容器管理          
            serviceCollection.AddLogging(builder =>
            {
                builder.AddConfiguration(Configuration.GetSection("Logging"));
                builder.AddConsole();
            });


            serviceCollection.AddTransient<OrderService>();

            IServiceProvider service = serviceCollection.BuildServiceProvider();
            ShowLogUseInjection(service);
            ShowLogUseILogger(service);
            ShowLogWithScope(service);

        }

        private static void ShowLogUseInjection(IServiceProvider service)
        {
            var order = service.GetService<OrderService>();// use logger in service 推荐使用强类型的方式注入logger
            order.Show();
        }

        private static void ShowLogUseILogger(IServiceProvider service)
        {
            ILoggerFactory loggerFactory = service.GetService<ILoggerFactory>();
            ILogger alogger = loggerFactory.CreateLogger("alogger");
            alogger.LogInformation(2001, "aiya");// 设置Event ID为2001打印出Event ID 和日志信息
            alogger.LogInformation("hello");
            var ex = new Exception("出错了");
            alogger.LogError(ex, "出错了");
            var alogger2 = loggerFactory.CreateLogger("alogger2");
            alogger2.LogDebug("aiya");
        }

        //一个事务包含多个操作
        //复杂流程日志关联
        //调用链追踪与请求处理过程对应
        private static void ShowLogWithScope(IServiceProvider service)
        {
            var logger = service.GetService<ILogger<Program>>();

            // 配置文件中加入  "IncludeScopes": true,
            while (Console.ReadKey().Key != ConsoleKey.Escape)
            {
                using (logger.BeginScope("ScopeId:{scopeid}", Guid.NewGuid()))
                {
                    logger.LogInformation("这是Info");
                    logger.LogError("这是Error");
                }
                Console.WriteLine("+++++++++++++++");
            }

            logger.LogInformation(new EventId(201, "xihuaa"), "world!");
        }

        /// <summary>
        /// 实现日志告警
        /// 实现上下文的关联
        /// 实现与追踪集成
        /// </summary>
        private static int ShowSerilog(string[] args)
        {
            Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(Configuration)
          .MinimumLevel.Debug()
          .Enrich.FromLogContext()
          .WriteTo.Console(new RenderedCompactJsonFormatter())
          .WriteTo.File(formatter: new CompactJsonFormatter(), "logs\\myapp.txt", rollingInterval: RollingInterval.Day)
          .CreateLogger();
            try
            {
                Log.Information("Starting web host");
                CreateHostBuilder(args).UseSerilog(dispose: true).Build().Run();//UseSerilog(dispose: true) 使用中间件 替换系统日志框架
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
