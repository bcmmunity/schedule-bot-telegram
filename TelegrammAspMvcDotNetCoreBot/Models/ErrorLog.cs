using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelegrammAspMvcDotNetCoreBot.Models
{
    public class ErrorLog
    {
        public int Id { get; set; }
        public long ChatId { get; set; }
        public string UpdateType { get; set; }
        public string MessageText { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime ErrorDateTime { get; set; }
    }
}
