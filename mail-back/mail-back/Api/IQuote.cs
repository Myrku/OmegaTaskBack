using mail_back.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mail_back.Api
{
    interface IQuote
    {
        Task<List<Quote>> GetData();
    }
}
