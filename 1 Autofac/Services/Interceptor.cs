using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DependencyInjectionAutofacDemo.Services
{
    // autofac 面向切面的最重要接口IInterceptor
    public class MyInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            Console.WriteLine($"Intercept before,Method:{invocation.Method.Name}");
            invocation.Proceed();
            Console.WriteLine($"Intercept after,Method:{invocation.Method.Name}");
        }
    }
}
