using System;

namespace TelegrammAspMvcDotNetCoreBot.Models
{

    public class SnUser
    {
        public int SnUserId { get; set; }
        public long SocialNetworkId { get; set; }
        public string SocialNetwork { get; set; }
        public University University { get; set; }
        public Facility Facility { get; set; }
        public Course Course { get; set; }
        public Group Group { get; set; }
        public DateTime LastActiveDate { get; set; }
    }
}