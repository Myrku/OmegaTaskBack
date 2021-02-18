using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mail_back.Models
{
    public class Quote
    {
        public int quote_id { get; set; }
        public string quote { get; set; }
        public string author { get; set; }
        public string series { get; set; }
    }
}
