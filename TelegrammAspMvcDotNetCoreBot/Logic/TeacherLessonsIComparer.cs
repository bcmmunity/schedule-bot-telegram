using System;
using System.Collections.Generic;
using TelegrammAspMvcDotNetCoreBot.Models;

namespace TelegrammAspMvcDotNetCoreBot.Logic
{
    public class TeacherLessonsIComparer<T> : IComparer<T>
        where T : Lesson
    {
        // Реализуем интерфейс IComparer<T>
        public int Compare(T x, T y)
        {
            DateTime xTime = DateTime.Parse(x?.Time.Split(" - ")[0]);
            DateTime yTime = DateTime.Parse(y?.Time.Split(" - ")[0]);


            if (y != null && (x != null && xTime > yTime))
                return 1;
            if (y != null && (x != null && xTime < yTime))
                return -1;

            return 0;
        }

    }
}