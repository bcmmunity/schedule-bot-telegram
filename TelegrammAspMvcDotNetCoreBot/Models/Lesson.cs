using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelegrammAspMvcDotNetCoreBot.Models
{
	public class Lesson
	{
		public int Id { get; set; }
		public string Number { get; set; }
		public string Name { get; set; }
		public string Teacher { get; set; }
		public string Time { get; set; }
		public string Room { get; set; }
	}
}
