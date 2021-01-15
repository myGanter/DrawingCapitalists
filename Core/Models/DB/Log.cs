using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Core.Models.DB
{
    public class Log
    {
        public int Id { get; set; }

        public User User { get; set; }

        public DateTime Time { get; set; }

        public LogLevel Type { get; set; }

        public string Message { get; set; }

        public string ExceptionMessage { get; set; } 

        public string StackTrace { get; set; }

        public string Context { get; set; }

        public string RequestId { get; set; }
    }
}
