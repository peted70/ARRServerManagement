using ARRServerManagement.Controllers;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ARRServerManagement.Models
{
    public class ARRService : IARRService
    {
        private const string ArrUri = "https://remoterendering.westeurope.mixedreality.azure.com";
        private const string StsUri = "https://sts.mixedreality.azure.com";

        private string _arrAccountId = string.Empty;
        private string _arrAccountKey = string.Empty;

        //private HttpClient _httpClient;
        private TokenResponse _token;

        public ARRService(IConfiguration config)
        {
            _arrAccountId = config.GetValue<string>("ARR:AccountId");
            if (string.IsNullOrEmpty(_arrAccountId))
            {
                throw new ArgumentException("ARR Account Id not set [set ARR:AccountId as a user secret]");
            }

            _arrAccountKey = config.GetValue<string>("ARR:AccountKey");
            if (string.IsNullOrEmpty(_arrAccountKey))
            {
                throw new ArgumentException("ARR Account Key not set [set ARR:AccountKey as a user secret]");
            }
        }

        private async Task<HttpClient> GetHttpClientAsync()
        {
            var token = await GetTokenAsync(_arrAccountId, _arrAccountKey);
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(ArrUri);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            httpClient.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
            return httpClient;
        }

        public async Task CreateSessionAsync(SessionDescriptor sessionDescriptor)
        {
            using var httpClient = await GetHttpClientAsync();
            string uri = $"/v1/accounts/{_arrAccountId}/sessions/create";

            var settings = new JsonSerializerSettings();
            settings.Converters.Add(new StringEnumConverter());
            var sessionStr = JsonConvert.SerializeObject(sessionDescriptor, settings);

            StringContent content = new StringContent(sessionStr, Encoding.UTF8, "application/json");

            var response = await httpClient.PostAsync(uri, content);
            response.EnsureSuccessStatusCode();
        }

        public async Task StopServerAsync(string sessionId)
        {
            using var httpClient = await GetHttpClientAsync();
            string uri = $"/v1/accounts/{_arrAccountId}/sessions/{sessionId}";
            var response = await httpClient.DeleteAsync(uri);
            response.EnsureSuccessStatusCode();
        }

        public async Task ExtendSessionAsync(string sessionId)
        {
            using var httpClient = await GetHttpClientAsync();
            string uri = $"/v1/accounts/{_arrAccountId}/sessions/{sessionId}";

            var str = JsonConvert.SerializeObject(new { maxLeaseTime = new TimeSpan(1, 0, 0) });
            StringContent content = new StringContent(str, Encoding.UTF8, "application/json");
            var response = await httpClient.PatchAsync(uri, content);
            response.EnsureSuccessStatusCode();
        }

        public async Task<SessionsModel> GetSessionsAsync()
        {
            using var httpClient = await GetHttpClientAsync();
            string uri = $"/v1/accounts/{_arrAccountId}/sessions?{DateTime.Now.Ticks}";
            var response = await httpClient.GetStringAsync(uri);
            var sessionRoot = JsonConvert.DeserializeObject<SessionRoot>(response);
            return new SessionsModel { Root = sessionRoot };
        }

        async Task<string> GetTokenAsync(string accId, string accKey)
        {
            if (_token == null)
            {
                string resp = null;
                using (var httpClient = new HttpClient())
                {
                    httpClient.BaseAddress = new Uri(StsUri);
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",
                        $"{accId}:{accKey}");

                    string uri = $"accounts/{accId}/token";
                    resp = await httpClient.GetStringAsync(uri);
                }

                var tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(resp);
                _token = tokenResponse;
            }
            return _token.AccessToken;
        }
    }
}
