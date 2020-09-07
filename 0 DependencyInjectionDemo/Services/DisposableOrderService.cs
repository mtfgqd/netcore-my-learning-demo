using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
namespace DependencyInjectionDemo.Services
{

    public interface IDisposableOrderService
    {

    }
    public interface ISingletonDisposableOrderService
    {

    }
    public interface IScopeDisposableOrderService
    {

    }
    public interface ITransitDisposableOrderService
    {

    }

    public class DisposableOrderService : IDisposableOrderService, IDisposable
    {
        public void Dispose()
        {
            Console.WriteLine($"DisposableOrderService Disposed:{this.GetHashCode()}");
        }
    }
    public class SingletonDisposableOrderService : ISingletonDisposableOrderService, IDisposable
    {
        public void Dispose()
        {
            Console.WriteLine($"SingletonDisposableOrderService Disposed:{this.GetHashCode()}");
        }
    }
    public class ScopeDisposableOrderService : IScopeDisposableOrderService, IDisposable
    {
        public void Dispose()
        {
            Console.WriteLine($"ScopeDisposableOrderService Disposed:{this.GetHashCode()}");
        }
    }
    public class TransitDisposableOrderService : ITransitDisposableOrderService, IDisposable
    {
        public void Dispose()
        {
            Console.WriteLine($"TransitDisposableOrderService Disposed:{this.GetHashCode()}");
        }
    }
}
