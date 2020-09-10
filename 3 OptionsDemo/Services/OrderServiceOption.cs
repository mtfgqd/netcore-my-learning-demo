using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
namespace OptionsDemo.Services
{
    public interface IOrderServiceOption
    {
        int ShowMaxOrderCount();
    }
    public class OrderServiceOption : IOrderServiceOption
    {
        IOptions<OrderServiceOptionProperty> _options;
        public OrderServiceOption(IOptions<OrderServiceOptionProperty> options)
        {
            _options = options;
        }

        public int ShowMaxOrderCount()
        {
            return _options.Value.MaxOrderCount;
        }
    }


    public class OrderServiceOptionProperty
    {
        public const string SectionName = "OrderService";

        public int MaxOrderCount { get; set; } = 100;
    }

}

