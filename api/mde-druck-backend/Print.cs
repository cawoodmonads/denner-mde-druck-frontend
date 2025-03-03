using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using FromBodyAttribute = Microsoft.Azure.Functions.Worker.Http.FromBodyAttribute;


namespace MDEDruck
{
    public class PrintAPI
    {
        private readonly ILogger<PrintAPI> _log;

        public PrintAPI(ILogger<PrintAPI> logger)
        {
            _log = logger;
        }

        [Function("LabelsPrint")]
        public IActionResult Print([HttpTrigger(AuthorizationLevel.Function, "post", Route = "print")] HttpRequest req, [FromBody] PrintRequest printRequest)
        {
            _log.LogInformation($"Labels Print: '{printRequest.items}'");
            return new OkObjectResult("Welcome to Azure Functions!");
        }

        public record PrintRequest(string Name, int items);
    }
}
