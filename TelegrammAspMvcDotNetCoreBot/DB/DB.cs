using Microsoft.EntityFrameworkCore;
using TelegrammAspMvcDotNetCoreBot.Models;


namespace TelegrammAspMvcDotNetCoreBot.DB
{
    public class DB
    {
        public MyContext Connect()
        {
            var optionsBuilder = new DbContextOptionsBuilder<MyContext>();
           // optionsBuilder.UseSqlServer("Server=localhost;Database=u0641156_studystat;User Id=u0641156_studystat;Password=Stdstt1!;");
              optionsBuilder.UseSqlServer("Server=studystat.ru;Database=u0641156_studystat;User Id=u0641156_studystat;Password=Stdstt1!;");
           
            return new MyContext(optionsBuilder.Options);
        }
    }
}