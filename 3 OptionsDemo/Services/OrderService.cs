using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
namespace OptionsDemo.Services
{
    public interface IOrderService
    {
        int ShowMaxOrderCount();
    }
    public class OrderService : IOrderService
    {
        OrderServiceProperty _options;
        public OrderService(OrderServiceProperty options)
        {
            _options = options;
        }

        public int ShowMaxOrderCount()
        {
            return _options.MaxOrderCount;
        }
    }

    public class OrderServiceProperty
    {
        [Range(30, 100)]
        public int MaxOrderCount { get; set; } = 100;// 这个值来自配置文件，把这个属性绑定到配置项上，怎么处理 
                                                     //见OrderServiceOption
    }


}

