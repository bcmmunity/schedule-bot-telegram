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
        string connectionString;
        public ScheduleDB()
        {
            DB db = new DB();
            connectionString = db.GetConnectionString();
        }

        public bool IsUniversityExist(string university)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                University universitym = db.QueryFirstOrDefault<University>("SELECT * FROM Universities WHERE Name = @university", new { university });
                bool result = universitym != null;

                return result;
            }

        }

        public bool IsFacilityExist(string university, string facility)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                Facility facultym = db.QueryFirstOrDefault<Facility>(
                        "SELECT f.FacilityId, f.Name, f.UniversityId FROM Facilities as f JOIN Universities as u on f.UniversityId = u.UniversityId where u.Name = @university and f.Name = @facility",
                        new {university, facility});
                bool result = facultym != null;

                return result;
            }
        }

        public bool IsCourseExist(string university, string facility, string course)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                Course coursem = db.QueryFirstOrDefault<Course>(
                    "SELECT c.CourseId, c.Name, c.FacilityId FROM Courses as c JOIN Facilities as f on c.FacilityId = f.FacilityId JOIN Universities as u on f.UniversityId = u.UniversityId where u.Name = @university and f.Name = @facility and c.Name = @course",
                    new {university, facility, course});
                bool result = coursem != null;

                return result;
            }
        }

        public bool IsGroupExist(string university, string facility, string course, string group)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                Group groupm = db.QueryFirstOrDefault<Group>(
                    "SELECT g.GroupId, g.Name, g.ScheduleType, g.CourseId FROM Groups as g JOIN Courses as c on c.CourseId = g.CourseId JOIN Facilities as f on c.FacilityId = f.FacilityId JOIN Universities as u on f.UniversityId = u.UniversityId where u.Name = @university and f.Name = @facility and c.Name = @course and g.Name = @group",
                    new { university, facility, course, group });
                bool result = groupm != null;

                return result;
            }
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
        }

      
    }
}