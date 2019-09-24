using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelegrammAspMvcDotNetCoreBot.Models
{
    public class UserTeacherSchedule
    {
        public UserTeacherSchedule()
        {
            IsActive = false;
        }
        public long ChatId { get; set; }
        public bool IsActive { get; set; }
        public string TeacherName { get; set; }
    }
}
