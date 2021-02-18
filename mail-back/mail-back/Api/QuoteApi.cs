using mail_back.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace mail_back.Api
{
    public class QuoteApi : IQuote
    {
        public async Task<List<Quote>> GetData()
        {
            List<Quote> quote = new List<Quote>();
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://www.breakingbadapi.com/api/quote/random")
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                quote = JsonConvert.DeserializeObject<List<Quote>>(body);
            }
            return quote;
        }
    }
}
