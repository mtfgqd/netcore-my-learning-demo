using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Log4NetDemo
{
    class Program
    {
        // Define a static logger variable so that it references the
        // Logger instance named "MyApp".
        private static readonly ILog log = LogManager.GetLogger("loginfo");//获取一个日志记录器

        static void Main(string[] args)
        {
            var fi = new System.IO.FileInfo("log4net.config");
            XmlConfigurator.Configure(fi);
            // Set up a simple configuration that logs on the console.

            var app = (Logger)log.Logger;

            var appender = (FileAppender)(app.Appenders[0]);
            Console.WriteLine(appender.File);
            log.Info("Entering application.");
            //Bar bar = new Bar();
            //bar.DoIt();
            log.Info("Exiting application.");
        }
    }
}
