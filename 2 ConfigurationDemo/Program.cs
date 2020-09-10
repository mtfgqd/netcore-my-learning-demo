using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace ConfigurationDemo
{
    class Program
    {
        public static IConfigurationBuilder builder;
        public static IConfigurationRoot configurationRoot;

        static void Main(string[] args)
        {
            // 配置整体来看类似于树形结构 root-section-element
            // 配置框架 4个接口  IConfiguration   IConfigurationRoot   IConfigurationSection   IConfigurationBuilder

            //扩展点
            //IConfigurationSource
            //IConfigurationProvider
            builder = new ConfigurationBuilder(); // 配置框架

            AddConfigrationItems();

            configurationRoot = builder.Build(); // 配置的根IConfigurationRoot
            IConfiguration config = configurationRoot;


            GetConfigrationItems();

            TraceConfigrationItemChange();
        }
        private static void AddConfigrationItems()
        {
            AddInMemoryConfigrationItems();
            AddEnvironmentConfigrationItems();
            AddFileConfigrationItems();
            AddCustomizedConfigration();
        }

        private static void GetConfigrationItems()
        {
            GetInMemoryConfigrationItems();
            GetEnvironmentConfigrationItems();
            GetFileConfigrationItems();
            GetFileConfigrationItemsFromBindingObject();
            GetCustomizedConfigration();
        }

        /// <summary>
        /// 用内存方式添加配置项
        /// </summary>
        private static void AddInMemoryConfigrationItems()
        {
            builder.AddInMemoryCollection(new Dictionary<string, string>()
            {
                { "InMemorykey1","InMemoryvalue1" },
                { "InMemorykey2","InMemoryvalue2" },
                { "InMemorysection1:InMemorykey4","InMemoryvalue4" },
                { "InMemorysection2:InMemorykey5","InMemoryvalue5" },
                { "InMemorysection2:InMemorykey6","InMemoryvalue6" },
                { "InMemorysection2:InMemorysection3:InMemorykey7","InMemoryvalue7" }
            });
        }
        private static void GetInMemoryConfigrationItems()
        {
            Console.WriteLine($"InMemorykey1:{configurationRoot["InMemorykey1"]}");
            Console.WriteLine($"InMemorykey1:{configurationRoot["InMemorykey2"]}");

            IConfigurationSection section = configurationRoot.GetSection("InMemorysection1");// 将配置分组
            Console.WriteLine($"InMemorykey4:{section["InMemorykey4"]}");
            Console.WriteLine($"InMemorykey5:{section["InMemorykey5"]}");


            IConfigurationSection section2 = configurationRoot.GetSection("InMemorysection2");
            Console.WriteLine($"InMemorykey5_v2:{section2["InMemorykey5"]}");
            var section3 = section2.GetSection("InMemorysection3");
            Console.WriteLine($"InMemorykey7:{section3["InMemorykey7"]}");
        }

        /// <summary>
        /// 用环境变量方式添加配置项
        /// </summary>
        private static void AddEnvironmentConfigrationItems()
        {
            builder.AddEnvironmentVariables();
            builder.AddEnvironmentVariables("TAO_");
        }

        private static void GetEnvironmentConfigrationItems()
        {
            Console.WriteLine($"EnvironmentKEY0:{configurationRoot["EnvironmentKEY0"]}");
            Console.WriteLine($"EnvironmentKEY1:{configurationRoot["EnvironmentKEY1"]}");
            Console.WriteLine($"EnvironmentKEY2:{configurationRoot["EnvironmentKEY2"]}");
        }
        /// <summary>
        /// 用配置文件方式添加配置项
        /// </summary>
        private static void AddFileConfigrationItems()
        {
            builder.AddJsonFile("appsettings.json", false, true);// 配置文件变化后重新加载
            builder.AddIniFile("appsettings.ini");
            builder.AddJsonFile("appsettings.Development.json");
        }

        private static void GetFileConfigrationItems()
        {
            Console.WriteLine($"jsonKey2:{configurationRoot["jsonKey2"]}");//被appsettings.Development.json中的覆盖
            Console.WriteLine($"iniKey3:{configurationRoot["iniKey3"]}");

            Console.WriteLine($"jsonKey1:{configurationRoot["jsonKey1"]}");// 根下无jsonKey1
            Console.WriteLine($"jsonKey2:{configurationRoot["jsonKey2"]}");// jsonKey2 在两个json中 但是被appsettings.Development.json中的覆盖
            Console.WriteLine($"iniKey3:{configurationRoot["iniKey3"]}");
        }

        /// <summary>
        /// 将配置项绑定到已有对象上
        /// 将配置项绑定到私有属性上
        /// </summary>
        private static void GetFileConfigrationItemsFromBindingObject()
        {
            var config = new Config()
            {
                Key1 = "config key1",
                Key5 = false
            };

            configurationRoot.GetSection("jsonOrderService").Bind(
                config,
                binderOptions => { binderOptions.BindNonPublicProperties = true; });//默认情况私有属性是不会被绑定
                                                                                    //需要将BindNonPublicProperties = true

            Console.WriteLine($"BindingObject:jsonOrderService:jsonKey1:{config.Key1}");
            Console.WriteLine($"BindingObject:jsonOrderService:jsonKey5:{config.Key5}");
            Console.WriteLine($"BindingObject:jsonOrderService:jsonKey6:{config.Key6}");
        }
        class Config
        {
            public string Key1 { get; set; }
            public bool Key5 { get; set; }
            public int Key6 { get; private set; } = 100;
        }

        /// <summary>
        /// 追踪配置文件发生变化
        /// </summary>
        private static void TraceConfigrationItemChange()
        {
            Console.WriteLine("开始了跟踪配置文件的变化了。。。。");

            #region 只跟踪一次配置发生变化
            IChangeToken token = configurationRoot.GetReloadToken();//跟踪配置发生变化
            token.RegisterChangeCallback(state =>
            {
                Console.WriteLine($"OneTimejsonKey6:{configurationRoot["jsonKey6"]}");
                token = configurationRoot.GetReloadToken(); // 获取新的token，IChangeToken对象只能使用一次，
                                                            //再使用的话需要重新获取新的token 然后再注册RegisterChangeCallback,成为一个无限循环的过程
                                                            //为解决这个问题演变出 一直跟踪配置变化
            }
            , configurationRoot);
            #endregion

            #region  一直跟踪配置变化
            ChangeToken.OnChange(() => configurationRoot.GetReloadToken(), () =>
            {
                Console.WriteLine($"jsonOrderService:jsonKey1:{configurationRoot["jsonOrderService:jsonKey1"]}");// 根下无jsonKey1
            });
            #endregion
            Console.ReadKey();
        }
        
        /// <summary>
        /// 用自定义数据源方式添加配置项
        /// </summary>
        private static void AddCustomizedConfigration()
        {
            builder.AddMyConfiguration();
        }

        private static void GetCustomizedConfigration()
        {
            ChangeToken.OnChange(() => configurationRoot.GetReloadToken(), () =>
            {
                Console.WriteLine($"lastTime:{configurationRoot["lastTime"]}");
            });

            Console.WriteLine("追踪CustomizedConfigration 开始了。。。。");
            Console.ReadKey();
        }
    }
}
