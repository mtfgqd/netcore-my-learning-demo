using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace CoreDemo.ConfigDemo
{
    class MyConfigurationSource : IConfigurationSource
    {
        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new MyConfigurationProvider();
        }
    }

    class MyConfigurationProvider : ConfigurationProvider
    {

        Timer timer;

        public MyConfigurationProvider() : base()
        {
            timer = new Timer();
            timer.Elapsed += Timer_Elapsed;
            timer.Interval = 3000;
            timer.Start();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Load(true);
        }

        public override void Load()
        {
            //加载数据
            Load(false);
        }

        void Load(bool reload)
        {
            this.Data["lastTime"] = DateTime.Now.ToString();
            if (reload)
            {
                base.OnReload();
            }
        }


    }

    public static class MyConfigurationBuilderExtensions
    {
        public static IConfigurationBuilder AddMyConfiguration(this IConfigurationBuilder builder)
        {
            builder.Add(new MyConfigurationSource());
            return builder;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder();
            
            builder.AddMyConfiguration();//=>builder.Add(new MyConfigurationSource());

            var configRoot = builder.Build();

            ChangeToken.OnChange(() => configRoot.GetReloadToken(), () =>
            {
                Console.WriteLine($"lastTime:{configRoot["lastTime"]}");
            });

            Console.WriteLine("开始了");
            Console.ReadKey();
        }
    }
}
