using System.Linq;
using System.Collections.Generic;
using TelegrammAspMvcDotNetCoreBot.Models;
using Microsoft.EntityFrameworkCore;

namespace TelegrammAspMvcDotNetCoreBot.Logic
{
	public class ScheduleDB
	{
	    public ScheduleDB()
	    {
	        var optionsBuilder = new DbContextOptionsBuilder<MyContext>();
	         optionsBuilder.UseSqlServer("Server=localhost;Database=u0641156_studystat;User Id=u0641156_studystat;Password=Stdstt1!;");
	        //optionsBuilder.UseSqlServer("Server=studystat.ru;Database=u0641156_studystat;User Id=u0641156_studystat;Password=Stdstt1!;");
	        Db = new MyContext(optionsBuilder.Options);
	    }

	    public static MyContext Db;
		
		public void AddUniversity(string name)
		{
			if (!IsUniversityExist(name))
			{
			    University un = new University {Name = name};

			    Db.Universities.Add(un);
				Db.SaveChanges();
			}
		}

		public void AddFacility(string university, string name)
		{
			if (!IsFacilityExist(university, name))
			{
			    Facility facility = new Facility
			    {
			        Name = name,
			        University = Db.Universities.FirstOrDefault(n => n.Name == university)
			    };

			    Db.Facilities.Add(facility);
				Db.SaveChanges();
			}
		}

		public void AddCourse(string university, string facility, string name)
		{
			if (!IsCourseExist(university, facility, name))
			{
			    Course co = new Course
			    {
			        Name = name,
			        Facility = Db.Facilities
			            .Where(n => n.University == Db.Universities.FirstOrDefault(m => m.Name == university))
			                                                       .FirstOrDefault(x => x.Name == facility)
			    };

			    Db.Courses.Add(co);
				Db.SaveChanges();
			}
		}

		public void AddGroup(string university, string facility, string course, string name)
		{
			if (!IsGroupExist(university, facility, course, name))
			{
			    Group gr = new Group
			    {
			        Name = name,
			        Course = Db.Courses.Where(l => l.Facility == Db.Facilities
			                                           .Where(n => n.University == Db.Universities
			                                                           .FirstOrDefault(m => m.Name == university))
			                                           .FirstOrDefault(x => x.Name == facility))
			            .FirstOrDefault(x => x.Name == course)
			    };

			    Db.Groups.Add(gr);
				Db.SaveChanges();
			}
		}



		public void AddScheduleWeek(string university, string facility, string course, string group, ScheduleWeek week)
		{
			week.Group = Db.Groups.Where(c => c.Course == Db.Courses
			                      .Where(ll => ll.Facility == Db.Facilities
			                      .Where(n => n.University == Db.Universities
			                      .FirstOrDefault(m => m.Name == university))
			                      .FirstOrDefault(x => x.Name == facility))
			                      .FirstOrDefault(x => x.Name == course))
			                      .FirstOrDefault(v => v.Name == group);
			Db.ScheduleWeeks.Add(week);
			Db.SaveChanges();
		}
		


		public bool IsUniversityExist(string university)
		{
            University universitym = Db.Universities.FirstOrDefault(m => m.Name == university);

			bool result = universitym != null;

			return result;
		}

		public bool IsFacilityExist(string university, string facility)
		{
			University universitym = Db.Universities.FirstOrDefault(m => m.Name == university);

			Facility facultym = Db.Facilities.Where(l => l.University == universitym)
			                      .FirstOrDefault(t => t.Name == facility);

			bool result = facultym != null;

			return result;
		}

		public bool IsCourseExist(string university, string facility, string course)
		{
			University universitym = Db.Universities.FirstOrDefault(m => m.Name == university);

			Facility facultym = Db.Facilities.Where(l => l.University == universitym)
			                      .FirstOrDefault(t => t.Name == facility);

			Course coursem = Db.Courses.Where(o => o.Facility == facultym).FirstOrDefault(t => t.Name == course);

			bool result = coursem != null;

			return result;
		}

		public bool IsGroupExist(string university, string facility, string course, string group)
		{
			University universitym = Db.Universities.FirstOrDefault(m => m.Name == university);

			Facility facultym = Db.Facilities.Where(l => l.University == universitym)
			                      .FirstOrDefault(t => t.Name == facility);

			Course coursem = Db.Courses.Where(o => o.Facility == facultym)
			                           .FirstOrDefault(t => t.Name == course);

			Group groupm = Db.Groups.Where(n => n.Course == coursem)
			                        .FirstOrDefault(t => t.Name == group);

			bool result = groupm != null;
			
			return result;
		}



		public List<string> GetUniversities()
		{
			List<string> result = new List<string>();
			List<University> source = Db.Universities.ToList();

			foreach (University item in source)
			{
				result.Add(item.Name);
			}

			return result;
		}

		public List<string> GetFacilities(string university)
		{
			List<string> result = new List<string>();

			University universitym = Db.Universities.FirstOrDefault(m => m.Name == university);
			
			List<Facility> source = Db.Facilities.Where(n => n.University == universitym).ToList();

			foreach (Facility item in source)
			{
				result.Add(item.Name);
			}

			return result;
		}

		public List<string> GetCourses(string university, string facility)
		{
			University universitym = Db.Universities.FirstOrDefault(m => m.Name == university);
			Facility facultym = Db.Facilities.Where(l => l.University == universitym)
			                      .FirstOrDefault(t => t.Name == facility);
			
			List<string> result = new List<string>();
			List<Course> source = Db.Courses.Where(n => n.Facility == facultym).ToList();

			foreach (Course item in source)
			{
				result.Add(item.Name);
			}

			return result;
		}

		public List<string> GetGroups(string university, string facility, string course)
		{
			University universitym = Db.Universities.FirstOrDefault(m => m.Name == university);
			Facility facultym = Db.Facilities.Where(l => l.University == universitym)
			                      .FirstOrDefault(t => t.Name == facility);
			Course coursem = Db.Courses.Where(o => o.Facility == facultym)
			                   .FirstOrDefault(t => t.Name == course);

			List<string> result = new List<string>();
			List<Group> source = Db.Groups.Where(n => n.Course == coursem).ToList();

			foreach (Group item in source)
			{
				result.Add(item.Name);
			}

			return result;
		}

		public ScheduleDay GetSchedule(string university, string facility, string course, string group, int week, int day)
		{
			University universitym = Db.Universities.FirstOrDefault(m => m.Name == university);

			Facility facultym = Db.Facilities.Where(l => l.University == universitym)
			                      .FirstOrDefault(t => t.Name == facility);

			Course coursem = Db.Courses.Where(o => o.Facility == facultym)
			                           .FirstOrDefault(t => t.Name == course);

			Group groupm = Db.Groups.Where(n => n.Course == coursem)
			                        .FirstOrDefault(t => t.Name == group);

			List<ScheduleDay> li = Db.ScheduleWeeks.Include(c => c.Day)
			                                       .Where(n => n.Group == groupm)
			                                       .FirstOrDefault(m => m.Week == week).Day.ToList();
			
			return Db.ScheduleDays.Include(r => r.Lesson)
			                      .FirstOrDefault(f => f.Id == li.FirstOrDefault(n => n.Day == day).Id);
		}
	}
}