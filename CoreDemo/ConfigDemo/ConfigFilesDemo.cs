using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreDemo.ConfigDemo
{
    public static class ConfigFilesDemo
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            //builder.AddJsonFile("appsettings.Development.json");
            //builder.AddJsonFile("appsettings.ini");
            var configurationRoot = builder.Build();

            var config = new Config()
            {
                Key1 = "config key1",
                Key5 = false
            };



            configurationRoot.GetSection("OrderService").Bind(config,
                binderOptions => { binderOptions.BindNonPublicProperties = true; });

            Console.WriteLine($"Key1:{config.Key1}");
            Console.WriteLine($"Key5:{config.Key5}");
            Console.WriteLine($"Key6:{config.Key6}");



            //监控配置文件变更
            IChangeToken token = configurationRoot.GetReloadToken();
            token.RegisterChangeCallback(state =>
            {
                Console.WriteLine($"Key1:{configurationRoot["Key1"]}");
                Console.WriteLine($"Key2:{configurationRoot["Key2"]}");
                Console.WriteLine($"Key3:{configurationRoot["Key3"]}");
            }, configurationRoot);


            ChangeToken.OnChange(() => configurationRoot.GetReloadToken(), () =>
            {
                Console.WriteLine($"Key1:{configurationRoot["Key1"]}");
                Console.WriteLine($"Key2:{configurationRoot["Key2"]}");
                Console.WriteLine($"Key3:{configurationRoot["Key3"]}");
            });
            Console.WriteLine("开始了");

            Console.ReadKey();
            //Console.WriteLine($"Key1:{configurationRoot["Key1"]}");
            //Console.WriteLine($"Key2:{configurationRoot["Key2"]}");
            //Console.WriteLine($"Key3:{configurationRoot["Key3"]}");
            Console.ReadKey();
        }
        static void CheckChangeToken(this IConfigurationRoot configurationRoot)
        {
            var token1 = configurationRoot.GetReloadToken();

            var token2 = configurationRoot.GetReloadToken();

            Console.WriteLine($"token1==token2: {token1 == token2}");

            token1.RegisterChangeCallback(data =>
            {
                var token3 = configurationRoot.GetReloadToken();
                Console.WriteLine("配置发生了变化");
                Console.WriteLine($"token1==token3: {token1 == token3}");

            }, null);
        }
        public static void Change(this IConfigurationRoot configurationRoot)
        {
            ChangeToken.OnChange(() => configurationRoot.GetReloadToken(), () =>
            {
                Console.WriteLine("配置发生了改变，新的配置值为：");

                Console.WriteLine($"Key1:{configurationRoot["Key1"]}");
                Console.WriteLine($"Key2:{configurationRoot["Key2"]}");
                Console.WriteLine($"Key3:{configurationRoot["Key3"]}");
                Console.WriteLine($"Key4:{configurationRoot["Key4"]}");
            });
        }


        public static void BindDemo(this IConfigurationRoot configurationRoot)
        {
            var myconfig = new MyConfig();


            //绑定私有属性
            configurationRoot.Bind(myconfig, option => { option.BindNonPublicProperties = false; });

            Console.WriteLine($"MyConfig.Key1:{myconfig.Key1}");
            Console.WriteLine($"MyConfig.Key5:{myconfig.Key5}");
            Console.WriteLine($"MyConfig.Key6:{myconfig.Key6}");
        }       
    }
    class MyConfig
    {
        public string Key1 { get; set; } = "default";
        public bool Key5 { get; set; } = true;
        public int Key6 { get; private set; } = 20;
    }
    class Config
    {
        public string Key1 { get; set; }
        public bool Key5 { get; set; }
        public int Key6 { get; private set; } = 100;
    }
}
