using mail_back.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace mail_back.Api
{
    public class ForexPairApi
    {
        const string APIKEY = "ee01f8ddf45c4e05a64ebfce5566779e";
        public async void GetData(string symbolStock)
        {
            ForexPair stock;
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://api.twelvedata.com/price?symbol={symbolStock}&apikey={APIKEY}")
            };

            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                stock = JsonConvert.DeserializeObject<ForexPair>(body);
                
            }
        }
    }
}
