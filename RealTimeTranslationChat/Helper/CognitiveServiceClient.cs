using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RealTimeTranslationChat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RealTimeTranslationChat.Helper
{
    public class CognitiveServiceClient
    {
        private HttpClient _client;
        private readonly string _apiKey;
        private readonly string _apiVersion;
        private ILogger<CognitiveServiceClient> _logger;
        public CognitiveServiceClient(HttpClient client, ILogger<CognitiveServiceClient> logger, IConfiguration config)
        {
            _client = client;
            _client.BaseAddress = new Uri(config["AppSettings:APIBaseURL"]);
            _apiVersion = config["AppSettings:APIVersion"];
            _apiKey = config["AppSettings:SubscriptionKey"];
            _logger = logger;
        }

        public async Task<List<Language>> GetSupportedLanguages()
        {
            var languages = new List<Language>();
            try
            {
                var languagesUrl = new Uri($"/languages?api-version={_apiVersion}", UriKind.Relative);
                var res = await _client.GetAsync(languagesUrl);
                res.EnsureSuccessStatusCode();
                var jsonResults = await res.Content.ReadAsStringAsync();

                dynamic entity = JObject.Parse(jsonResults);
                foreach (JProperty property in entity.translation.Properties())
                {
                    dynamic langDetail = JObject.Parse(property.Value.ToString());
                    var language = new Language();
                    language.Code = property.Name;
                    language.Name = langDetail.name;
                    languages.Add(language);
                }
                return languages;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"An error occurred connecting to CognitiveService API {ex.ToString()}");
                throw;
            }
        }

        public async Task<List<TranslationResult>> Translate(string message, string languages)
        {
            try
            {
                System.Object[] body = new System.Object[] { new { Text = message } };
                var requestBody = JsonConvert.SerializeObject(body);

                var translateUrl = new Uri($"/translate?api-version={_apiVersion}&{languages}", UriKind.Relative);
                
                string jsonResults = null;
                using (var request = new HttpRequestMessage())
                {
                    request.Method = HttpMethod.Post;
                    request.RequestUri = translateUrl;
                    request.Content = new StringContent(requestBody, Encoding.UTF8, "application/json");
                    request.Headers.Add("Ocp-Apim-Subscription-Key", _apiKey);

                    var res = await _client.SendAsync(request);                   
                    jsonResults = await res.Content.ReadAsStringAsync();
                }
                return JsonConvert.DeserializeObject<List<TranslationResult>>(jsonResults);
                    
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError($"An error occurred connecting to CognitiveService API {ex.ToString()}");
                throw;
            }
        }
    }
}
