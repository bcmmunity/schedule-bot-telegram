﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelegrammAspMvcDotNetCoreBot.Models
{
    public class ActivityLog
    {
        public int ActivityLogId { get; set; }
        public SnUser SnUser { get; set; }
        public string MessageText { get; set; }
        public DateTime MessageDateTime { get; set; }
    }
}
