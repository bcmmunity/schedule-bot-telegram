using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelegrammAspMvcDotNetCoreBot.Models
{
    public class UserHW
    {
        public UserHW()
        {
            IsActive = false;
        }
        public long ChatId { get; set; }
        public bool IsActive { get; set; }
        public string Date { get; set; }
    }
}
