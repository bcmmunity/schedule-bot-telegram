namespace TelegrammAspMvcDotNetCoreBot.Models
{
    public class HomeWork
    {
        public int HomeworkId { get; set; }
        public Group Group { get; set; }
        public string Date { get; set; }
        public string HomeWorkText { get; set; }
    }
}