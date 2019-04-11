using System.Collections.Generic;

namespace TelegrammAspMvcDotNetCoreBot.Models
{
	public class ScheduleWeek
	{
		public int Id { get; set; }
		public int Week { get; set; }
		public Group Group { get; set; }
		public ICollection<ScheduleDay> Day { get; set; }

		public ScheduleWeek()
		{
			Day = new List<ScheduleDay>();
		}
	}
}