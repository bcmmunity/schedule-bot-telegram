using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TelegrammAspMvcDotNetCoreBot.Models;

namespace TelegrammAspMvcDotNetCoreBot.DB
{
    public class ScheduleDB
    {
        private readonly MyContext _db;

        public ScheduleDB()
        {
            _db = new DB().Connect();
        }

        public bool IsUniversityExist(string university)
        {
            University universitym = _db.Universities.FirstOrDefault(m => m.Name == university);

            bool result = universitym != null;

            return result;
        }

        public bool IsFacilityExist(string university, string facility)
        {
            University universitym = _db.Universities.FirstOrDefault(m => m.Name == university);

            Facility facultym = _db.Facilities.Where(l => l.University == universitym)
                .FirstOrDefault(t => t.Name == facility);

            bool result = facultym != null;

            return result;
        }

        public bool IsCourseExist(string university, string facility, string course)
        {
            University universitym = _db.Universities.FirstOrDefault(m => m.Name == university);

            Facility facultym = _db.Facilities.Where(l => l.University == universitym)
                .FirstOrDefault(t => t.Name == facility);

            Course coursem = _db.Courses.Where(o => o.Facility == facultym).FirstOrDefault(t => t.Name == course);

            bool result = coursem != null;

            return result;
        }

        public bool IsGroupExist(string university, string facility, string course, string group)
        {
            University universitym = _db.Universities.FirstOrDefault(m => m.Name == university);

            Facility facultym = _db.Facilities.Where(l => l.University == universitym)
                .FirstOrDefault(t => t.Name == facility);

            Course coursem = _db.Courses.Where(o => o.Facility == facultym)
                .FirstOrDefault(t => t.Name == course);

            Group groupm = _db.Groups.Where(n => n.Course == coursem)
                .FirstOrDefault(t => t.Name == group);

            bool result = groupm != null;

            return result;
        }



        public List<string> GetUniversities()
        {
            List<string> result = new List<string>();
            List<University> source = _db.Universities.ToList();

            foreach (University item in source)
            {
                result.Add(item.Name);
            }

            return result;
        }

        public List<string> GetFacilities(string university)
        {
            List<string> result = new List<string>();

            University universitym = _db.Universities.FirstOrDefault(m => m.Name == university);

            List<Facility> source = _db.Facilities.Where(n => n.University == universitym).ToList();

            foreach (Facility item in source)
            {
                result.Add(item.Name);
            }

            return result;
        }

        public List<string> GetCourses(string university, string facility)
        {
            University universitym = _db.Universities.FirstOrDefault(m => m.Name == university);
            Facility facultym = _db.Facilities.Where(l => l.University == universitym)
                .FirstOrDefault(t => t.Name == facility);

            List<string> result = new List<string>();
            List<Course> source = _db.Courses.Where(n => n.Facility == facultym).ToList();

            foreach (Course item in source)
            {
                result.Add(item.Name);
            }

            return result;
        }

        public List<string> GetGroups(string university, string facility, string course)
        {
            University universitym = _db.Universities.FirstOrDefault(m => m.Name == university);
            Facility facultym = _db.Facilities.Where(l => l.University == universitym)
                .FirstOrDefault(t => t.Name == facility);
            Course coursem = _db.Courses.Where(o => o.Facility == facultym)
                .FirstOrDefault(t => t.Name == course);

            List<string> result = new List<string>();
            List<Group> source = _db.Groups.Where(n => n.Course == coursem).ToList();

            foreach (Group item in source)
            {
                result.Add(item.Name);
            }

            return result;
        }

        public Teacher GeTeacher(Lesson lesson)
        {
            return _db.Lessons.Include(t => t.Teacher).FirstOrDefault(l => l.LessonId == lesson.LessonId).Teacher;

        }
        public ScheduleDay GetSchedule(string university, string facility, string course, string group, int week,
            int day)
        {
            University universitym = _db.Universities.FirstOrDefault(m => m.Name == university);

            Facility facultym = _db.Facilities.Where(l => l.University == universitym)
                .FirstOrDefault(t => t.Name == facility);

            Course coursem = _db.Courses.Where(o => o.Facility == facultym)
                .FirstOrDefault(t => t.Name == course);

            Group groupm = _db.Groups.Where(n => n.Course == coursem)
                .FirstOrDefault(t => t.Name == group);

            List<ScheduleDay> li = _db.ScheduleWeeks
                .Include(v => v.Day)
                .Where(n => n.Group == groupm)
                .FirstOrDefault(m => m.Week == week)
                ?.Day.ToList();

            return _db.ScheduleDays.Include(r => r.Lesson)
                .FirstOrDefault(f => f.ScheduleDayId == li.FirstOrDefault(n => n.Day == day).ScheduleDayId);
        }

    }
}