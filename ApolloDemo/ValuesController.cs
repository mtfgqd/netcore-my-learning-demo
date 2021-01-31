using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace ApolloDemo
{
    [Route("api/Values")]
    public class ValuesController : Controller
    {
        private IConfiguration _configuration;

        public ValuesController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Get([FromServices] IConfiguration configuration)
        {
            return Content(configuration["timeout"]);
        }
    }
}
