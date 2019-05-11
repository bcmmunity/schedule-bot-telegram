using System;
using System.Collections.Generic;
using TelegrammAspMvcDotNetCoreBot.Models;

namespace TelegrammAspMvcDotNetCoreBot.Logic
{
    public class LessonIComparer<T> : IComparer<T>
        where T : Lesson
    {
        // Реализуем интерфейс IComparer<T>
        public int Compare(T x, T y)
        {
            if (y != null && (x != null && Convert.ToInt32(x.Number) > Convert.ToInt32(y.Number)))
                return 1;
            if (y != null && (x != null && Convert.ToInt32(x.Number) < Convert.ToInt32(y.Number)))
                return -1;

            return 0;
        }

    }
}
