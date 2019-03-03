using System;

namespace TelegrammAspMvcDotNetCoreBot.Models.ScheduleExceptions.AlreadyExists
{
    public class UniversityAlreadyExistsException : Exception
    {
        public UniversityAlreadyExistsException() : base("Такой университет уже существует")
        {
        }
    }

    public class FacultyAlreadyExistsException : Exception
    {
        public FacultyAlreadyExistsException() : base("Такой факультет уже существует")
        {
        }
    }

    public class CourseAlreadyExistsException : Exception
    {
        public CourseAlreadyExistsException() : base("Такой курс уже существует")
        {
        }
    }

    public class GroupAlreadyExistsException : Exception
    {
        public GroupAlreadyExistsException() : base("Такая группа уже существует")
        {
        }
    }
}