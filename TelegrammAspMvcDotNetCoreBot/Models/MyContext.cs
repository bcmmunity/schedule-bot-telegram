

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TeacherLesson>()
                .HasKey(us => new { us.TeacherId, us.LessonId });

            modelBuilder.Entity<TeacherLesson>()
                .HasOne(us => us.Teacher)
                .WithMany(e => e.TeacherLessons)
                .HasForeignKey(k => k.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<TeacherLesson>()
                .HasOne(us => us.Lesson)
                .WithMany(e => e.TeacherLessons)
                .HasForeignKey(k => k.LessonId)
                .OnDelete(DeleteBehavior.Cascade);
        }

        public DbSet<University> Universities { get; set; }
		public DbSet<Facility> Facilities { get; set; }
		public DbSet<Course> Courses { get; set; }
		public DbSet<Group> Groups { get; set; }
		public DbSet<ScheduleWeek> ScheduleWeeks { get; set; }
		public DbSet<ScheduleDay> ScheduleDays { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
		public DbSet<Lesson> Lessons { get; set; }
		public DbSet<HomeWork> HomeWorks { get; set; }
	    public DbSet<SnUser> SnUsers { get; set; }
        public DbSet<ErrorLog> ErrorLogs { get; set; }
        public DbSet<ActivityLog> ActivityLogs { get; set; }
    }
}
