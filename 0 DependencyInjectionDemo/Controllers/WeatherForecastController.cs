using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DependencyInjectionDemo.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace DependencyInjectionDemo.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };
        IOrderService _orderService;
        private readonly ILogger<WeatherForecastController> _logger;
        // 从容器中获取服务的方式有两种
        // 1.构造函数注入，多个接口用到了该服务
        // 2.FromServices 从容器中获取注入的服务,只有个别接口用到了该服务
        public WeatherForecastController(ILogger<WeatherForecastController> logger, IOrderService orderService, IGenericService<IOrderService> genericService)
        {
            _orderService = orderService;
            _logger = logger;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var name = _orderService.ToString();
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpGet]
        public int GetService([FromServices] IMySingletonService singleton1,
                              [FromServices] IMySingletonService singleton2,
                              [FromServices] IMyTransientService transient1,
                              [FromServices] IMyTransientService transient2,
                              [FromServices] IMyScopedService scoped1,
                              [FromServices] IMyScopedService scoped2)
        {
            Console.WriteLine($"singleton1:{singleton1.GetHashCode()}");
            Console.WriteLine($"singleton2:{singleton2.GetHashCode()}");


            Console.WriteLine($"transient1:{transient1.GetHashCode()}");
            Console.WriteLine($"transient2:{transient2.GetHashCode()}");

            Console.WriteLine($"scoped1:{scoped1.GetHashCode()}");
            Console.WriteLine($"scoped2:{scoped2.GetHashCode()}");

            Console.WriteLine($"========请求结束=======");
            return 1;
        }


        [HttpGet]
        public int GetServiceList([FromServices] IEnumerable<ISingletonOrderService> services)
        {
            foreach (var item in services)
            {
                Console.WriteLine($"获取到服务实例：{item.ToString()}:{item.GetHashCode()}");
            }
            var myNeeded = services.First(o => o.GetType() == typeof(SingletonOrderServiceEx));
            Console.WriteLine($"myNeeded 实例：{myNeeded.ToString()}:{myNeeded.GetHashCode()}");
            return 1;
        }

        /// ITransitDisposableOrderService被注册为瞬时
        /// service1 service2 service3 service4是四个不同对象
        /// 接口执行完毕后被分别释放，共释放4个对象
        [HttpGet]
        public int TransitDispose([FromServices] ITransitDisposableOrderService service1,
            [FromServices] ITransitDisposableOrderService service2)
        {
            #region 
            Console.WriteLine("=======1==========");

            //  HttpContext.RequestServices是当前请求的一个容器，是根容器的子容器，在此子容器下再创建一个子容器
            using (IServiceScope scope = HttpContext.RequestServices.CreateScope())
            {
                var service3 = scope.ServiceProvider.GetService<ITransitDisposableOrderService>();
                var service4 = scope.ServiceProvider.GetService<ITransitDisposableOrderService>();
            }
            Console.WriteLine("=======2==========");

            Console.WriteLine("接口请求处理结束");
            #endregion
            return 1;
        }

        /// IScopeDisposableOrderService 被注册为scope，
        /// service1和service2在一个作用域内，只能获得一个对象，所以service1=service2
        /// service3和service4在根容器的子容器的子容器内同属一个作用域，获得的service3和service4是同一个对象，所以也只释放一次
        /// 最终只释放2个对象
        [HttpGet]
        public int ScopeDispose([FromServices] IScopeDisposableOrderService service1,
     [FromServices] IScopeDisposableOrderService service2)
        {
            #region 
            Console.WriteLine("=======1==========");

            //  HttpContext.RequestServices是当前请求的一个容器，是根容器的子容器，在此子容器下再创建一个子容器
            using (IServiceScope scope = HttpContext.RequestServices.CreateScope())
            {
                var service3 = scope.ServiceProvider.GetService<IScopeDisposableOrderService>();
                var service4 = scope.ServiceProvider.GetService<IScopeDisposableOrderService>();
            }
            Console.WriteLine("=======2==========");

            Console.WriteLine("接口请求处理结束");
            #endregion
            return 1;
        }

        /// ISingletonDisposableOrderService被注册为单例的，尽管service1 到service4，服务被引用4次，但是同一个对象，
        /// 整个生命周期内只有一个对象，执行完毕后对象也不会被释放
        [HttpGet]
        public int SingletonDispose([FromServices] ISingletonDisposableOrderService service1,
            [FromServices] ISingletonDisposableOrderService service2)
        {
            #region 
            Console.WriteLine("=======1==========");          
            using (IServiceScope scope = HttpContext.RequestServices.CreateScope())
            {
                var service3 = scope.ServiceProvider.GetService<ISingletonDisposableOrderService>();
                var service4 = scope.ServiceProvider.GetService<ISingletonDisposableOrderService>();
            }
            Console.WriteLine("=======2==========");

            Console.WriteLine("接口请求处理结束");
            #endregion
            return 1;
        }
        /// DisposableOrderService服务是我们自己创建的，容器不会帮我们回收
        /// singletonDisposableOrderService服务由容器管理，容器会回收
        [HttpGet]
        public int DisposeLifeTime([FromServices] IDisposableOrderService service1,
            [FromServices] ISingletonDisposableOrderService service2,
            [FromServices] IHostApplicationLifetime hostApplicationLifetime, [FromQuery] bool stop = false)
        {
            #region 
            Console.WriteLine("=======1==========");
            using (IServiceScope scope = HttpContext.RequestServices.CreateScope())
            {
                var service11 = scope.ServiceProvider.GetService<IDisposableOrderService>();
                var service22 = scope.ServiceProvider.GetService<ISingletonDisposableOrderService>();
            }
            Console.WriteLine("=======2==========");
            #endregion

            #region
            if (stop)
            {
                hostApplicationLifetime.StopApplication();
            }
            #endregion

            Console.WriteLine("接口请求处理结束");
            return 1;
        }
    }
}
