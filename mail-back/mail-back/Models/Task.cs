using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mail_back.Models
{
    public class Task
    {
        public int id { get; set; }
        public int userid { get; set; }
        public string taskname { get; set; }
        public string description { get; set; }
        public string starttime { get; set; }
        public int period { get; set; }
        public int apiid { get; set; }
        public string apiparam { get; set; }
        public string laststart { get; set; }
    }
}
