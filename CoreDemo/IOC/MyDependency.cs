using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoreDemo.IOC
{
    public interface IMyDependency
    {
        Task WriteMessage(string message);
    }

    public class MyDependency : IMyDependency
    {
        private readonly ILogger<MyDependency> _logger;

        public MyDependency(ILogger<MyDependency> logger)
        {
            _logger = logger;
        }

        public Task WriteMessage(string message)
        {
            _logger.LogInformation(
            "MyDependency.WriteMessage called. Message: {MESSAGE}",
            message);

            return Task.FromResult(0);
        }
    }
    public class DifMyDependency : IMyDependency
    {
        private readonly ILogger<MyDependency> _logger;

        public DifMyDependency(ILogger<MyDependency> logger)
        {
            _logger = logger;
        }

        public Task WriteMessage(string message)
        {
            _logger.LogInformation(
            "MyDependency.WriteMessage called. Message: {MESSAGE}",
            message);

            return Task.FromResult(0);
        }
    }

    public class IndexModel : PageModel
    {
        private readonly IMyDependency _myDependency;

        public IndexModel(IMyDependency myDependency)
        {
            _myDependency = myDependency;
        }
        public async Task OnGetAsync()
        {
            await _myDependency.WriteMessage(
                "IndexModel.OnGetAsync created this message.");
        }
    }
}
