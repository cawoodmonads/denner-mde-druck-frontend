using System.Text.Json.Serialization;
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
            var articles = new List<Article>{
                new("Spaghetti al Pomodoro", "19932458", "800 g", "7.45", "3"),
                new("Cherry-Rispentomaten", "19953411", "500 g", "5.00", "4"),
                new("Rispentomaten", "19911458", "Spanien, per kg", "6.00", "5")
            };
            _log.LogInformation($"Articles Search: '{query}'");
            return new OkObjectResult(articles);
        }
    }

    public class Article
    {
        [JsonPropertyName("description")]
        public string Description { get; set; }
        [JsonPropertyName("materialNo")]
        public string MaterialNo { get; set; }
        [JsonPropertyName("unit")]
        public string Unit { get; set; }
        [JsonPropertyName("price")]
        public string Price { get; set; }
        [JsonPropertyName("barCode")]
        public string BarCode { get; set; }

        public Article(string description, string materialNo, string unit, string price, string barCode)
        {
            this.Description = description;
            this.MaterialNo = materialNo;
            this.Unit = unit;
            this.Price = price;
            this.BarCode = barCode;
        }
    }
}
