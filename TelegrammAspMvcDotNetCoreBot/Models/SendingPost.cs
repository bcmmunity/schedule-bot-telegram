using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelegrammAspMvcDotNetCoreBot.Models
{
    public class SendingPost
    {
        public string Key { get; set; }
        public string SocialNetwork { get; set; }
        public string University { get; set; }
        public string Facility { get; set; }
        public string Course { get; set; }
        public string Group { get; set; }

        public string Message { get; set; }
    }
}
