using Denner.MDEDruck;
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
        private readonly PrintAPIAdapter _printAPI;

        public PrintAPI(ILogger<PrintAPI> logger, PrintAPIAdapter printAPI)
        {
            _log = logger;
            _printAPI = printAPI;
        }

        [Function("LabelsPrint")]
        public async Task<IActionResult> Print([HttpTrigger(AuthorizationLevel.Function, "post", Route = "print")] HttpRequest req, [FromBody] PrintRequest printRequest)
        {
            foreach (var item in printRequest.Items)
            {
                // var article = await GetArticle(item.MaterialNo);
                var printJob = KumoArticleToPrintJob(item.MaterialNo, printRequest.BranchId);
                for (var i = 0; i < item.Quantity; i++)
                {
                    _log.LogInformation($"{i}. Printing label: '{item.MaterialNo}' ...");
                    await _printAPI.Print(printJob);
                }
            }
            _log.LogInformation($"Labels Print: '{printRequest.Items.Count} labels submitted for print");
            return new OkObjectResult("Print job submitted successfully");
        }


        private PrintJob KumoArticleToPrintJob(string materialNo, string branchId)
        {
            return new PrintJob
            {
                Shops = new List<string> { branchId },
                ArticleCode = materialNo,
                LabelFormat = "???",
                PrintDate = new DateTime()
            };
        }


        public record PrintRequest(string BranchId, List<PrintItem> Items);

        public record PrintItem(string MaterialNo, int Quantity);
    }
}
