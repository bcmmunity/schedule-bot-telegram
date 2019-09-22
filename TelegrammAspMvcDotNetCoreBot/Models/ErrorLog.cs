using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelegrammAspMvcDotNetCoreBot.Models
{
    public class ErrorLog
    {
        public int ErrorLogId { get; set; }
        public SnUser SnUser { get; set; }
        public string UpdateType { get; set; }
        public string MessageText { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime ErrorDateTime { get; set; }
    }
}
