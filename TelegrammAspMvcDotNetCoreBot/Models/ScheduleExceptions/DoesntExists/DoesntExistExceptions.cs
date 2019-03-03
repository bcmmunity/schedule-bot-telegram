using System;

namespace TelegrammAspMvcDotNetCoreBot.Models.ScheduleExceptions.DoesntExists
{
    
    public class UniversityDoesntExistsException: Exception
    {
        public UniversityDoesntExistsException() : base("Такого вуза не существует")
        {
        }
    }
    
    public class FacultyDoesntExistsException: Exception
    {
        public FacultyDoesntExistsException() : base("Такого факультета не существует!")
        {
        }
    }
    
    public class CourseDoesntExistsException: Exception
    {
        public CourseDoesntExistsException() : base("Такого курса не существует")
        {
        }  
    }
    
    public class GroupDoesntExistsException: Exception
    {
        public GroupDoesntExistsException() : base("Такой группы не существует")
        {
        }
    }
    
    public class WeekDoesntExistsException: Exception
    {
        public WeekDoesntExistsException() : base("Такой недели не существует")
        {
        }
    }
}