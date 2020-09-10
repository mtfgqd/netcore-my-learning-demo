using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
namespace OptionsDemo.Services
{
    public interface IOrderServiceOptionMonitor
    {
        int ShowMaxOrderCount();
    }

    //IOptionsMonitor 是一种单一示例服务，可随时检索当前选项值(会实时检测配置项的更新)，如果有更新可以使用OnChange方法通知应用
    //单例服务使用IOptionsMonitor作为构造函数参数
    public class OrderServiceOptionMonitor : IOrderServiceOptionMonitor
    {
        IOptionsMonitor<OrderServiceOptionPropertyMonitor> _options;      
        public OrderServiceOptionMonitor(IOptionsMonitor<OrderServiceOptionPropertyMonitor> options)
        {
            _options = options;          
            _options.OnChange(option =>
            {
                Console.WriteLine($"配置更新了，最新的值是:{_options.CurrentValue.MaxOrderCountMonitor}");
            });
        }

        public int ShowMaxOrderCount()
        {
            return _options.CurrentValue.MaxOrderCountMonitor;
        }
    }

    public class OrderServiceOptionPropertyMonitor
    {
        public const string SectionName = "OrderServiceMonitor";

        public int MaxOrderCountMonitor { get; set; } = 200;
    }
}

