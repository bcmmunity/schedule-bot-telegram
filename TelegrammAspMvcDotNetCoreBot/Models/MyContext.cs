using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace TelegrammAspMvcDotNetCoreBot.Models
{
	public class MyContext : DbContext
	{

		public MyContext(DbContextOptions<MyContext> options)
			: base(options)
		{
			Database.EnsureCreated();
		}

		public DbSet<University> Universities { get; set; }
		public DbSet<Faculty> Faculties { get; set; }
		public DbSet<Course> Courses { get; set; }
		public DbSet<Group> Groups { get; set; }
		public DbSet<ScheduleWeek> ScheduleWeeks { get; set; }
		public DbSet<ScheduleDay> ScheduleDays { get; set; }
		public DbSet<Lesson> Lessons { get; set; }
		public DbSet<HomeWork> HomeWorks { get; set; }
	}
}
