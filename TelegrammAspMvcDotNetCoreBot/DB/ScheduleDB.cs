using System;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TelegrammAspMvcDotNetCoreBot.Logic;
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

        public bool IsTeacherExist(string name)
        {
            if (_db.Teachers.FirstOrDefault(t => t.Name == name) != null)
                return true;
            return false;
        }

        public List<Teacher> TeachersSearch(string name)
        {
            if (_db.Teachers.FirstOrDefault(t => t.Name.Contains(name)) != null)
            {
                List<Teacher> list = _db.Teachers.Where(t => t.Name.Contains(name)).ToList();
                return list.GroupBy(x => x.Name).Select(x => x.First()).ToList();
            }

            return new List<Teacher>();
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
            List<int> sortList = new List<int>();

            foreach (Course item in source)
            {
                sortList.Add(Convert.ToInt32(item.Name));
            }
            sortList.Sort();

            foreach (int item in sortList)
            {
                result.Add(item.ToString());
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
            //University universitym = _db.Universities.FirstOrDefault(m => m.Name == university);

            //Facility facultym = _db.Facilities.Where(l => l.University == universitym)
            //    .FirstOrDefault(t => t.Name == facility);

            //Course coursem = _db.Courses.Where(o => o.Facility == facultym)
            //    .FirstOrDefault(t => t.Name == course);

            //Group groupm = _db.Groups.Where(n => n.Course == coursem)
            //    .FirstOrDefault(t => t.Name == group);

            //List<ScheduleDay> li = _db.ScheduleWeeks
            //    .Include(v => v.Day)
            //    .Where(n => n.Group == groupm)
            //    .FirstOrDefault(m => m.Week == week)
            //    ?.Day.ToList();

            //return _db.ScheduleDays.Include(r => r.Lesson)
            //    .FirstOrDefault(f => f.ScheduleDayId == li.FirstOrDefault(n => n.Day == day).ScheduleDayId);

            List<ScheduleDay> li = _db.ScheduleWeeks.Include(v => v.Day).Where(l => l.Group.Course.Facility.University.Name == university).Where(k => k.Group.Course.Facility.Name == facility).Where(j => j.Group.Course.Name == course).Where(n => n.Group.Name == group).FirstOrDefault(m => m.Week == week)
                ?.Day.ToList();

            return _db.ScheduleDays.Include(r => r.Lesson).FirstOrDefault(f => f == li.FirstOrDefault(w => w.Day == day));
        }

        public ScheduleDay GetTeacherSchedule(string teacher, int week,
            int day)
        {
            List<Lesson> listPar = new List<Lesson>();

            List<Lesson> lessons = _db.Lessons.Include(t => t.Teacher).Where(t => t.Teacher.Name == teacher).ToList();
            List<ScheduleDay> scheduleDays = _db.ScheduleDays.Include(l => l.Lesson).Where(d => d.Day == day).ToList();

            List<string> previousLessons = new List<string>();
            foreach (var scDay in scheduleDays)
            {
                foreach (var lesson in scDay.Lesson)
                {
                    if (lessons.Contains(lesson) && !previousLessons.Contains(lesson.Number) && 
                        _db.ScheduleWeeks.Include(d => d.Day).FirstOrDefault(d => d.Day.Contains(scDay))?.Week == week)
                    {
                        listPar.Add(lesson);
                        previousLessons.Add(lesson.Number);
                    }
                        

                }
            }


            ScheduleDay scheduleDay = new ScheduleDay
            {
                Day = day,
                Lesson = listPar
            };
            return scheduleDay;
        }

      
    }
}