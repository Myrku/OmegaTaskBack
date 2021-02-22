using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mail_back.Models
{
    public class Task
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string TaskName { get; set; }
        public string Description { get; set; }
        public string StartTime { get; set; }
        public string Period { get; set; }
        public int ApiId { get; set; }
        public string ApiParam { get; set; }
        public string LastStart { get; set; }
        public int Count { get; set; }
    }
}
