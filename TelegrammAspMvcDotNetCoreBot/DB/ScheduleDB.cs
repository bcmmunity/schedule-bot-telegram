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

        public void AddUniversity(string name)
        {
            if (!IsUniversityExist(name))
            {
                University un = new University { Name = name };

                _db.Universities.Add(un);
                _db.SaveChanges();
            }
        }

        public void AddFacility(string university, string name)
        {
            if (!IsFacilityExist(university, name))
            {
                Facility facility = new Facility
                {
                    Name = name,
                    University = _db.Universities.FirstOrDefault(n => n.Name == university)
                };

                _db.Facilities.Add(facility);
                _db.SaveChanges();
            }
        }

        public void AddCourse(string university, string facility, string name)
        {
            if (!IsCourseExist(university, facility, name))
            {
                Course co = new Course
                {
                    Name = name,
                    Facility = _db.Facilities
                        .Where(n => n.University == _db.Universities.FirstOrDefault(m => m.Name == university))
                        .FirstOrDefault(x => x.Name == facility)
                };

                _db.Courses.Add(co);
                _db.SaveChanges();
            }
        }

        public void AddGroup(string university, string facility, string course, string name)
        {
            if (!IsGroupExist(university, facility, course, name))
            {
                Group gr = new Group
                {
                    Name = name,
                    Course = _db.Courses.Where(l => l.Facility == _db.Facilities
                                                        .Where(n => n.University == _db.Universities
                                                                        .FirstOrDefault(m => m.Name == university))
                                                        .FirstOrDefault(x => x.Name == facility))
                        .FirstOrDefault(x => x.Name == course)
                };

                _db.Groups.Add(gr);
                _db.SaveChanges();
            }
        }



        public void AddScheduleWeek(string university, string facility, string course, string group, ScheduleWeek week)
        {
            week.Group = _db.Groups.Where(c => c.Course == _db.Courses
                                                   .Where(ll => ll.Facility == _db.Facilities
                                                                    .Where(n => n.University == _db.Universities
                                                                                    .FirstOrDefault(m =>
                                                                                        m.Name == university))
                                                                    .FirstOrDefault(x => x.Name == facility))
                                                   .FirstOrDefault(x => x.Name == course))
                .FirstOrDefault(v => v.Name == group);


            University universitym = _db.Universities.FirstOrDefault(m => m.Name == university);

            Facility facultym = _db.Facilities.Where(l => l.University == universitym)
                .FirstOrDefault(t => t.Name == facility);

            Course coursem = _db.Courses.Where(o => o.Facility == facultym)
                .FirstOrDefault(t => t.Name == course);

            Group groupm = _db.Groups.Where(n => n.Course == coursem)
                .FirstOrDefault(t => t.Name == group);

            ScheduleWeek oldScheduleWeek = _db.ScheduleWeeks
                .Include(v => v.Day)
                .Where(n => n.Group == groupm)
                .FirstOrDefault(m => m.Week == week.Week);

            if (oldScheduleWeek != null)
            {
                oldScheduleWeek.Day.Clear();


                foreach (var day in week.Day)
                {
                    oldScheduleWeek.Day.Add(day);
                }
            }

            else
                _db.ScheduleWeeks.Add(week);

            _db.SaveChanges();
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
                .FirstOrDefault(f => f.Id == li.FirstOrDefault(n => n.Day == day).Id);
        }
    }
}