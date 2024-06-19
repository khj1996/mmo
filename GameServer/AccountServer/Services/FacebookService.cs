using AccountServer.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace AccountServer.Services
{
    public class FacebookService
    {
        HttpClient _httpClient;
        readonly string _accessToken = "2271097109703692|AF4LnhdkqmxTiVqz_aZCeOgKi0g"; // TODO Secret

        public FacebookService()
        {
            _httpClient = new HttpClient() { BaseAddress = new Uri("https://graph.facebook.com/") };
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<FacebookTokenData> GetUserTokenData(string inputToken)
        {
            HttpResponseMessage response =
                await _httpClient.GetAsync($"debug_token?input_token={inputToken}&access_token={_accessToken}");

            if (!response.IsSuccessStatusCode)
                return null;

            string resultStr = await response.Content.ReadAsStringAsync();

            FacebookResponseJsonData result = JsonConvert.DeserializeObject<FacebookResponseJsonData>(resultStr);

            return result.data;
        }
    }
}