using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DependencyInjectionDemo.Services
{

    public interface IOrderService
    {

    }

    public class OrderService : IOrderService
    {

    }


    public class OrderServiceEx : IOrderService
    { 
    
    }

    public interface ISingletonOrderService
    {

    }

    public class SingletonOrderService : ISingletonOrderService
    {

    }

    public class SingletonOrderServiceEx : ISingletonOrderService
    {

    }
    public class SingletonOrderServiceFactory : ISingletonOrderService
    {

    }
    public class SingletonOrderServiceTryAdd : ISingletonOrderService
    {

    }
    public interface IScopeOrderService
    {

    }

    public class ScopeOrderService : IScopeOrderService
    {

    }


    public class ScopeOrderServiceEx : IScopeOrderService
    {

    }
    public interface ITransitOrderService
    {

    }

    public class TransitOrderService : ITransitOrderService
    {

    }


    public class TransitOrderServiceEx : ITransitOrderService
    {

    }
}
