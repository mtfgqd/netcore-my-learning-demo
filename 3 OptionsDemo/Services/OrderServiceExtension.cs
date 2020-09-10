using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
namespace OptionsDemo.Services
{
    public interface IOrderServiceExtension
    {
        int ShowMaxOrderCount();
    }
    public class OrderServiceExtension : IOrderServiceExtension
    {
        IOptions<OrderServicePropertyExtension> _options;
        public OrderServiceExtension(IOptions<OrderServicePropertyExtension> options)
        {
            _options = options;
        }

        public int ShowMaxOrderCount()
        {
            return _options.Value.MaxOrderCountExtension;
        }
    }

    public class OrderServicePropertyExtension
    {
        public const string SectionName = "OrderServiceExtension";

        public int MaxOrderCountExtension { get; set; } = 700;// 这个值来自配置文件，把这个属性绑定到配置项上，怎么处理 
                                                              //见OrderServiceOption
    }

}

