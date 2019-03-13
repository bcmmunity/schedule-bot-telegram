namespace TelegrammAspMvcDotNetCoreBot.Models
{

    public class User
    {
        public int UserId { get; set; }
        public long TelegramId { get; set; }
        public University University { get; set; }
        public Facility Facility { get; set; }
        public Course Course { get; set; }
        public Group Group { get; set; }
    }
}