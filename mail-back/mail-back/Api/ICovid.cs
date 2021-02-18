using mail_back.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mail_back.Api
{
    public interface ICovid
    {
        Task<List<Covid>> GetData(string param);
    }
}
