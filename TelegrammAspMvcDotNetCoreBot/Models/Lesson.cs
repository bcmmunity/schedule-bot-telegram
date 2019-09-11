namespace TelegrammAspMvcDotNetCoreBot.Models
{
	public class Lesson
	{
		public int LessonId { get; set; }
		public string Number { get; set; }
		public string Name { get; set; }
		public Teacher Teacher { get; set; }
        public string Type { get; set; }
		public string Time { get; set; }
		public string Room { get; set; }
	}
}
