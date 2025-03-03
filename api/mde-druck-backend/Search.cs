using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace MDEDruck
{
    public class SearchAPI
    {
        private readonly ILogger<SearchAPI> _log;

        public SearchAPI(ILogger<SearchAPI> logger)
        {
            _log = logger;
        }

        [Function("ArticlesSearch")]
        public IActionResult Search([HttpTrigger(AuthorizationLevel.Function, "get", Route = "articles/search")] HttpRequest req, string query)
        {
            _log.LogInformation($"Articles Search: '{query}'");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
