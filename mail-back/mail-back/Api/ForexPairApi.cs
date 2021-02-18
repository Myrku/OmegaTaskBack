using mail_back.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace mail_back.Api
{
    public class ForexPairApi : IForex
    {
        
        public async Task<List<ForexPair>> GetData(string symbolStock)
        {
            List<ForexPair> stock = new List<ForexPair>();
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://api.twelvedata.com/price?symbol={symbolStock}&apikey={IForex.APIKEY}")
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var s = JsonConvert.DeserializeObject<ForexPair>(body);
                s.symbol = symbolStock;
                stock.Add(s);
            }
            return stock;
        }
    }
}
