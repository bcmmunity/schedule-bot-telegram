using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using NPOI.HSSF.Record.Chart;
using TelegrammAspMvcDotNetCoreBot.DB;
using TelegrammAspMvcDotNetCoreBot.Logic;
using TelegrammAspMvcDotNetCoreBot.Models;

namespace TelegrammAspMvcDotNetCoreBot.Controllers
{
    public class HomeController : Controller
    {
        private readonly MyContext db;

        public HomeController(MyContext context)
        {
            db = context;
        }

        public IActionResult Details()
        {
            LoggingDB log = new LoggingDB();
            ViewBag.Statistic = log.GetDetails();
            return View();
        }


        public IActionResult Index()
        {
            //try
            //{
            //    int c = 1;
            //    ViewBag.Statistic += "1";
            //    List <Lesson> lessons = db.Lessons.Include(t=>t.Teacher).ToList();
            //    foreach (var lesson in lessons)
            //    {
            //        if (lesson.Teacher == null)
            //            continue;
            //        ViewBag.Statistic += "2";
            //           Teacher teacher = db.Teachers.Find(lesson.Teacher.TeacherId);
            //           ViewBag.Statistic += "3";
            //        Regex teacherRegex = new Regex(@"[А-Я][а-я]+\.[А-Я]\. ?[А-Я]\.");
            //        ViewBag.Statistic += "4";
            //        MatchCollection teachersMatches =
            //            teacherRegex.Matches(teacher.Name);
            //        ViewBag.Statistic += "5";
            //        if (teachersMatches.Count == 0)
            //        {
            //            ViewBag.Statistic += "6";
            //               lesson.TeacherLessons.Add(new TeacherLesson
            //            {TeacherLessonId = c,
            //                Lesson = lesson,
            //                LessonId = lesson.LessonId,
            //                Teacher = new Teacher
            //                {
            //                    Name = teacher.Name
            //                },
            //                TeacherId = lesson.Teacher.TeacherId
            //            });
            //               c++;
            //            lesson.TeachersNames = teacher.Name;
            //            db.Lessons.Update(lesson);
            //            db.SaveChanges();
            //            ViewBag.Statistic += "7";
            //        }
            //        else
            //        {
            //            ViewBag.Statistic += "8";
            //            string names = String.Empty;
            //            foreach (Match teachersMatch in teachersMatches)
            //            {
            //                ViewBag.Statistic += teachersMatch.Value;
            //                ViewBag.Statistic += "9";
            //                string name = teachersMatch.Value.Split('.')[0] + " " + teachersMatch.Value.Split('.')[1] +
            //                              ". " + teachersMatch.Value.Split('.')[2] + ".";
            //                names += name + ",";
            //                ViewBag.Statistic += "10";
            //                Teacher exisingTeacher = db.Teachers.FirstOrDefault(t => t.Name == teachersMatch.Value);
            //                ViewBag.Statistic += "11";
            //                if (exisingTeacher == null)
            //                {
            //                    lesson.TeacherLessons.Add(new TeacherLesson
            //                    {
            //                        TeacherLessonId = c,
            //                        Lesson = lesson,
            //                        LessonId = lesson.LessonId,
            //                        Teacher = new Teacher
            //                        {
            //                            Name = name
            //                        },
            //                        TeacherId = lesson.Teacher.TeacherId
            //                    });
            //                    c++;
            //                    ViewBag.Statistic += "12";
            //                }
            //                else
            //                {
            //                    lesson.TeacherLessons.Add(new TeacherLesson
            //                    {
            //                        TeacherLessonId = c,
            //                        Lesson = lesson,
            //                        LessonId = lesson.LessonId,
            //                        Teacher = exisingTeacher,
            //                        TeacherId = exisingTeacher.TeacherId
            //                    });
            //                    c++;
            //                    ViewBag.Statistic +="13";
            //                }
            //            }

            //            lesson.TeachersNames = names;
            //            db.Lessons.Update(lesson);
            //            db.SaveChanges();
            //        }
            //    }





            //}
            //catch (Exception e)
            //{
            //    ViewBag.Statistic += e.Message;
            //    ViewBag.Statistic += e.InnerException?.Message;
            //}
            LoggingDB log = new LoggingDB();
            ViewBag.Statistic = log.GetStatistic();
            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
