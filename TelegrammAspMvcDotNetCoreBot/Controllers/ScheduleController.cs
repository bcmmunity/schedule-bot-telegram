using System;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using TelegrammAspMvcDotNetCoreBot.Models.ScheduleExceptions.DoesntExists;
using TelegrammAspMvcDotNetCoreBot.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;

namespace TelegrammAspMvcDotNetCoreBot.Controllers
{
	public static class ScheduleController
	{
	   
		//static DbContextOptionsBuilder<MyContext> optionsBuilder = new DbContextOptionsBuilder<MyContext>().UseSqlite("Server=(localdb)\\mssqllocaldb;Database=mobilesdb;Trusted_Connection=True;");

		//public MyContext db = HttpContext.RequestServices.GetService<MyContext>();

		public static MyContext Db;
		//private static MyContext db = new MyContext(optionsBuilder.Options);
		
		public static void Unit()
		{
			var optionsBuilder = new DbContextOptionsBuilder<MyContext>();
		    optionsBuilder.UseSqlServer("Server=localhost;Database=u0641156_studystat;User Id=u0641156_studystat;Password=Stdstt1!;");
           //optionsBuilder.UseSqlServer("Server=vladafon.ru;Database=schedule-bot;User Id=sa;Password=Pizza2135;");
			Db = new MyContext(optionsBuilder.Options);
		}

		public static void AddUniversity(string name)
		{

			//MyContext dbContext = new MyContext(optionsBuilder.Options);
			//var controller = new ScheduleController(dbContext);

			if (!IsUniversityExist(name))
			{
				University un = new University();
				un.Name = name;

				Db.Universities.Add(un);
				Db.SaveChanges();
			}
		}

		public static void AddFacility(string university, string name)
		{
			if (!IsFacilityExist(university, name))
			{
				Facility facility = new Facility();
				facility.Name = name;
				facility.University = Db.Universities.Where(n => n.Name == university).FirstOrDefault();

				Db.Facilities.Add(facility);
				Db.SaveChanges();
			}
		}

		public static void AddCourse(string university, string facility, string name)
		{
			if (!IsCourseExist(university, facility, name))
			{
				Course co = new Course();
				co.Name = name;
				co.Facility = Db.Facilities.Where(n => n.University == Db.Universities.Where(m => m.Name == university).FirstOrDefault()).Where(x => x.Name == facility).FirstOrDefault();

				Db.Courses.Add(co);
				Db.SaveChanges();
			}
		}

		public static void AddGroup(string university, string facility, string course, string name)
		{
			if (!IsGroupExist(university, facility, course, name))
			{
				Group gr = new Group();
				gr.Name = name;
				gr.Course = Db.Courses.Where(l => l.Facility == Db.Facilities.Where(n => n.University == Db.Universities.Where(m => m.Name == university).FirstOrDefault()).Where(x => x.Name == facility).FirstOrDefault()).Where(x => x.Name == course).FirstOrDefault();

				Db.Groups.Add(gr);
				Db.SaveChanges();
			}
		}



		public static void AddScheduleWeek(string university, string facility, string course, string group, ScheduleWeek week)
		{
			week.Group = Db.Groups.Where(c => c.Course == Db.Courses.Where(ll => ll.Facility == Db.Facilities.Where(n => n.University == Db.Universities.Where(m => m.Name == university).FirstOrDefault()).Where(x => x.Name == facility).FirstOrDefault()).Where(x => x.Name == course).FirstOrDefault()).Where(v => v.Name == group).FirstOrDefault();
			Db.ScheduleWeeks.Add(week);
			Db.SaveChanges();
		}
		


		public static bool IsUniversityExist(string university)
		{
            University universitym = Db.Universities.FirstOrDefault(m => m.Name == university);

			bool result = false;

			if (universitym != null)
				result = true;

			return result;
		}

		public static bool IsFacilityExist(string university, string facility)
		{
			University universitym = Db.Universities.Where(m => m.Name == university).FirstOrDefault();

			Facility facultym = Db.Facilities.Where(l => l.University == universitym).Where(t => t.Name == facility).FirstOrDefault();

			bool result = false;

			if (facultym != null)
				result = true;

			return result;
		}

		public static bool IsCourseExist(string university, string facility, string course)
		{
			University universitym = Db.Universities.Where(m => m.Name == university).FirstOrDefault();

			Facility facultym = Db.Facilities.Where(l => l.University == universitym).Where(t => t.Name == facility).FirstOrDefault();

			Course coursem = Db.Courses.Where(o => o.Facility == facultym).Where(t => t.Name == course).FirstOrDefault();

			bool result = false;

			if (coursem != null)
				result = true;

			return result;
		}

		public static bool IsGroupExist(string university, string facility, string course, string group)
		{
			University universitym = Db.Universities.Where(m => m.Name == university).FirstOrDefault();

			Facility facultym = Db.Facilities.Where(l => l.University == universitym).Where(t => t.Name == facility).FirstOrDefault();

			Course coursem = Db.Courses.Where(o => o.Facility == facultym).Where(t => t.Name == course).FirstOrDefault();

			Group groupm = Db.Groups.Where(n => n.Course == coursem).Where(t => t.Name == group).FirstOrDefault();

			bool result = false;

			if (groupm != null)
				result = true;
			
			return result;
		}



		public static List<string> GetUniversities()
		{
			List<string> result = new List<string>();
			List<University> source = Db.Universities.ToList();

			foreach (University item in source)
			{
				result.Add(item.Name);
			}

			return result;
		}

		public static List<string> GetFacilities(string university)
		{
			List<string> result = new List<string>();

			University universitym = Db.Universities.Where(m => m.Name == university).FirstOrDefault();
			
			List<Facility> source = Db.Facilities.Where(n => n.University == universitym).ToList();

			foreach (Facility item in source)
			{
				result.Add(item.Name);
			}

			return result;
		}

		public static List<string> GetCourses(string university, string facility)
		{
			University universitym = Db.Universities.Where(m => m.Name == university).FirstOrDefault();
			Facility facultym = Db.Facilities.Where(l => l.University == universitym).Where(t => t.Name == facility).FirstOrDefault();
			
			List<string> result = new List<string>();
			List<Course> source = Db.Courses.Where(n => n.Facility == facultym).ToList();

			foreach (Course item in source)
			{
				result.Add(item.Name);
			}

			return result;
		}

		public static List<string> GetGroups(string university, string facility, string course)
		{
			University universitym = Db.Universities.Where(m => m.Name == university).FirstOrDefault();
			Facility facultym = Db.Facilities.Where(l => l.University == universitym).Where(t => t.Name == facility).FirstOrDefault();
			Course coursem = Db.Courses.Where(o => o.Facility == facultym).Where(t => t.Name == course).FirstOrDefault();

			List<string> result = new List<string>();
			List<Group> source = Db.Groups.Where(n => n.Course == coursem).ToList();

			foreach (Group item in source)
			{
				result.Add(item.Name);
			}

			return result;
		}

		public static ScheduleDay GetSchedule(string university, string facility, string course, string group, int week, int day)
		{
			University universitym = Db.Universities.Where(m => m.Name == university).FirstOrDefault();

			Facility facultym = Db.Facilities.Where(l => l.University == universitym).Where(t => t.Name == facility).FirstOrDefault();

			Course coursem = Db.Courses.Where(o => o.Facility == facultym).Where(t => t.Name == course).FirstOrDefault();

			Group groupm = Db.Groups.Where(n => n.Course == coursem).Where(t => t.Name == group).FirstOrDefault();

			List<ScheduleDay> li = Db.ScheduleWeeks.Include(c => c.Day).Where(n => n.Group == groupm).Where(m => m.Week == week).FirstOrDefault().Day.ToList();
			
			return Db.ScheduleDays.Include(r => r.Lesson).Where(f => f.Id == li.Where(n => n.Day == day).FirstOrDefault().Id).FirstOrDefault();
		}
	}
}