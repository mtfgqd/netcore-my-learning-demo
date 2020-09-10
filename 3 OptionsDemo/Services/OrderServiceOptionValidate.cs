using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
namespace OptionsDemo.Services
{
    public interface IOrderServiceOptionValidate
    {
        int ShowMaxOrderCount();
    }
    public class OrderServiceOptionValidate : IOrderServiceOptionValidate
    {
        IOptionsMonitor<OrderServiceOptionValidateProperty> _options;
        public OrderServiceOptionValidate(IOptionsMonitor<OrderServiceOptionValidateProperty> options)
        {
            _options = options;
        }

        public int ShowMaxOrderCount()
        {
            return _options.CurrentValue.MaxOrderCountValidate;
        }
    }


    public class OrderServiceOptionValidateProperty
    {
        public const string SectionName = "OrderServiceOptionValidate";

        [Range(30, 800)]
        public int MaxOrderCountValidate { get; set; } = 700;

    }
    public class OrderServiceOptionValidateOptions : IValidateOptions<OrderServiceOptionValidateProperty>
    {
        public ValidateOptionsResult Validate(string name, OrderServiceOptionValidateProperty options)
        {
            if (options.MaxOrderCountValidate > 800)
            {
                return ValidateOptionsResult.Fail("自定义校验类中输出的错误：不能大于800");
            }
            else
            {
                return ValidateOptionsResult.Success;
            }
        }
    }

}

