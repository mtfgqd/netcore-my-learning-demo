using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoggingDemo
{
    public class OrderService
    {
        ILogger<OrderService> _logger; //推荐使用强类型的方式注入logger
        public OrderService(ILogger<OrderService> logger)
        {
            _logger = logger;
        }

        public void Show()
        {
            _logger.LogInformation("Show Time{time}", DateTime.Now);//logger 的名字为 LoggingDemo.OrderService[0] namespace+classname
                                                                    //可以在appsetting中设置日志级别       
                                                                    // 使用模板方式在LogInformation（）输出时才拼接

            _logger.LogInformation($"Show Time{DateTime.Now}"); // 先拼接好之后传给LogInformation（）
        }
    }
}
