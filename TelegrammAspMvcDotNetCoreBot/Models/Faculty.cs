using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TelegrammAspMvcDotNetCoreBot.Models
{
	public class Faculty
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public University University { get; set; }
	}
}
