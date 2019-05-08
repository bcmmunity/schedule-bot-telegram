using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelegrammAspMvcDotNetCoreBot.Models
{
    public class StatisticLog
    {
        public int Id { get; set; }
        public long ChatId { get; set; }
        public string MessageText { get; set; }
        public DateTime MessageDateTime { get; set; }
    }
}
