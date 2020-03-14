using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using TelegrammAspMvcDotNetCoreBot.DB;
using TelegrammAspMvcDotNetCoreBot.Logic;
using TelegrammAspMvcDotNetCoreBot.Models;
using TelegrammAspMvcDotNetCoreBot.Models.VK;
using VkNet.Abstractions;
using VkNet.Model;
using VkNet.Model.Attachments;
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

        private readonly SnUserDb userDb = new SnUserDb("Vk");

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
            LoggingDB loggingDb = new LoggingDB();

            try
            {

                switch (updates.Type)
                {
                    case "confirmation":
                        return Ok(_configuration["ConfigVk:Confirmation"]);

                    case "message_new":
                        {
                            ResponseBuilder response = new ResponseBuilder("Vk");
                            ScheduleDB scheduleDb = new ScheduleDB();
                            HomeWorkLogic homeWork = new HomeWorkLogic();
                            HomeWorkDB homeWorkDb = new HomeWorkDB();
                            VkKeyboard keyboard = new VkKeyboard();
                            ModeSelection mode = new ModeSelection();

                            // Десериализация
                            var message = Message.FromJson(new VkResponse(updates.Object));
                            if (message == null)
                                return Ok("ok");

                            var chatId = message.FromId ?? -1;

                            if (chatId == -1)
                                return Ok("ok");
                            loggingDb.AddRecordInLog(chatId,
                                message.Text, DateTime.Now);
                            userDb.EditUser(chatId, "activity", "");
                            //Режим добавления ДЗ
                            if (mode.IsHWEnable(chatId) && message.Text != "Отменить")
                            {
                                homeWorkDb.AddHomeWork(userDb.CheckUserElements(chatId, "university"),
                                    userDb.CheckUserElements(chatId, "facility"),
                                    userDb.CheckUserElements(chatId, "course"),
                                    userDb.CheckUserElements(chatId, "group"), mode.GetDate(chatId), message.Text);
                               mode.HWSwitch(chatId, false);
                                _vkApi.Messages.Send(new MessagesSendParams
                                {
                                    RandomId = new DateTime().Millisecond,
                                    PeerId = message.PeerId.Value,
                                    Message = "Задание было успешно добавлено",
                                    Keyboard = response.VkMainKeyboard
                                });
                                return Ok("ok");
                            }

                            if (mode.IsTeacherScheduleEnable(chatId) && message.Text != "Отменить")
                            {

                                int index;
                                if (Int32.TryParse(message.Text, out index) && index > 0)
                                {
                                    List<Teacher> teachers = mode.GetTeacherList(chatId);
                                    if (index - 1 < teachers.Count)
                                    {
                                        mode.TeacherScheduleSwitch(chatId, false, teachers[index - 1].Name);
                                        _vkApi.Messages.Send(new MessagesSendParams
                                        {
                                            RandomId = new DateTime().Millisecond,
                                            PeerId = message.PeerId.Value,
                                            Message = "Выбери неделю и день",
                                            Keyboard = response.GetVkScheduleKeyboard(chatId, true)
                                        });

                                        return Ok("ok");
                                    }
                                    else
                                    {
                                            _vkApi.Messages.Send(new MessagesSendParams
                                            {
                                                RandomId = new DateTime().Millisecond,
                                                PeerId = message.PeerId.Value,
                                                Message = "Номер набран неправильно",
                                                Keyboard = response.PayloadCancelKeyboard
                                            });
                                            return Ok("ok");
                                    }
                                    
                                }
                                if (scheduleDb.IsTeacherExist(message.Text))
                                {
                                    mode.TeacherScheduleSwitch(chatId,false,message.Text);
                                    _vkApi.Messages.Send(new MessagesSendParams
                                    {
                                        RandomId = new DateTime().Millisecond,
                                        PeerId = message.PeerId.Value,
                                        Message = "Выбери неделю и день",
                                        Keyboard = response.GetVkScheduleKeyboard(chatId,true)
                                    });
                                  
                                    return Ok("ok");
                                }
                                else if (scheduleDb.TeachersSearch(message.Text).Count != 0)
                                {

                                    List<Teacher> teachers = scheduleDb.TeachersSearch(message.Text);
                                    if (teachers.Count > 15)
                                    {
                                        _vkApi.Messages.Send(new MessagesSendParams
                                        {
                                            RandomId = new DateTime().Millisecond,
                                            PeerId = message.PeerId.Value,
                                            Message = "Найдено слишком много преподавателей! Попробуй сделать запрос более точным",
                                            Keyboard = response.PayloadCancelKeyboard
                                        });
                                        return Ok("ok");
                                    }

                                    mode.AddTeachersList(chatId, teachers);
                                    string answer = "Выбери нужного преподавателя и отправь его номер: \n";
                                    for (int i = 0; i < teachers.Count; i++)
                                    {
                                        answer += (i + 1).ToString() + ". " + teachers[i].Name + "\n";
                                    }


                                    _vkApi.Messages.Send(new MessagesSendParams
                                    {
                                        RandomId = new DateTime().Millisecond,
                                        PeerId = message.PeerId.Value,
                                        Message = answer,
                                        Keyboard = response.PayloadCancelKeyboard
                                    });
                                    return Ok("ok");
                                }
                                else
                                {
                                    _vkApi.Messages.Send(new MessagesSendParams
                                    {
                                        RandomId = new DateTime().Millisecond,
                                        PeerId = message.PeerId.Value,
                                        Message =
                                            "Преподаватель не найден\nВведи ФИО преподавателя в формате Фамилия И. О.",
                                        Keyboard = response.PayloadCancelKeyboard
                                    });
                                    return Ok("ok");
                                }
                            }



                            if (!userDb.CheckUser(chatId) || message.Text == "Начать")
                            {
                                string[][] universities = response.UniversitiesArray(chatId);
                                List<MediaAttachment> universityAttachments = new List<MediaAttachment>(); 
                                universityAttachments.Add(new Photo
                                {
                                    OwnerId = -182016108,
                                    Id = 457239025
                                });
                                _vkApi.Messages.Send(new MessagesSendParams
                                {
                                    RandomId = new DateTime().Millisecond,
                                    PeerId = message.PeerId.Value,
                                    Attachments = universityAttachments,
                                    Message = "Для выбора используй кнопки снизу.",
                                    Keyboard = keyboard.GetKeyboard(universities,"universities")
                                });
                                return Ok("ok");
                            }

                            //deserialize
                            if (message.Payload != null)
                            {
                                ButtonPayload payload = JsonConvert.DeserializeObject<ButtonPayload>(message.Payload);


                                if (!string.IsNullOrEmpty(payload.Button))
                                {
                                    if (payload.Button.Contains(';'))
                                    {
                                        loggingDb.AddRecordInLog(chatId,
                                            payload.Button, DateTime.Now);
                                        int page = Convert.ToInt32(payload.Button.Split(';')[0]);
                                        string keyboardMode = payload.Button.Split(';')[2];
                                        if (keyboardMode == "universities")
                                        {
                                            string[][] universities = response.UniversitiesArray(chatId, page);

                                            _vkApi.Messages.Send(new MessagesSendParams
                                            {
                                                RandomId = new DateTime().Millisecond,
                                                PeerId = message.PeerId.Value,
                                                Message = "Переход на другую страницу",
                                                Keyboard = keyboard.GetKeyboard(universities,"universities", "")
                                            });
                                            return Ok("ok");
                                        }
                                        else if (keyboardMode == "facilities")
                                        {
                                            string university = payload.Button.Split(';')[1];


                                            string[][] groups = response.FacilitiesArray(chatId, university, page);

                                            _vkApi.Messages.Send(new MessagesSendParams
                                            {
                                                RandomId = new DateTime().Millisecond,
                                                PeerId = message.PeerId.Value,
                                                Message = "Переход на другую страницу",
                                                Keyboard = keyboard.GetKeyboard(groups, "facilities", university)
                                            });
                                            return Ok("ok");
                                        }
                                        else if (keyboardMode == "courses")
                                        {
                                            string facility = payload.Button.Split(';')[1];


                                            string[][] groups = response.CoursesArray(chatId, facility, page);

                                            _vkApi.Messages.Send(new MessagesSendParams
                                            {
                                                RandomId = new DateTime().Millisecond,
                                                PeerId = message.PeerId.Value,
                                                Message = "Переход на другую страницу",
                                                Keyboard = keyboard.GetKeyboard(groups, "courses", facility)
                                            });
                                            return Ok("ok");
                                        }
                                        else if (keyboardMode == "groups")
                                        {
                                            string course = payload.Button.Split(';')[1];


                                            string[][] groups = response.GroupsArray(chatId, course, page);

                                            _vkApi.Messages.Send(new MessagesSendParams
                                            {
                                                RandomId = new DateTime().Millisecond,
                                                PeerId = message.PeerId.Value,
                                                Message = "Переход на другую страницу",
                                                Keyboard = keyboard.GetKeyboard(groups, "groups", course)
                                            });
                                            return Ok("ok");
                                        }
                                        

                                        return Ok("ok");
                                    }
                                    else
                                    {
                                        Schedule schedule = new Schedule();
                                        int a = Convert.ToInt32(Char.GetNumericValue(payload.Button[0]));
                                        int b = Convert.ToInt32(Char.GetNumericValue(payload.Button[1]));
                                        int c = Convert.ToInt32(Char.GetNumericValue(payload.Button[2]));
                                        if (a == 0)
                                        {
                                            mode.HWSwitch(chatId,false);
                                            mode.TeacherScheduleSwitch(chatId,false);
                                            _vkApi.Messages.Send(new MessagesSendParams
                                            {
                                                RandomId = new DateTime().Millisecond,
                                                PeerId = message.PeerId.Value,
                                                Message = "Главное меню",
                                                Keyboard = response.VkMainKeyboard
                                            });

                                            return Ok("ok");
                                        }
                                        else if (a == 1 || a == 2)
                                        {
                                            string result = schedule.ScheduleOnTheDay(chatId, a, b, "Vk");

                                            _vkApi.Messages.Send(new MessagesSendParams
                                            {
                                                RandomId = new DateTime().Millisecond,
                                                PeerId = message.PeerId.Value,
                                                Message = result,
                                                Keyboard = response.GetVkScheduleKeyboard(chatId, false)
                                            });

                                            return Ok("ok");
                                        }
                                        else if (a == 3)
                                        {
                                            if (c == 0)
                                                _vkApi.Messages.Send(new MessagesSendParams
                                                {
                                                    RandomId = new DateTime().Millisecond,
                                                    PeerId = message.PeerId.Value,
                                                    Message = AddHomework(chatId,b),
                                                    Keyboard = response.PayloadCancelKeyboard
                                                });
                                            else if (c == 1)
                                                _vkApi.Messages.Send(new MessagesSendParams
                                                {
                                                    RandomId = new DateTime().Millisecond,
                                                    PeerId = message.PeerId.Value,
                                                    Message = AddHomework(chatId,-b),
                                                    Keyboard = response.PayloadCancelKeyboard
                                                });
                                            return Ok("ok");
                                        }
                                        else if (a == 4)
                                        {
                                            string result = String.Empty;
                                            if (c == 0)
                                                result = homeWork.SendHomework(chatId, b, "Vk");
                                            else if (c == 1)
                                                result = homeWork.SendHomework(chatId, -b, "Vk");
                                            _vkApi.Messages.Send(new MessagesSendParams
                                            {
                                                RandomId = new DateTime().Millisecond,
                                                PeerId = message.PeerId.Value,
                                                Message = result,
                                                Keyboard = response.PayloadWatchingHomeworkKeyboard
                                            });

                                            return Ok("ok");
                                        }
                                        else if (a == 5 || a == 6)
                                        {
                                            string result = schedule.TeacherScheduleOnTheDay(chatId,mode.GetTeacherName(chatId), a-4, b, "Vk");

                                            _vkApi.Messages.Send(new MessagesSendParams
                                            {
                                                RandomId = new DateTime().Millisecond,
                                                PeerId = message.PeerId.Value,
                                                Message = result,
                                                Keyboard = response.GetVkScheduleKeyboard(chatId,true)
                                            });

                                            return Ok("ok");
                                        }

                                    }

                                }

                            }

                            if (message.Text.Contains("Помощь"))
                            {
                                new ErrorLoggingDB().AddErrorInLog(chatId, "Help", message.Text, "Unknown", DateTime.Now);

                                _vkApi.Messages.Send(new MessagesSendParams
                                {
                                    RandomId = new DateTime().Millisecond,
                                    PeerId = message.PeerId.Value,
                                    Message = "Обращение было успешно зарегистировано. Спасибо!"
                                });
                                return Ok("ok");
                            }

                            if (message.Text == "Сбросить")
                            {
                                mode.HWSwitch(chatId,false);
                                mode.TeacherScheduleSwitch(chatId,false);
                                string[][] universities = response.UniversitiesArray(chatId);
                                List<MediaAttachment> universityAttachments = new List<MediaAttachment>();
                                universityAttachments.Add(new Photo
                                {
                                    OwnerId = -182016108,
                                    Id = 457239025
                                });
                                _vkApi.Messages.Send(new MessagesSendParams
                                {
                                    RandomId = new DateTime().Millisecond,
                                    PeerId = message.PeerId.Value,
                                    Attachments = universityAttachments,
                                    Message = "Для выбора используй кнопки снизу.",
                                    Keyboard = keyboard.GetKeyboard(universities,"universities")
                                });
                                return Ok("ok");
                            }


                            //Основной режим 
                            if (String.IsNullOrEmpty(userDb.CheckUserElements(chatId, "university")) &&
                            scheduleDb.IsUniversityExist(message.Text))
                            {
                                string[][] facilities = response.FacilitiesArray(chatId, message.Text);
                                List<MediaAttachment> facilityAttachments = new List<MediaAttachment>();
                                facilityAttachments.Add(new Photo
                                {
                                    OwnerId = -182016108,
                                    Id = 457239024
                                });
                                _vkApi.Messages.Send(new MessagesSendParams
                                {
                                    RandomId = new DateTime().Millisecond,
                                    PeerId = message.PeerId.Value,
                                    Attachments = facilityAttachments,
                                 //   Message = "Теперь выбери факультет",
                                    Keyboard = keyboard.GetKeyboard(facilities,"facilities", message.Text)
                                });

                                return Ok("ok");
                            }
                            else if (String.IsNullOrEmpty(userDb.CheckUserElements(chatId, "university")) &&
                                     !scheduleDb.IsUniversityExist(message.Text))
                            {
                                _vkApi.Messages.Send(new MessagesSendParams
                                {
                                    RandomId = new DateTime().Millisecond,
                                    PeerId = message.PeerId.Value,
                                    Message = "Неправильный ввод! Попробуй еще раз"
                                });
                                return Ok("ok");
                            }

                            if (String.IsNullOrEmpty(userDb.CheckUserElements(chatId, "facility")) &&
                                scheduleDb.IsFacilityExist(userDb.CheckUserElements(chatId, "university"), message.Text))
                            {
                                string[][] courses = response.CoursesArray(chatId, message.Text);
                                List<MediaAttachment> courseAttachments = new List<MediaAttachment>();
                                courseAttachments.Add(new Photo
                                {
                                    OwnerId = -182016108,
                                    Id = 457239023
                                });
                                _vkApi.Messages.Send(new MessagesSendParams
                                {
                                    RandomId = new DateTime().Millisecond,
                                    PeerId = message.PeerId.Value,
                                    Attachments = courseAttachments,
                                   // Message = "Теперь выбери курс",
                                    Keyboard = keyboard.GetKeyboard(courses,"courses", message.Text)
                                });

                                return Ok("ok");
                            }
                            else if (String.IsNullOrEmpty(userDb.CheckUserElements(chatId, "facility")) &&
                                         !scheduleDb.IsFacilityExist(userDb.CheckUserElements(chatId, "university"),
                                             message.Text))
                                 {
                                    _vkApi.Messages.Send(new MessagesSendParams
                                    {
                                        RandomId = new DateTime().Millisecond,
                                        PeerId = message.PeerId.Value,
                                        Message = "Неправильный ввод! Попробуй еще раз"
                                    });
                                    return Ok("ok");
                                 }
                            if (String.IsNullOrEmpty(userDb.CheckUserElements(chatId, "course")) && scheduleDb.IsCourseExist(
                                    userDb.CheckUserElements(chatId, "university"),
                                    userDb.CheckUserElements(chatId, "facility"),
                                    message.Text))
                            {
                                string[][] groups = response.GroupsArray(chatId, message.Text);
                                List<MediaAttachment> groupAttachments = new List<MediaAttachment>();
                                groupAttachments.Add(new Photo
                                {
                                    OwnerId = -182016108,
                                    Id = 457239027
                                });
                                _vkApi.Messages.Send(new MessagesSendParams
                                {
                                    RandomId = new DateTime().Millisecond,
                                    PeerId = message.PeerId.Value,
                                    Attachments = groupAttachments,
                                   // Message = "Теперь выбери группу",
                                    Keyboard = keyboard.GetKeyboard(groups,"groups", message.Text)
                                });

                                return Ok("ok");
                            }
                            else if (String.IsNullOrEmpty(userDb.CheckUserElements(chatId, "course")) && !scheduleDb.IsCourseExist(
                                         userDb.CheckUserElements(chatId, "university"),
                                         userDb.CheckUserElements(chatId, "facility"),
                                         message.Text))
                                 {
                                    _vkApi.Messages.Send(new MessagesSendParams
                                    {
                                        RandomId = new DateTime().Millisecond,
                                        PeerId = message.PeerId.Value,
                                        Message = "Неправильный ввод! Попробуй еще раз"
                                    });
                                    return Ok("ok");
                                 }

                            if (String.IsNullOrEmpty(userDb.CheckUserElements(chatId, "group")) && scheduleDb.IsGroupExist(
                                    userDb.CheckUserElements(chatId, "university"),
                                    userDb.CheckUserElements(chatId, "facility"),
                                    userDb.CheckUserElements(chatId, "course"), message.Text))
                            {
                                response.LetsWork(chatId, message.Text);
                                List<MediaAttachment> doneAttachments = new List<MediaAttachment>();
                                doneAttachments.Add(new Photo
                                {
                                    OwnerId = -182016108,
                                    Id = 457239022
                                });
                                _vkApi.Messages.Send(new MessagesSendParams
                                {
                                    RandomId = new DateTime().Millisecond,
                                    PeerId = message.PeerId.Value,
                                    Attachments = doneAttachments,
                                    //Message = "Отлично, можем работать!",
                                    Keyboard = response.VkMainKeyboard
                                });

                                return Ok("ok");
                            }
                                else if (String.IsNullOrEmpty(userDb.CheckUserElements(chatId, "group")) && !scheduleDb.IsGroupExist(
                                             userDb.CheckUserElements(chatId, "university"),
                                             userDb.CheckUserElements(chatId, "facility"),
                                             userDb.CheckUserElements(chatId, "course"), message.Text))
                            {
                                _vkApi.Messages.Send(new MessagesSendParams
                                {
                                    RandomId = new DateTime().Millisecond,
                                    PeerId = message.PeerId.Value,
                                    Message = "Неправильный ввод! Попробуй еще раз"
                                });
                                return Ok("ok");
                            }

                            if (message.Text == "Сегодня" && !String.IsNullOrEmpty(userDb.CheckUserElements(chatId, "group")))
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
                                {
                                    List<MediaAttachment> noLessonsAttachments = new List<MediaAttachment>();
                                    noLessonsAttachments.Add(new Photo
                                    {
                                        OwnerId = -182016108,
                                        Id = 457239026
                                    });
                                    _vkApi.Messages.Send(new MessagesSendParams
                                    {
                                        RandomId = new DateTime().Millisecond,
                                        PeerId = message.PeerId.Value,
                                        Attachments = noLessonsAttachments,
                                        //Message = "Пар нет",
                                        Keyboard = response.VkMainKeyboard
                                    });
                                }


                                return Ok("ok");

                            }

                            if (message.Text == "Завтра" && !String.IsNullOrEmpty(userDb.CheckUserElements(chatId, "group")))
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
                                    List<MediaAttachment> noLessonsAttachments = new List<MediaAttachment>();
                                    noLessonsAttachments.Add(new Photo
                                    {
                                        OwnerId = -182016108,
                                        Id = 457239026
                                    });
                                    _vkApi.Messages.Send(new MessagesSendParams
                                    {
                                        RandomId = new DateTime().Millisecond,
                                        PeerId = message.PeerId.Value,
                                        Attachments = noLessonsAttachments,
                                        //Message = "Пар нет",
                                        Keyboard = response.VkMainKeyboard
                                    });
                                }


                             
                                return Ok("ok");

                            }

                            if (message.Text == "Расписание" && !String.IsNullOrEmpty(userDb.CheckUserElements(chatId, "group")))
                            {
                                _vkApi.Messages.Send(new MessagesSendParams
                                {
                                    RandomId = new DateTime().Millisecond,
                                    PeerId = message.PeerId.Value,
                                    Message = "Выбери неделю и день\nОтсчет недель слева направо",
                                    Keyboard = response.GetVkScheduleKeyboard(chatId,false)
                                });
                                return Ok("ok");
                            }

                            if (message.Text == "Что задали?" && !String.IsNullOrEmpty(userDb.CheckUserElements(chatId, "group")))
                            {
                                _vkApi.Messages.Send(new MessagesSendParams
                                {
                                    RandomId = new DateTime().Millisecond,
                                    PeerId = message.PeerId.Value,
                                    Message = "Выбери дату\n \nСегодня " + response.DateConverter(DateTime.Now),
                                    Keyboard = response.PayloadWatchingHomeworkKeyboard
                                });
                                return Ok("ok");
                            }

                            if (message.Text == "Добавить ДЗ" && !String.IsNullOrEmpty(userDb.CheckUserElements(chatId, "group")))
                            {

                                _vkApi.Messages.Send(new MessagesSendParams
                                {
                                    RandomId = new DateTime().Millisecond,
                                    PeerId = message.PeerId.Value,
                                    Message = "Выбери дату\n \nСегодня " + response.DateConverter(DateTime.Now),
                                    Keyboard = response.PayloadAddingHomeworkKeyboard
                                });
                                return Ok("ok");
                            }
                            if (message.Text == "Расписание преподавателя" && !String.IsNullOrEmpty(userDb.CheckUserElements(chatId, "group")))
                            {
                               
                                mode.TeacherScheduleSwitch(chatId,true);
                                _vkApi.Messages.Send(new MessagesSendParams
                                {
                                    RandomId = new DateTime().Millisecond,
                                    PeerId = message.PeerId.Value,
                                    Message = "Введи ФИО преподавателя в формате Фамилия И. О. или Фамилия",
                                    Keyboard = response.PayloadCancelKeyboard
                                });
                                return Ok("ok");
                            }

                            if (message.Text == "О пользователе" && !String.IsNullOrEmpty(userDb.CheckUserElements(chatId, "group")))
                            {
                                _vkApi.Messages.Send(new MessagesSendParams
                                {
                                    RandomId = new DateTime().Millisecond,
                                    PeerId = message.PeerId.Value,
                                    Message = response.UserInfo(chatId),
                                    Keyboard = response.VkMainKeyboard
                                });
                                return Ok("ok");
                            }
                            if (message.Text == "Сообщить о неверном расписании" && !String.IsNullOrEmpty(userDb.CheckUserElements(chatId, "group")))
                            {
                                ErrorLoggingDB errorLoggingDb = new ErrorLoggingDB();
                                errorLoggingDb.AddErrorInLog(chatId, "ScheduleError", message.Text, userDb.CheckUserElements(chatId, "university"), DateTime.Now);

                                _vkApi.Messages.Send(new MessagesSendParams
                                {
                                    RandomId = new DateTime().Millisecond,
                                    PeerId = message.PeerId.Value,
                                    Message = "Спасибо за помощь!\nМы скоро исправим это",
                                    Keyboard = response.VkMainKeyboard
                                });
                                return Ok("ok");
                            }



                            if (message.Text == "В главное меню" && !String.IsNullOrEmpty(userDb.CheckUserElements(chatId, "group")))
                            {
                                _vkApi.Messages.Send(new MessagesSendParams
                                {
                                    RandomId = new DateTime().Millisecond,
                                    PeerId = message.PeerId.Value,
                                    Message = "Главное меню",
                                    Keyboard = response.VkMainKeyboard
                                });
                                return Ok("ok");
                            }

                            if (message.Text.Contains("Спасибо") &&
                                !String.IsNullOrEmpty(userDb.CheckUserElements(chatId, "group")))
                            {
                                

                                _vkApi.Messages.Send(new MessagesSendParams
                                {
                                    RandomId = new DateTime().Millisecond,
                                    PeerId = message.PeerId.Value,
                                    Message = "Всегда пожалуйста 😉"
                                });
                                return Ok("ok");

                            }



                            _vkApi.Messages.Send(new MessagesSendParams
                            {
                                RandomId = new DateTime().Millisecond,
                                PeerId = message.PeerId.Value,
                                Message = "Извините, такой команды я не знаю"
                            });
                            return Ok("ok");

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
                    Message message = Message.FromJson(new VkResponse(updates.Object));

                    long chatId = message.FromId ?? -1;

                    _vkApi.Messages.Send(new MessagesSendParams
                    {
                        RandomId = new DateTime().Millisecond,
                        PeerId = message.PeerId.Value,
                        Message = "Хм, что-то пошло не так\nЕсли у вас возникают проблемы, просто напишите боту о своей проблеме, снабжая вопрос надписью 'Помощь', и мы постараемся помочь вам."
                    });

                    errorLoggingDb.AddErrorInLog(chatId, "Message", message.Text, e.Source + ": " + e.Message, DateTime.Now);
                    return Ok("ok");
                }

                return Ok("ok");
            }
        }
        private string AddHomework(long chatId,int daysfromtoday)
        {
            DateTime now = DateTime.Now.Date;
            ResponseBuilder response = new ResponseBuilder("Telegram");
            ModeSelection mode = new ModeSelection();
            if (daysfromtoday < 0)
            {
                mode.HWSwitch(chatId, true, response.DateConverter(now.Subtract(new TimeSpan(-daysfromtoday, 0, 0, 0))));
            }
            else if (daysfromtoday == 0)
            {
                mode.HWSwitch(chatId, true, response.DateConverter(now));
            }
            else if (daysfromtoday > 0)
            {
                mode.HWSwitch(chatId, true, response.DateConverter(now.AddDays(daysfromtoday)));
            }

            return "Введите текст домашнего задания и отправьте его как обычное сообщение";
        }

        public void SendMessages(List<long> users, string message)
        {
            foreach (long user in users)
            {
                _vkApi.Messages.Send(new MessagesSendParams
                {
                    RandomId = new DateTime().Millisecond,
                    PeerId = user,
                    Message = message
                });
            }
        }
    }
}
