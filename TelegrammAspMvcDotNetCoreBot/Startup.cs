using System.Net;
using System.Threading;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TelegrammAspMvcDotNetCoreBot.DB;
using TelegrammAspMvcDotNetCoreBot.Models;
using TelegrammAspMvcDotNetCoreBot.Models.Telegramm;
using VkNet;
using VkNet.Abstractions;
using VkNet.Model;

namespace TelegrammAspMvcDotNetCoreBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            keepAliveThread.Start();
        }

        public static IConfiguration Configuration { get; private set; }
        static Thread keepAliveThread = new Thread(KeepAlive);

        static void KeepAlive()
        {
            while (true)
            {
                WebRequest req = WebRequest.Create("http://studystat.ru/");
                req.GetResponse();
                try
                {
                    Thread.Sleep(60000);
                }
                catch (ThreadAbortException)
                {
                    break;
                }
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // получаем строку подключения из файла конфигурации
            string connection = Configuration.GetConnectionString("DefaultConnection");
            // добавляем контекст MyContext в качестве сервиса в приложение
            services.AddDbContext<MyContext>(options =>
                options.UseSqlServer(connection));
            services.AddMvc();

            services.AddSingleton<IVkApi>(sp =>
            {
                var api = new VkApi();
                api.Authorize(new ApiAuthParams { AccessToken = Configuration["ConfigVk:AccessToken"] });
                return api;
            });
        }

        private void OnShutdown()
        {
            keepAliveThread.Abort();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IApplicationLifetime applicationLifetime)
        {
            applicationLifetime.ApplicationStopping.Register(OnShutdown);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            // обработка ошибок HTTP
            app.UseStatusCodePages();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            Bot.GetBotClientAsync().Wait();
        }
    }
}
