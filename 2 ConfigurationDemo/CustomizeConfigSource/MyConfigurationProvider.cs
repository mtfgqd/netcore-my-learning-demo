using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;

namespace ConfigurationCustom
{
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
            this.Data["lastTime"] = DateTime.Now.ToString();      //Data 是放配置数据的   
                                                                  // 这里的数据可以来自任何地方
                                                                  //可以集成   阿波罗
            if (reload)
            {
                base.OnReload();
            }
        }


    }
}
