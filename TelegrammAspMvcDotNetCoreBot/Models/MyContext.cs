using Microsoft.EntityFrameworkCore;

namespace TelegrammAspMvcDotNetCoreBot.Models
{
	public sealed class MyContext : DbContext
	{

		public MyContext(DbContextOptions<MyContext> options)
			: base(options)
		{
			Database.EnsureCreated();
		}

		public DbSet<University> Universities { get; set; }
		public DbSet<Facility> Facilities { get; set; }
		public DbSet<Course> Courses { get; set; }
		public DbSet<Group> Groups { get; set; }
		public DbSet<ScheduleWeek> ScheduleWeeks { get; set; }
		public DbSet<ScheduleDay> ScheduleDays { get; set; }
		public DbSet<Lesson> Lessons { get; set; }
		public DbSet<HomeWork> HomeWorks { get; set; }
	    public DbSet<SnUser> SnUsers { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }
        public DbSet<StatisticLog> StatisticLogs { get; set; }
    }
}
