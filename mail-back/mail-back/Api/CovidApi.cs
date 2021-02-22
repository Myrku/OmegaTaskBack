using mail_back.Models;
using Newtonsoft.Json;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace mail_back.Api
{
    public class CovidApi : ICovid
    {
        Logger logger = LogManager.GetCurrentClassLogger();
        public async Task<List<Covid>> GetData(string param)
        {
            List<Covid> covids = new List<Covid>();
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"https://covid19-api.org/api/status/{param}")
                };

                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var body = await response.Content.ReadAsStringAsync();
                    if (string.IsNullOrEmpty(param))
                    {
                        covids = JsonConvert.DeserializeObject<List<Covid>>(body);
                    }
                    else
                    {
                        covids.Add(JsonConvert.DeserializeObject<Covid>(body));
                    }
                }
                return covids;
            }
            catch (Exception ex)
            {
                logger.Error(ex.Message);
                return covids;
            }
        }
    }
}
