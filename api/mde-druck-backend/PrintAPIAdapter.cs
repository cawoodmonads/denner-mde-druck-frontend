using Denner.MDEDruck.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Denner.MDEDruck
{
    public class PrintAPIAdapter
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PrintAPIAdapter> _log;
        private readonly ServiceOptions _options;
        private DateTime _token_valid_until = DateTime.MinValue;
        private const string APIPrefix = "/api/v1/";
        private const string HttpJsonContent = "application/json";

        public PrintAPIAdapter(ILogger<PrintAPIAdapter> logger, IHttpClientFactory httpClientFactory, IOptions<ServiceOptions> options)
        {
            _log = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options.Value;

            _log.LogDebug("PrintAPI Adapter initiating");
            _httpClient = httpClientFactory.CreateClient("HttpClientWithSSLCustom");
            _httpClient.BaseAddress = new Uri(_options.PrintBaseAddress);
            _httpClient.DefaultRequestHeaders.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue(HttpJsonContent)
                );
        }

        private async Task<HttpClient> GetAuthenticatedHttpClient()
        {
            if (DateTime.Now < _token_valid_until) return _httpClient;

            _log.LogDebug("APIToken NewToken Refresh required...");
            try
            {
                await RefreshAuthToken();
            }
            catch (HttpRequestException e)
            {
                if (e.StatusCode == null || (int)e.StatusCode != 401) throw;
                _log.LogDebug("APIToken TokenInvalid");
                _token_valid_until = DateTime.MinValue; // Invalidate token and...
                try
                {
                    await RefreshAuthToken(); // Try once more...
                    _log.LogInformation("APIToken NewToken Success");
                }
                catch (Exception ex)
                {
                    _log.LogError(ex, $"APIToken NewToken Refresh Retry Failed: {ex.Message}");
                    throw new UnexpectedException(ex.Message);
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, $"APIToken NewToken Refresh Failed: {ex.Message}");
                throw new UnexpectedException(ex.Message);
            }
            return _httpClient;
        }

        private async Task RefreshAuthToken()
        {
            string requestBodyForToken = $"grant_type=password&username={_options.PrintUserName}&password={_options.PrintPassword}";
            HttpResponseMessage responseWithToken = await _httpClient.PostAsync("/connect/token", new StringContent(requestBodyForToken, Encoding.UTF8, "application/x-www-form-urlencoded"));
            string body = await responseWithToken.EnsureSuccessStatusCode().Content.ReadAsStringAsync();
            AccessTokenResponse token = JsonSerializer.Deserialize<AccessTokenResponse>(body) ?? throw new InvalidOperationException("Failed to deserialize access token.");
            if (token == null) throw new Exception("Failed to deserialize token response");
            _token_valid_until = DateTime.Now.AddSeconds((token?.ExpiresIn ?? 0) * 0.95); //add the expires in delta minus 5% safety margin
            if (string.IsNullOrEmpty(token?.TokenType) || string.IsNullOrEmpty(token.AccessToken)) throw new InvalidOperationException("Token type or access token is null or empty.");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(token.TokenType, token.AccessToken);
            _log.LogDebug("GetAuthenticatedHttpClient: New Token received, it is valid until {Date}", _token_valid_until.ToLongTimeString());
        }

        public virtual async Task Print(PrintJob printJob)
        {
            string body = JsonSerializer.Serialize(printJob);
            HttpContent content = new StringContent(body, Encoding.UTF8, HttpJsonContent);
            HttpResponseMessage response = await (await GetAuthenticatedHttpClient()).PostAsync($"{APIPrefix}labelPrintjobs", content);
            LogHttpResponse(response, "LabelPrintJob");
            if (!response.IsSuccessStatusCode && !response.StatusCode.Equals(HttpStatusCode.Conflict)) HandleUnsuccessfulHttpResponse(response);
        }

        private void HandleUnsuccessfulHttpResponse(HttpResponseMessage response)
        {
            throw new APIException($"HTTPCall {response.RequestMessage?.Method} {response.RequestMessage?.RequestUri} Failed with status response {(int)response.StatusCode} {response.ReasonPhrase}");
        }

        private void LogHttpResponse(HttpResponseMessage response, string ObjectType)
        {
            var isError = !response.IsSuccessStatusCode;
            if (response.RequestMessage?.Method == HttpMethod.Get)
            {
                // GET: Exclude 404 as non-errors are expected when testing to see if something exists
                if (response.StatusCode == HttpStatusCode.NotFound) isError = false;
            }
            else
            {
                // PUT, POST: Everything is an error except Conflict (409)
                if (response.StatusCode == HttpStatusCode.Conflict) isError = false;
            }
            if (isError)
            {
                var responseText = response.Content.ReadAsStringAsync().Result;
                try
                {
                    var err = JsonSerializer.Deserialize<PrintAPIErrorResponse>(responseText);
                    responseText = $"PrintErrorMessage: {err?.Message ?? err?.Message2 ?? "UNKNOWN"}";
                }
                catch { }
                _log.LogError("HTTP{Method}Failed  {ObjectType} {RequestUri} status {StatusCode} ({ReasonPhrase}): {responseText}",
                    response.RequestMessage?.Method,
                    ObjectType,
                    response.RequestMessage?.RequestUri,
                    (int)response.StatusCode,
                    response.ReasonPhrase,
                    responseText);
            }
        }

    }

    class AccessTokenResponse
    {
        [JsonPropertyName("token_type")]
        public required string TokenType { get; set; }
        [JsonPropertyName("access_token")]
        public required string AccessToken { get; set; }
        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }

    public partial class PrintAPIErrorResponse
    {
        [JsonPropertyName("Message")]
        public required string Message { get; set; }

        [JsonPropertyName("message")]
        public required string Message2 { get; set; }

        [JsonPropertyName("RequestId")]
        public required string RequestId { get; set; }

        [JsonPropertyName("Timestamp")]
        public required string Timestamp { get; set; }

        [JsonPropertyName("FieldName")]
        public required string FieldName { get; set; }
    }

    public class PrintJob
    {
        public required List<string> Shops { get; set; }
        public required string ArticleCode { get; set; }
        public required string LabelFormat { get; set; }
        public DateTime PrintDate { get; set; }
    }

    public class LabelDetails
    {
        public required string ItemCode { get; set; }
        public string? ItemDescription { get; set; }
        public float Price { get; set; }
        public float RegularPrice { get; set; }
        public float BasePrice { get; set; }
        public float AdditionPrice { get; set; }
        public string? Promotion { get; set; }
        public float PromotionValue { get; set; }
        public string? Barcode { get; set; }
        public string? ShortDescription { get; set; }
        public string? Notes1 { get; set; }
        public string? Notes2 { get; set; }
        public string? Notes3 { get; set; }
        public string? PrintoutNotes { get; set; }
        public string? SaleUnit { get; set; }
        public int SaleQuantity { get; set; }
        public string? UomDescription { get; set; }
        public string? CustomValue1 { get; set; }
        public string? CustomValue2 { get; set; }
        public string? CustomValue3 { get; set; }
        public string? CustomValue4 { get; set; }
        public string? CustomValue5 { get; set; }
        public string? CustomValue6 { get; set; }
    }

}