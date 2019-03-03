using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelegrammAspMvcDotNetCoreBot.Models
{
	public class ScheduleDay
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public int Day { get; set; }
		public ICollection<Lesson> Lesson { get; set; }

		public ScheduleDay()
		{
			Lesson = new List<Lesson>();
		}
	}
}
