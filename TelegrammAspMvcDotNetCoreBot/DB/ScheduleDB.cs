using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using TelegrammAspMvcDotNetCoreBot.Models;

namespace TelegrammAspMvcDotNetCoreBot.DB
{

    public class ScheduleDB
    {
      //  private readonly MyContext _db;


        string connectionString;
        public ScheduleDB()
        {
            DB db = new DB();
            connectionString = db.GetConnectionString();
           // _db = db.Connect();
        }

        public bool IsUniversityExist(string university)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return Convert.ToBoolean(db.Execute("SELECT * FROM Universities WHERE Name = @university", new { university }));
            }

            //University universitym = _db.Universities.FirstOrDefault(m => m.Name == university);

            //bool result = universitym != null;

            //return result;
        }

        public bool IsFacilityExist(string university, string facility)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return Convert.ToBoolean(db.Execute(
                        "SELECT f.FacilityId, f.Name, f.UniversityId FROM Facilities as f JOIN Universities as u on f.UniversityId = u.UniversityId where u.Name = @university and f.Name = @facility",
                        new {university, facility}));
            }
            //University universitym = _db.Universities.FirstOrDefault(m => m.Name == university);

            //Facility facultym = _db.Facilities.Where(l => l.University == universitym)
            //    .FirstOrDefault(t => t.Name == facility);

            //bool result = facultym != null;

            //return result;
        }

        public bool IsCourseExist(string university, string facility, string course)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return Convert.ToBoolean(db.Execute(
                    "SELECT c.CourseId, c.Name, c.FacilityId FROM Courses as c JOIN Facilities as f on c.FacilityId = f.FacilityId JOIN Universities as u on f.UniversityId = u.UniversityId where u.Name = @university and f.Name = @facility and c.Name = @course",
                    new {university, facility, course}));
            }
            //University universitym = _db.Universities.FirstOrDefault(m => m.Name == university);

            //Facility facultym = _db.Facilities.Where(l => l.University == universitym)
            //    .FirstOrDefault(t => t.Name == facility);

            //Course coursem = _db.Courses.Where(o => o.Facility == facultym).FirstOrDefault(t => t.Name == course);

            //bool result = coursem != null;

            //return result;
        }

        public bool IsGroupExist(string university, string facility, string course, string group)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return Convert.ToBoolean(db.Execute(
                    "SELECT g.GroupId, g.Name, g.ScheduleType, g.CourseId FROM Groups as g JOIN Courses as c on c.CourseId = g.CourseId JOIN Facilities as f on c.FacilityId = f.FacilityId JOIN Universities as u on f.UniversityId = u.UniversityId where u.Name = @university and f.Name = @facility and c.Name = @course and g.Name = @group",
                    new {university, facility, course, group}));
            }

            //University universitym = _db.Universities.FirstOrDefault(m => m.Name == university);

            //Facility facultym = _db.Facilities.Where(l => l.University == universitym)
            //    .FirstOrDefault(t => t.Name == facility);

            //Course coursem = _db.Courses.Where(o => o.Facility == facultym)
            //    .FirstOrDefault(t => t.Name == course);

            //Group groupm = _db.Groups.Where(n => n.Course == coursem)
            //    .FirstOrDefault(t => t.Name == group);

            //bool result = groupm != null;

            //return result;
        }

        public bool IsTeacherExist(string name)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                Teacher searchingTeacher = db.QueryFirstOrDefault<Teacher>("SELECT * From Teachers where Name = @name", new {name});
                if (searchingTeacher != null)
                    return true;
                return false;
            }
            //if (_db.Teachers.FirstOrDefault(t => t.Name == name) != null)
            //    return true;
            //return false;
        }

        public List<Teacher> TeachersSearch(string name)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                if (db.QueryFirstOrDefault<Teacher>("SELECT * From Teachers where Name LIKE '%' + @name + '%'", new {name}) != null)
                {
                    List<Teacher> list = db.Query<Teacher>("SELECT * From Teachers where Name LIKE '%' + @name + '%'", new { name }).ToList();
                    return list.GroupBy(x => x.Name).Select(x => x.First()).ToList();
                }

                return new List<Teacher>();
            }
            //if (_db.Teachers.FirstOrDefault(t => t.Name.Contains(name)) != null)
            //{
            //    List<Teacher> list = _db.Teachers.Where(t => t.Name.Contains(name)).ToList();
            //    return list.GroupBy(x => x.Name).Select(x => x.First()).ToList();
            //}

            //return new List<Teacher>();
        }

        public List<string> GetUniversities()
        {
            
           
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                List<University> source = db.Query<University>("SELECT * FROM Universities").ToList();
                List<string> result = new List<string>();
                foreach (University item in source)
                {
                    result.Add(item.Name);
                }

                return result;
            }

            //List<University> source = _db.Universities.ToList();
            //List<string> result = new List<string>();
            //foreach (University item in source)
            //{
            //    result.Add(item.Name);
            //}

            //return result;
        }

        public List<string> GetFacilities(string university)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                List<Facility> source = db.Query<Facility>(
                    "SELECT f.FacilityId, f.Name, f.UniversityId FROM Facilities as f JOIN Universities as u on f.UniversityId = u.UniversityId where u.Name = @university",
                    new { university }).ToList();
                List<string> result = new List<string>();
                foreach (Facility item in source)
                {
                    result.Add(item.Name);
                }

                return result;
            }
            //List<string> result = new List<string>();

            //University universitym = _db.Universities.FirstOrDefault(m => m.Name == university);

            //List<Facility> source = _db.Facilities.Where(n => n.University == universitym).ToList();

            //foreach (Facility item in source)
            //{
            //    result.Add(item.Name);
            //}

            //return result;
        }

        public List<string> GetCourses(string university, string facility)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                List<Course> source = db.Query<Course>(
                    "SELECT c.CourseId, c.Name, c.FacilityId FROM Courses as c JOIN Facilities as f on c.FacilityId = f.FacilityId JOIN Universities as u on f.UniversityId = u.UniversityId where u.Name = @university and f.Name = @facility",
                    new { university, facility}).ToList();
                List<int> sortList = new List<int>();
                List<string> result = new List<string>();

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
            //University universitym = _db.Universities.FirstOrDefault(m => m.Name == university);
            //Facility facultym = _db.Facilities.Where(l => l.University == universitym)
            //    .FirstOrDefault(t => t.Name == facility);

            //List<string> result = new List<string>();
            //List<Course> source = _db.Courses.Where(n => n.Facility == facultym).ToList();
            //List<int> sortList = new List<int>();

            //foreach (Course item in source)
            //{
            //    sortList.Add(Convert.ToInt32(item.Name));
            //}
            //sortList.Sort();

            //foreach (int item in sortList)
            //{
            //    result.Add(item.ToString());
            //}
            //return result;
        }

        public List<string> GetGroups(string university, string facility, string course)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                List<Group> source = db.Query<Group>(
                    "SELECT g.GroupId, g.Name, g.ScheduleType, g.CourseId FROM Groups as g JOIN Courses as c on c.CourseId = g.CourseId JOIN Facilities as f on c.FacilityId = f.FacilityId JOIN Universities as u on f.UniversityId = u.UniversityId where u.Name = @university and f.Name = @facility and c.Name = @course",
                    new { university, facility, course}).ToList();
                List<string> result = new List<string>();

                foreach (Group item in source)
                {
                    result.Add(item.Name);
                }

                return result;
            }
            //University universitym = _db.Universities.FirstOrDefault(m => m.Name == university);
            //Facility facultym = _db.Facilities.Where(l => l.University == universitym)
            //    .FirstOrDefault(t => t.Name == facility);
            //Course coursem = _db.Courses.Where(o => o.Facility == facultym)
            //    .FirstOrDefault(t => t.Name == course);

            
            //List<Group> source = _db.Groups.Where(n => n.Course == coursem).ToList();

            //foreach (Group item in source)
            //{
            //    result.Add(item.Name);
            //}

            //return result;
        }

        public string GeTeacher(Lesson lesson)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.QueryFirstOrDefault("SELECT TeachersNames FROM Lessons WHERE LessonId = @lessonId",
                    new { lessonId = lesson.LessonId }).ToString();
            }
            //return _db.Lessons.FirstOrDefault(l => l.LessonId == lesson.LessonId)?.TeachersNames;

        }
        public List<Lesson> GetSchedule(string university, string facility, string course, string group, int week,
            int day)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<Lesson>(
                    "SELECT l.* FROM ScheduleDays as sd JOIN ScheduleWeeks as sw on sd.ScheduleWeekId = sw.ScheduleWeekId JOIN Groups as g on g.GroupId = sw.GroupId JOIN Courses as c ON c.CourseId = g.CourseId JOIN Facilities as f ON f.FacilityId = c.FacilityId JOIN Universities as u ON u.UniversityId = f.UniversityId JOIN Lessons as l on l.ScheduleDayId = sd.ScheduleDayId WHERE u.Name = @university and f.Name = @facility and c.Name = @course and g.Name = @group and sw.Week = @week and sd.Day = @day",
                    new {university, facility, course, group, week, day}).ToList();
            }

            //List<ScheduleDay> li = _db.ScheduleWeeks.Include(v => v.Day).Where(l => l.Group.Course.Facility.University.Name == university).Where(k => k.Group.Course.Facility.Name == facility).Where(j => j.Group.Course.Name == course).Where(n => n.Group.Name == group).FirstOrDefault(m => m.Week == week)
            //    ?.Day.ToList();

            //return _db.ScheduleDays.Include(r => r.Lesson).FirstOrDefault(f => f == li.FirstOrDefault(w => w.Day == day));
        }

        public List<Lesson> GetTeacherSchedule(string teacher, int week,
            int day)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                List<Lesson> alLessons = db.Query<Lesson>(
                    "SELECT l.* FROM ScheduleDays as sd JOIN ScheduleWeeks as sw on sw.ScheduleWeekId = sd.ScheduleWeekId JOIN Lessons as l on l.ScheduleDayId = sd.ScheduleDayId JOIN TeacherLesson as tl on tl.LessonId = l.LessonId JOIN Teachers as t on t.TeacherId = tl.TeacherId WHERE t.Name = @teacher and sw.Week = @week and sd.Day = @day",
                    new { teacher, week, day }).ToList();
                List<string> timeList = new List<string>();
                List<Lesson> result = new List<Lesson>();
                foreach (var lesson in alLessons)
                {
                    if (!timeList.Contains(lesson.Time))
                    {
                        timeList.Add(lesson.Time);
                        result.Add(lesson);
                    }
                }

                return result;
            }
            //List<Lesson> listPar = new List<Lesson>();

            //List<Lesson> lessons = _db.Lessons.Include(t => t.TeacherLessons).Where(t => t.TeacherLessons.FirstOrDefault(l=>l.Teacher.Name==teacher) != null).ToList();
            //List<ScheduleDay> scheduleDays = _db.ScheduleDays.Include(l => l.Lesson).Where(d => d.Day == day).ToList();

            //List<string> previousLessons = new List<string>();
            //foreach (var scDay in scheduleDays)
            //{
            //    foreach (var lesson in scDay.Lesson)
            //    {
            //        if (lessons.Contains(lesson) && !previousLessons.Contains(lesson.Number) &&
            //            _db.ScheduleWeeks.Include(d => d.Day).FirstOrDefault(d => d.Day.Contains(scDay))?.Week == week)
            //        {
            //            listPar.Add(lesson);
            //            previousLessons.Add(lesson.Number);
            //        }


            //    }



            //}


            //ScheduleDay scheduleDay = new ScheduleDay
            //{
            //    Day = day,
            //    Lesson = listPar
            //};
            //return scheduleDay;
        }

      
    }
}