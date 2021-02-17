using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mail_back.Models
{
    public class Covid
    {
        public string country { get; set; }
        public DateTime last_update { get; set; }
        public int cases { get; set; }
        public int deaths { get; set; }
        public int recovered { get; set; }
    }
}
