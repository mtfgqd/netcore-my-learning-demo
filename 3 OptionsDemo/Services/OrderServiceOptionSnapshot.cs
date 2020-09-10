using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
namespace OptionsDemo.Services
{   
    public interface IOrderServiceOptionSnapshot
    {
        int ShowMaxOrderCount();
    }

    //IOptionsSnapshot 是一种作用域服务，并在构造 IOptionsSnapshot<T> 对象时提供选项的快照。 选项快照旨在用于暂时性和有作用域的依赖项。
    //当使用支持读取已更新的配置值的配置提供程序时，将在应用启动后读取对配置所做的更改（重新请求时检测配置项的更新）
    //范围作用域使用IOptionsSnapshot作为构造函数参数
    //当配置项发生变化在次请求可以得到新的配置值
    public class OrderServiceOptionSnapshot : IOrderServiceOptionSnapshot
    {
        IOptionsSnapshot<OrderServiceOptionPropertySnapshot> _options;
        public OrderServiceOptionSnapshot(IOptionsSnapshot<OrderServiceOptionPropertySnapshot> options)
        {
            _options = options;
        }

        public int ShowMaxOrderCount()
        {
            return _options.Value.MaxOrderCountSnapshot;
        }
    }



    public class OrderServiceOptionPropertySnapshot
    {
        public const string SectionName = "OrderServiceSnapshot";

        public int MaxOrderCountSnapshot { get; set; } = 300;
    }
}

