using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DependencyInjectionAutofacDemo.Services
{
    public class WiredProperty
    {

    }

    public interface IMyService
    {
        void ShowCode();
    }
    public interface IMyServiceV2
    {
        void ShowCode();
    }
    public interface IAOPService
    {
        void ShowCode();
    }
    public class MyService : IMyService
    {
        public void ShowCode()
        {
            Console.WriteLine($"MyService.ShowCode:{GetHashCode()}");
        }
    }

    public class NamedService : IMyService
    {
        public void ShowCode()
        {
            Console.WriteLine($"NamedService.ShowCode:{GetHashCode()}");
        }
    }

    public class PropertyAutoWiredService : IMyServiceV2
    {
        //用来演示属性注入
        public WiredProperty WiredProperty { get; set; }

        public void ShowCode()
        {
            Console.WriteLine($"PropertyAutoWiredService.ShowCode:{GetHashCode()},WiredProperty是否为空：{WiredProperty == null}");
        }
    }

    public class ScopedService : IMyService
    {
        public void ShowCode()
        {
            Console.WriteLine($"ScopedService.ShowCode:{GetHashCode()}");
        }
    }
    public class AOPService : IAOPService
    {
        public void ShowCode()
        {
            Console.WriteLine($"AOPService.ShowCode:{GetHashCode()}");
        }
    }
    public class AOPPropertyAutoWiredService : IAOPService
    {
        public WiredProperty AOPWiredProperty { get; set; }
        public void ShowCode()
        {
            Console.WriteLine($"AOPServiceV2.ShowCode:{GetHashCode()}WiredProperty是否为空：{AOPWiredProperty == null}");
        }
    }
}
