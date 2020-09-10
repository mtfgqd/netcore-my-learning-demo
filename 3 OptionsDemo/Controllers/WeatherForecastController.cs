using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OptionsDemo.Services;
namespace OptionsDemo.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public int Get([FromServices]IOrderService orderService,
            [FromServices] IOrderServiceOption orderServiceOption,
            [FromServices] IOrderServiceOptionSnapshot orderServiceOptionSnapshot,
            [FromServices] IOrderServiceOptionMonitor orderServiceOptionMonitor,
            [FromServices] IOrderServiceExtension orderServiceExtension,
             [FromServices] IOrderServiceOptionValidate orderServiceOptionValidate
            )
        {
            Console.WriteLine($"orderService.ShowMaxOrderCount:{orderService.ShowMaxOrderCount()}");
            Console.WriteLine($"orderServiceOption.ShowMaxOrderCount:{orderServiceOption.ShowMaxOrderCount()}");
            Console.WriteLine($"orderServiceOptionSnapshot.ShowMaxOrderCount:{orderServiceOptionSnapshot.ShowMaxOrderCount()}");
            Console.WriteLine($"orderServiceOptionMonitor.ShowMaxOrderCount:{orderServiceOptionMonitor.ShowMaxOrderCount()}");
            Console.WriteLine($"orderServiceExtension.ShowMaxOrderCount:{orderServiceExtension.ShowMaxOrderCount()}");
            Console.WriteLine($"orderServiceOptionValidate.ShowMaxOrderCount:{orderServiceOptionValidate.ShowMaxOrderCount()}");
            return 1;
        }
    }
}
