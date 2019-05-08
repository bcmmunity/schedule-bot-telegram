using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using TelegrammAspMvcDotNetCoreBot.DB;
using TelegrammAspMvcDotNetCoreBot.Logic;
using TelegrammAspMvcDotNetCoreBot.Models;
using TelegrammAspMvcDotNetCoreBot.Models.VK;
using VkNet.Abstractions;
using VkNet.Model;
using VkNet.Model.RequestParams;
using VkNet.Utils;

namespace TelegrammAspMvcDotNetCoreBot.Controllers
{
    [Route("api/messagevk/update")]
    public class VkCallbackController : Controller
    {
        /// <summary>
        /// Конфигурация приложения
        /// </summary>
        private readonly IConfiguration _configuration;
        private readonly IVkApi _vkApi;

        private SnUserDb userDb = new SnUserDb("Vk");

        public VkCallbackController(IVkApi vkApi, IConfiguration configuration)
        {
            _vkApi = vkApi;
            _configuration = configuration;
        }


        // GET: /<controller>/
        [HttpGet]
        public string Get()
        {
            return "Method GET unavailable"; ;
        }



        [HttpPost]
        public IActionResult Callback([FromBody] Updates updates)
        {
            DateTime startTime = DateTime.Now;
            LoggingDB loggingDb = new LoggingDB();

            try
            {

                switch (updates.Type)
                {
                    case "confirmation":
                        return Ok(_configuration["Config:Confirmation"]);

                    case "message_new":
                    {
                        ResponseBulder response = new ResponseBulder("Vk");
                        ScheduleDB scheduleDb = new ScheduleDB();
                        VkKeyboard keyboard = new VkKeyboard();

                        // Десериализация
                        var message = Message.FromJson(new VkResponse(updates.Object));

                            var chatId = message.FromId ?? -1;

                        if (chatId == -1)
                            return Ok("ok");


                            if (!userDb.CheckUser(chatId) || message.Text == "Начать")
                        {
                            var universities = response.UniversitiesArray(chatId);
                            _vkApi.Messages.Send(new MessagesSendParams
                            {
                                RandomId = new DateTime().Millisecond,
                                PeerId = message.PeerId.Value,
                                Message = "Привет, выбери свой университет",
                                Keyboard = keyboard.GetKeyboard(universities)
                            });
                            return Ok("ok");
                        }

                        //deserialize
                        if (message.Payload != null)
                        {
                            ButtonPayload payload = JsonConvert.DeserializeObject<ButtonPayload>(message.Payload);


                            if (!string.IsNullOrEmpty(payload.Button))
                            {

                                int page = Convert.ToInt32(payload.Button.Split(';')[0]);
                                string course = payload.Button.Split(';')[1];


                                var groups = response.GroupsArray(chatId, course, page);

                                _vkApi.Messages.Send(new MessagesSendParams
                                {
                                    RandomId = new DateTime().Millisecond,
                                    PeerId = message.PeerId.Value,
                                    Message = "Переход на другую страницу",
                                    Keyboard = keyboard.GetKeyboard(groups, course)
                                });

                                return Ok("ok");
                            }
                            }
                           

                            //Основной режим 
                        if (userDb.CheckUserElements(chatId, "university") == "" &&
                            scheduleDb.IsUniversityExist(message.Text))
                        {
                            var facilities = response.FacilitiesArray(chatId, message.Text);

                            _vkApi.Messages.Send(new MessagesSendParams
                            {
                                RandomId = new DateTime().Millisecond,
                                PeerId = message.PeerId.Value,
                                Message = "Теперь выбери факультет",
                                Keyboard = keyboard.GetKeyboard(facilities)
                            });

                            return Ok("ok");
                        }

                        if (userDb.CheckUserElements(chatId, "facility") == "" &&
                            scheduleDb.IsFacilityExist(userDb.CheckUserElements(chatId, "university"), message.Text))
                        {
                            var courses = response.CoursesArray(chatId, message.Text);
                            _vkApi.Messages.Send(new MessagesSendParams
                            {
                                RandomId = new DateTime().Millisecond,
                                PeerId = message.PeerId.Value,
                                Message = "Теперь выбери курс",
                                Keyboard = keyboard.GetKeyboard(courses)
                            });

                            return Ok("ok");
                        }

                        if (userDb.CheckUserElements(chatId, "course") == "" && scheduleDb.IsCourseExist(
                                userDb.CheckUserElements(chatId, "university"),
                                userDb.CheckUserElements(chatId, "facility"),
                                message.Text))
                        {
                            var groups = response.GroupsArray(chatId, message.Text);

                            _vkApi.Messages.Send(new MessagesSendParams
                            {
                                RandomId = new DateTime().Millisecond,
                                PeerId = message.PeerId.Value,
                                Message = "Теперь выбери группу",
                                Keyboard = keyboard.GetKeyboard(groups, message.Text)
                            });

                            return Ok("ok");
                        }

                        if (userDb.CheckUserElements(chatId, "group") == "" && scheduleDb.IsGroupExist(
                                userDb.CheckUserElements(chatId, "university"),
                                userDb.CheckUserElements(chatId, "facility"),
                                userDb.CheckUserElements(chatId, "course"), message.Text))
                        {
                            response.LetsWork(chatId, message.Text);
                            _vkApi.Messages.Send(new MessagesSendParams
                            {
                                RandomId = new DateTime().Millisecond,
                                PeerId = message.PeerId.Value,
                                Message = "Отлично, можем работать!",
                                Keyboard = response.VkMainKeyboard
                            });

                            return Ok("ok");
                        }

                        if (message.Text == "Сегодня" && userDb.CheckUserElements(chatId, "group") != "")
                        {
                            string result = response.Today(chatId);

                            if (!result.Equals("Учебы нет"))
                                _vkApi.Messages.Send(new MessagesSendParams
                                {
                                    RandomId = new DateTime().Millisecond,
                                    PeerId = message.PeerId.Value,
                                    Message = result,
                                    Keyboard = response.VkMainKeyboard
                                });
                            else
                                _vkApi.Messages.Send(new MessagesSendParams
                                {
                                    RandomId = new DateTime().Millisecond,
                                    PeerId = message.PeerId.Value,
                                    Message = "Пар нет",
                                    Keyboard = response.VkMainKeyboard
                                });


                            loggingDb.AddRecordInLog(chatId, message.Text + " <Time of evaluation> = "+(DateTime.Now-startTime).Seconds, startTime);
                                return Ok("ok");

                        }

                        if (message.Text == "Завтра" && userDb.CheckUserElements(chatId, "group") != "")
                        {
                            string result = response.Tommorrow(chatId);

                            if (!result.Equals("Учебы нет"))
                            {
                                _vkApi.Messages.Send(new MessagesSendParams
                                {
                                    RandomId = new DateTime().Millisecond,
                                    PeerId = message.PeerId.Value,
                                    Message = result,
                                    Keyboard = response.VkMainKeyboard
                                });
                            }

                            else
                            {
                                _vkApi.Messages.Send(new MessagesSendParams
                                {
                                    RandomId = new DateTime().Millisecond,
                                    PeerId = message.PeerId.Value,
                                    Message = "Пар нет",
                                    Keyboard = response.VkMainKeyboard
                                });
                            }


                            loggingDb.AddRecordInLog(chatId, message.Text + " <Time of evaluation> = " + (DateTime.Now - startTime).Seconds, startTime);
                                return Ok("ok");

                        }

                        if (message.Text == "Расписание" && userDb.CheckUserElements(chatId, "group") != "")
                        {
                            //TODO
                            _vkApi.Messages.Send(new MessagesSendParams
                            {
                                RandomId = new DateTime().Millisecond,
                                PeerId = message.PeerId.Value,
                                Message = "Извините, данная функция пока находится в разработке",
                                Keyboard = response.VkMainKeyboard
                            });
                            return Ok("ok");
                        }

                        if (message.Text == "Что задали?" && userDb.CheckUserElements(chatId, "group") != "")
                        {
                            //TODO
                            _vkApi.Messages.Send(new MessagesSendParams
                            {
                                RandomId = new DateTime().Millisecond,
                                PeerId = message.PeerId.Value,
                                Message = "Извините, данная функция пока находится в разработке",
                                Keyboard = response.VkMainKeyboard
                            });
                            return Ok("ok");
                        }

                        if (message.Text == "Добавить ДЗ" && userDb.CheckUserElements(chatId, "group") != "")
                        {
                            //TODO
                            _vkApi.Messages.Send(new MessagesSendParams
                            {
                                RandomId = new DateTime().Millisecond,
                                PeerId = message.PeerId.Value,
                                Message = "Извините, данная функция пока находится в разработке",
                                Keyboard = response.VkMainKeyboard
                            });
                            return Ok("ok");
                        }

                        if (message.Text == "О пользователе" && userDb.CheckUserElements(chatId, "group") != "")
                        {
                            string result = "Информация о пользователе\n \n";
                            result += "Id: " + chatId + "\n";
                            result += "Институт: " + userDb.CheckUserElements(chatId, "university") + "\n";
                            result += "Факультет: " + userDb.CheckUserElements(chatId, "facility") + "\n";
                            result += "Курс: " + userDb.CheckUserElements(chatId, "course") + "\n";
                            result += "Группа: " + userDb.CheckUserElements(chatId, "group") + "\n";

                            _vkApi.Messages.Send(new MessagesSendParams
                            {
                                RandomId = new DateTime().Millisecond,
                                PeerId = message.PeerId.Value,
                                Message = result,
                                Keyboard = response.VkMainKeyboard
                            });
                            return Ok("ok");
                        }

                        if (message.Text == "Сбросить")
                        {
                            var universities = response.UniversitiesArray(chatId);
                            _vkApi.Messages.Send(new MessagesSendParams
                            {
                                RandomId = new DateTime().Millisecond,
                                PeerId = message.PeerId.Value,
                                Message = "Привет, выбери свой университет",
                                Keyboard = keyboard.GetKeyboard(universities)
                            });

                            loggingDb.AddRecordInLog(chatId, message.Text + " <Time of evaluation> = " + (DateTime.Now - startTime).Seconds, startTime);
                                return Ok("ok");
                        }

                        _vkApi.Messages.Send(new MessagesSendParams
                        {
                            RandomId = new DateTime().Millisecond,
                            PeerId = message.PeerId.Value,
                            Message = "Извините, такой команды я не знаю",
                            Keyboard = response.VkMainKeyboard
                        });

                        break;
                    }

                }


                return Ok("ok");

            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                if (updates != null)
                {
                    ErrorLoggingDB errorLoggingDb = new ErrorLoggingDB();
                    var message = Message.FromJson(new VkResponse(updates.Object));

                    var chatId = message.FromId ?? -1;

                    _vkApi.Messages.Send(new MessagesSendParams
                    {
                        RandomId = new DateTime().Millisecond,
                        PeerId = message.PeerId.Value,
                        Message = "Хм, что-то пошло не так"
                    });

                    errorLoggingDb.AddErrorInLog(chatId, "Message", message.Text, e.Source+": "+e.Message, DateTime.Now);
                }

                return Ok("ok");
            }
        }
    }
}
