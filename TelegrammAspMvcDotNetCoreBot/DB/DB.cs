using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TelegrammAspMvcDotNetCoreBot.Models;


namespace TelegrammAspMvcDotNetCoreBot.DB
{
    public class DB
    {
        public IConfiguration AppConfiguration { get; set; }
        public DB()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
            // создаем конфигурацию
            AppConfiguration = builder.Build();
        }
        public MyContext Connect()
        {
            var optionsBuilder = new DbContextOptionsBuilder<MyContext>();
            optionsBuilder.UseSqlServer(AppConfiguration.GetConnectionString("DefaultConnection"));
          
            return new MyContext(optionsBuilder.Options);
        }
    }
}