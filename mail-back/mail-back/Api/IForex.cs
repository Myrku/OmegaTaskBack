using mail_back.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mail_back.Api
{
    interface IForex
    {
        Task<List<ForexPair>> GetData(string symbolStock);
        const string APIKEY = "ee01f8ddf45c4e05a64ebfce5566779e";
    }
}
