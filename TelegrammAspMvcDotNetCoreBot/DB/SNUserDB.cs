using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Dapper;
using MessagePack.Formatters;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using TelegrammAspMvcDotNetCoreBot.Models;

namespace TelegrammAspMvcDotNetCoreBot.DB
{
    internal class SnUserDb
    {
        // private readonly MyContext _db;
        string connectionString;
        private string SocialNetwork { get; }

        public SnUserDb(string socialNetwork)
        {
            //_db = new DB().Connect();
            DB db = new DB();
            connectionString = db.GetConnectionString();
            SocialNetwork = socialNetwork;
        }

        public void CreateUser(long userId)
        {
            if (CheckUser(userId)) return;

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                db.Execute("INSERT INTO SnUsers (SocialNetwork, SocialNetworkId) Values (@SocialNetwork, @userId)",
                    new {SocialNetwork, userId});
            }

            //    SnUser user = new SnUser
            //{
            //    SocialNetworkId = userId,
            //    SocialNetwork = SocialNetwork
            //};
            //_db.SnUsers.Add(user);
            //_db.SaveChanges();
        }

        public bool CheckUser(long userId) //Проверка существования пользователя
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                int isUserExists = db.QueryFirstOrDefault<int>(
                    "SELECT Count(*) FROM SnUsers WHERE SocialNetwork = @SocialNetwork AND SocialNetworkId = @userId",
                    new {SocialNetwork, userId});
                if (isUserExists != 0)
                    return true;
                return false;
            }
        }

        public byte GetUserScheduleType(long chatId)
        {
            string group = CheckUserElements(chatId, "group");
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.QueryFirstOrDefault<byte>("SELECT ScheduleType FROM Groups WHERE Name = @group", new {group});
            }

            //return _db.Groups.FirstOrDefault(g => g.Name == group).ScheduleType;
        }


        public void RecreateUser(long userId)
        {
            if (!CheckUser(userId)) return;
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                db.Execute(
                    "Update SnUsers set GroupId = null, CourseId = null, FacilityId = null, UniversityId = null where SocialNetworkId = @userId and SocialNetwork = @SocialNetwork",
                    new {userId, SocialNetwork});
            }
            //SnUser user = _db.SnUsers.Include(u=> u.University).Include(u => u.Course).Include(u => u.Facility).Include(u => u.Group).FirstOrDefault(n => n.SocialNetworkId == userId && n.SocialNetwork == SocialNetwork);
            //user.Group = null;
            //user.Course = null;
            //user.Facility = null;
            //user.University = null;

            //_db.SaveChanges();
        }

        public void EditUser(long userId, string type, string param)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                switch (type)
                {
                    case "university":
                    {
                        int universityId =
                            db.QueryFirstOrDefault<int>("SELECT UniversityId from Universities WHERE Name = @param",
                                new {param});
                        db.Execute(
                            "Update SnUsers set UniversityId = @universityId where SocialNetworkId = @userId and SocialNetwork = @SocialNetwork",
                            new {universityId, userId, SocialNetwork});
                        //_db.SnUsers.FirstOrDefault(n => n.SocialNetworkId == userId && n.SocialNetwork == SocialNetwork).University = _db.Universities.FirstOrDefault(n => n.Name == param);
                        //_db.SnUsers.Update(user);
                        break;
                    }

                    case "facility":
                    {
                        if (CheckUserElements(userId, "university") == "")
                            return;
                        int universityId =
                            db.QueryFirstOrDefault<int>(
                                "SELECT UniversityId from SnUsers WHERE SocialNetworkId = @userId and SocialNetwork = @SocialNetwork",
                                new {userId, SocialNetwork});
                        int facilityId =
                            db.QueryFirstOrDefault<int>(
                                "SELECT FacilityId from Facilities WHERE Name = @param AND UniversityId = @universityId",
                                new {param, universityId});
                        db.Execute(
                            "Update SnUsers set FacilityId = @facilityId where SocialNetworkId = @userId and SocialNetwork = @SocialNetwork",
                            new {facilityId, userId, SocialNetwork});
                        //Facility facility = _db.Facilities.FirstOrDefault(n => n.Name == param);
                        //user.Facility = facility;
                        //_db.SnUsers.Update(user);
                        break;
                    }

                    case "course":
                    {
                        if (CheckUserElements(userId, "facility") == "")
                            return;
                        int facilityId =
                            db.QueryFirstOrDefault<int>(
                                "SELECT FacilityId from SnUsers WHERE SocialNetworkId = @userId and SocialNetwork = @SocialNetwork",
                                new {userId, SocialNetwork});
                        int courseId =
                            db.QueryFirstOrDefault<int>(
                                "SELECT CourseId from Courses WHERE Name = @param AND FacilityId = @facilityId",
                                new {facilityId, param});
                        db.Execute(
                            "Update SnUsers set CourseId = @courseId where SocialNetworkId = @userId and SocialNetwork = @SocialNetwork",
                            new {courseId, userId, SocialNetwork});
                        //Course course = _db.Courses.FirstOrDefault(n => n.Name == param);
                        //user.Course = course;
                        //_db.SnUsers.Update(user);
                        break;
                    }

                    case "group":
                    {
                        if (CheckUserElements(userId, "course") == "")
                            return;
                        int courseId =
                            db.QueryFirstOrDefault<int>(
                                "SELECT CourseId from SnUsers WHERE SocialNetworkId = @userId and SocialNetwork = @SocialNetwork",
                                new {userId, SocialNetwork});
                        int groupId =
                            db.QueryFirstOrDefault<int>(
                                "SELECT GroupId from Groups WHERE Name = @param AND CourseId = @courseId",
                                new {courseId, param});
                        db.Execute(
                            "Update SnUsers set GroupId = @groupId where SocialNetworkId = @userId and SocialNetwork = @SocialNetwork",
                            new {groupId, userId, SocialNetwork});
                        //Group group = _db.Groups.FirstOrDefault(n => n.Name == param);
                        //user.Group = group;
                        //_db.SnUsers.Update(user);
                        break;
                    }

                    case "activity":
                    {
                        db.Execute(
                            "Update SnUsers set LastActiveDate = @dateNow where SocialNetworkId = @userId and SocialNetwork = @SocialNetwork",
                            new {dateNow = DateTime.Now, userId, SocialNetwork});
                        break;
                    }
                }
            }


            //SnUser user = _db.SnUsers.FirstOrDefault(n => n.SocialNetworkId == userId && n.SocialNetwork == SocialNetwork);
            //switch (type)
            //{
            //    case "university":
            //        _db.SnUsers.FirstOrDefault(n => n.SocialNetworkId == userId && n.SocialNetwork == SocialNetwork).University = _db.Universities.FirstOrDefault(n => n.Name == param);
            //        _db.SnUsers.Update(user);
            //        break;
            //    case "facility":
            //        Facility facility = _db.Facilities.FirstOrDefault(n => n.Name == param);
            //        user.Facility = facility;
            //        _db.SnUsers.Update(user);
            //        break;
            //    case "course":
            //        Course course = _db.Courses.FirstOrDefault(n => n.Name == param);
            //        user.Course = course;
            //        _db.SnUsers.Update(user);
            //        break;
            //    case "group":
            //        Group group = _db.Groups.FirstOrDefault(n => n.Name == param);
            //        user.Group = group;
            //        _db.SnUsers.Update(user);
            //        break;
            //}

            //_db.SaveChanges();
        }

        public string CheckUserElements(long userId, string type)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                switch (type)
                {
                    case "university":
                        return db.QueryFirstOrDefault<string>(
                            "SELECT u.Name FROM SnUsers as us JOIN Universities as u on u.UniversityId = us.UniversityId WHERE SocialNetworkId = @userId and SocialNetwork = @SocialNetwork",
                            new {userId, SocialNetwork});
                    //if (_db.SnUsers.Include(y => y.University).FirstOrDefault(n =>
                    //        n.SocialNetworkId == userId && n.SocialNetwork == SocialNetwork)?.University != null)
                    //    return _db.SnUsers.Include(y => y.University)
                    //        .FirstOrDefault(n => n.SocialNetworkId == userId && n.SocialNetwork == SocialNetwork)
                    //        .University.Name;
                    //break;
                    case "facility":
                        return db.QueryFirstOrDefault<string>(
                            "SELECT f.Name FROM SnUsers as us JOIN Facilities as f on f.FacilityId = us.FacilityId WHERE SocialNetworkId = @userId and SocialNetwork = @SocialNetwork",
                            new {userId, SocialNetwork});
                    //if (_db.SnUsers.Include(y => y.Facility).FirstOrDefault(n =>
                    //        n.SocialNetworkId == userId && n.SocialNetwork == SocialNetwork)?.Facility != null)
                    //    return _db.SnUsers.Include(y => y.Facility).FirstOrDefault(n =>
                    //        n.SocialNetworkId == userId && n.SocialNetwork == SocialNetwork).Facility.Name;
                    //break;
                    case "course":
                        return db.QueryFirstOrDefault<string>(
                            "SELECT co.Name FROM SnUsers as us JOIN Courses as co on co.CourseId = us.CourseId WHERE SocialNetworkId = @userId and SocialNetwork = @SocialNetwork",
                            new {userId, SocialNetwork});
                    //if (_db.SnUsers.Include(y => y.Course).FirstOrDefault(n =>
                    //        n.SocialNetworkId == userId && n.SocialNetwork == SocialNetwork)?.Course != null)
                    //    return _db.SnUsers.Include(y => y.Course).FirstOrDefault(n =>
                    //        n.SocialNetworkId == userId && n.SocialNetwork == SocialNetwork).Course.Name;
                    //break;
                    case "group":
                        return db.QueryFirstOrDefault<string>(
                            "SELECT g.Name FROM SnUsers as us JOIN Groups as g on g.GroupId = us.GroupId WHERE SocialNetworkId = @userId and SocialNetwork = @SocialNetwork",
                            new {userId, SocialNetwork});
                    //if (_db.SnUsers.Include(y => y.Group).FirstOrDefault(n =>
                    //        n.SocialNetworkId == userId && n.SocialNetwork == SocialNetwork)?.Group != null)
                    //    return _db.SnUsers.Include(y => y.Group).FirstOrDefault(n =>
                    //        n.SocialNetworkId == userId && n.SocialNetwork == SocialNetwork).Group.Name;
                    //break;
                }

                return "";
            }
        }

        public List<SnUser> GetUsers(string university, string facility, string course,
            string group)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                if (!string.IsNullOrEmpty(university) && !string.IsNullOrEmpty(facility) &&
                    !string.IsNullOrEmpty(course) &&
                    !string.IsNullOrEmpty(group))
                {
                    return db.Query<SnUser>(
                        "SELECT us.* FROM SnUsers as us JOIN Universities as u on u.UniversityId = us.UniversityId JOIN Facilities as f on f.FacilityId = us.FacilityId JOIN Courses as c on c.CourseId = us.CourseId JOIN Groups as g on g.GroupId = us.GroupId WHERE u.Name = @university AND f.Name = @facility AND c.Name = @course AND g.Name = @group AND SocialNetwork = @SocialNetwork",
                        new {university, facility, course, group, SocialNetwork}).ToList();
                    //return _db.SnUsers.Include(u => u.University).Include(u => u.Facility)
                    //    .Include(u => u.Course)
                    //    .Include(u => u.Group).Where(n => n.SocialNetwork == SocialNetwork)
                    //    .Where(u => u.University.Name == university).Where(u => u.Facility.Name == facility)
                    //    .Where(u => u.Course.Name == course).Where(u => u.Group.Name == group).ToList();
                }
                else if (!string.IsNullOrEmpty(university) && !string.IsNullOrEmpty(facility) &&
                         !string.IsNullOrEmpty(course))
                {
                    return db.Query<SnUser>(
                        "SELECT us.* FROM SnUsers as us JOIN Universities as u on u.UniversityId = us.UniversityId JOIN Facilities as f on f.FacilityId = us.FacilityId JOIN Courses as c on c.CourseId = us.CourseId WHERE u.Name = @university AND f.Name = @facility AND c.Name = @course AND SocialNetwork = @SocialNetwork",
                        new {university, facility, course, SocialNetwork}).ToList();
                    //return _db.SnUsers.Include(u => u.University).Include(u => u.Facility)
                    //    .Include(u => u.Course)
                    //    .Where(n => n.SocialNetwork == SocialNetwork).Where(u => u.University.Name == university)
                    //    .Where(u => u.Facility.Name == facility).Where(u => u.Course.Name == course).ToList();
                }
                else if (!string.IsNullOrEmpty(university) && !string.IsNullOrEmpty(facility))
                {
                    return db.Query<SnUser>(
                        "SELECT us.* FROM SnUsers as us JOIN Universities as u on u.UniversityId = us.UniversityId JOIN Facilities as f on f.FacilityId = us.FacilityId WHERE u.Name = @university AND f.Name = @facility AND c.Name = @course AND SocialNetwork = @SocialNetwork",
                        new {university, facility, SocialNetwork}).ToList();
                    //return _db.SnUsers.Include(u => u.University).Include(u => u.Facility)
                    //    .Where(n => n.SocialNetwork == SocialNetwork).Where(u => u.University.Name == university)
                    //    .Where(u => u.Facility.Name == facility).ToList();
                }
                else if (!string.IsNullOrEmpty(university))
                {
                    return db.Query<SnUser>(
                        "SELECT us.* FROM SnUsers as us JOIN Universities as u on u.UniversityId = us.UniversityId WHERE u.Name = @university AND SocialNetwork = @SocialNetwork",
                        new {university, SocialNetwork}).ToList();
                    //return _db.SnUsers.Include(u => u.University).Where(n => n.SocialNetwork == SocialNetwork)
                    //    .Where(u => u.University.Name == university).ToList();
                }

                return db.Query<SnUser>(
                    "SELECT * FROM SnUsers WHERE SocialNetwork = @SocialNetwork",
                    new {SocialNetwork}).ToList();
                //return _db.SnUsers.Where(n => n.SocialNetwork == SocialNetwork).ToList();
            }
        }

        public bool IsAliceUserInitialized()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                int count = db.QueryFirstOrDefault<int>(
                    "SELECT Count(*) FROM SnUsers WHERE SocialNetwork = @SocialNetwork",
                    new { SocialNetwork });
                if (count == 0)
                {
                    db.Execute(
                        "Insert into SnUsers (SocialNetwork, SocialNetworkId) values (@SocialNetwork, 0)",
                        new {SocialNetwork });
                }
                long chatId = db.QueryFirstOrDefault<long>(
                    "SELECT SocialNetworkId FROM SnUsers WHERE SocialNetwork = @SocialNetwork",
                    new {SocialNetwork});

                if (chatId != 0)
                    return true;
                return false;
            }
        }

        public bool IsAliceUserExists(long userId)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                int isUserExists = db.QueryFirstOrDefault<int>(
                    "SELECT Count(*) FROM SnUsers WHERE SocialNetworkId = @userId",
                    new {userId});
                if (isUserExists != 0)
                    return true;
                return false;
            }
        }

        public void CreateAliceUser(long userId)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                int universityId =
                    db.QueryFirstOrDefault<int>(
                        "SELECT UniversityId from SnUsers WHERE SocialNetworkId = @userId",
                        new {userId});
                int facilityId =
                    db.QueryFirstOrDefault<int>(
                        "SELECT FacilityId from SnUsers WHERE SocialNetworkId = @userId",
                        new {userId});
                int courseId =
                    db.QueryFirstOrDefault<int>(
                        "SELECT CourseId from SnUsers WHERE SocialNetworkId = @userId",
                        new {userId});
                int groupId =
                    db.QueryFirstOrDefault<int>(
                        "SELECT GroupId from SnUsers WHERE SocialNetworkId = @userId",
                        new {userId});

               db.Execute(
                        "Update SnUsers set SocialNetworkId = @userId, UniversityId = @universityId, FacilityId = @facilityId, CourseId = @courseId, GroupId = @groupId where SocialNetwork = @SocialNetwork",
                        new {userId, universityId, facilityId, courseId, groupId, SocialNetwork});
            }
        }

        public long GetAliceUserId(string userId)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.QueryFirstOrDefault<long>(
                    "Select SocialNetworkId From SnUsers where SocialNetwork = @SocialNetwork",
                    new {SocialNetwork});
            }

        }

        public void DeleteAliceUser(string userId)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                db.Execute(
                    "Update SnUsers set SocialNetworkId = 0, UniversityId = null, FacilityId = null, CourseId = null, GroupId = null where SocialNetwork = @SocialNetwork",
                    new {SocialNetwork});


            }
        }
    }
}
