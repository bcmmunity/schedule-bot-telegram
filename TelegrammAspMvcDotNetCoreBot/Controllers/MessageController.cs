using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using TelegrammAspMvcDotNetCoreBot.DB;
using TelegrammAspMvcDotNetCoreBot.Logic;
using TelegrammAspMvcDotNetCoreBot.Models;
using TelegrammAspMvcDotNetCoreBot.Models.Telegramm;

namespace TelegrammAspMvcDotNetCoreBot.Controllers
{
    [Route("api/message/update")]
    public class MessageController : Controller
    {
        private readonly SnUserDb userDb = new SnUserDb("Telegram");
        
        // GET api/values
        [HttpGet]
        public string Get()
        {
            return "Method GET unavailable";
        }

        // POST api/values
        [HttpPost]
        public async Task<OkResult> Post([FromBody]Update update)
        {
            LoggingDB loggingDb = new LoggingDB();

            try
            {
                if (update == null)
                {
                    return Ok();
                }

                var botClient = await Bot.GetBotClientAsync();

                //await botClient.SendTextMessageAsync(update.Message.Chat.Id, "Бот на профилактике.", parseMode: Telegram.Bot.Types.Enums.ParseMode.Default);
                //return Ok();

                if (update.Type == UpdateType.Message)
                {
                    var message = update.Message;
                    var chatId = message.Chat.Id;
                    var commands = Bot.Commands;

                    //await botClient.SendTextMessageAsync(chatId, "Бот на профилактике.", parseMode: Telegram.Bot.Types.Enums.ParseMode.Default);
                    //return Ok();



                    foreach (var command in commands)
                    {
                        if (command.Contains(message) || (!userDb.CheckUser(chatId)))
                        {
                            await command.Execute(message, botClient);
                            return Ok();
                        }
                    }

                    loggingDb.AddRecordInLog(chatId, message.Text, DateTime.Now);
                    userDb.EditUser(chatId,"activity","");

                    if (!userDb.CheckUser(chatId))
                    {
                        message.Text = @"/start";

                        foreach (var command in commands)
                        {
                            if (command.Contains(message))
                            {
                                await command.Execute(message, botClient);
                                return Ok();
                            }
                        }

                        return Ok();
                    }
                    //Жирные стикеры
                    //InputOnlineFile facSticker = new InputOnlineFile("CAADAgADBwADi6p7D7JUJy3u1Q22Ag");
                    //InputOnlineFile courseSticker = new InputOnlineFile("CAADAgADBgADi6p7DxEJvhyK0iHFAg");
                    //InputOnlineFile groupSticker = new InputOnlineFile("CAADAgADBAADi6p7DzzxU-ilYtP6Ag");
                    //InputOnlineFile workSticker = new InputOnlineFile("CAADAgADBQADi6p7D849HV-BVKxIAg");
                    //InputOnlineFile relaxSticker = new InputOnlineFile("CAADAgADAgADi6p7D_SOcGo7bWCIAg");

                    InputOnlineFile facSticker = new InputOnlineFile("CAADAgADEAADi6p7D9f-4MDdLon5Ag");
                    InputOnlineFile courseSticker = new InputOnlineFile("CAADAgADDwADi6p7D20iVusN4DUzAg");
                    InputOnlineFile groupSticker = new InputOnlineFile("CAADAgADEwADi6p7D5LIy-MGaXhaAg");
                    InputOnlineFile workSticker = new InputOnlineFile("CAADAgADDgADi6p7DxKP7piNPfEcAg");
                    InputOnlineFile relaxSticker = new InputOnlineFile("CAADAgADEgADi6p7D-1w9zvhrRKPAg");

                    ResponseBuilder response = new ResponseBuilder("Telegram");
                    HomeWorkDB homeWorkDb = new HomeWorkDB();
                    ModeSelection mode = new ModeSelection();
                    ScheduleDB scheduleDb = new ScheduleDB();

                    TelegramKeyboard keybord = new TelegramKeyboard();

                    //Режим добавления ДЗ
                    if (mode.IsHWEnable(chatId))
                    {
                        homeWorkDb.AddHomeWork(userDb.CheckUserElements(chatId, "university"),
                            userDb.CheckUserElements(chatId, "facility"), userDb.CheckUserElements(chatId, "course"),
                            userDb.CheckUserElements(chatId, "group"), mode.GetDate(chatId), message.Text);
                        mode.HWSwitch(chatId, false);
                        await botClient.SendTextMessageAsync(chatId, "Задание было успешно добавлено", ParseMode.Default);
                        return Ok();
                    }

                    //Режим расписания
                    if (mode.IsTeacherScheduleEnable(chatId))
                    {
                        int index;
                        if (Int32.TryParse(message.Text, out index) && index > 0)
                        {
                            List<Teacher> teachers = mode.GetTeacherList(chatId);
                            if (index - 1 < teachers?.Count)
                            {
                                {
                                    mode.TeacherScheduleSwitch(chatId, false, teachers[index - 1].Name);
                                    await botClient.SendTextMessageAsync(chatId, "Выбери неделю и день", ParseMode.Default,
                                        replyMarkup: response.InlineTeacherScheduleKeyboard);
                                    return Ok();
                                }
                            }
                            else
                            {
                                await botClient.SendTextMessageAsync(chatId, "Номер набран неправильно", ParseMode.Default,
                                    replyMarkup: response.InlineCancelKeyboard);
                                return Ok();
                            }
                        }
                        if (scheduleDb.IsTeacherExist(message.Text))
                        {
                            mode.TeacherScheduleSwitch(chatId,false,message.Text);
                            await botClient.SendTextMessageAsync(chatId, "Выбери неделю и день", ParseMode.Default,
                                replyMarkup: response.InlineTeacherScheduleKeyboard);
                            return Ok();
                        }
                        else if (scheduleDb.TeachersSearch(message.Text).Count != 0)
                        {
                            List<Teacher> teachers = scheduleDb.TeachersSearch(message.Text);
                            if (teachers.Count > 15)
                            {
                                await botClient.SendTextMessageAsync(chatId, "Найдено слишком много преподавателей! Попробуй сделать запрос более точным", ParseMode.Default,
                                    replyMarkup: response.InlineCancelKeyboard);
                                return Ok();
                            }
                            mode.AddTeachersList(chatId,teachers);
                            string answer = "Выбери нужного преподавателя и отправь его номер: \n";
                            for(int i = 0; i<teachers.Count;i++)
                            {
                                answer += (i+1).ToString()+". "+teachers[i].Name + "\n";
                            }


                            await botClient.SendTextMessageAsync(chatId, answer, ParseMode.Default,
                                replyMarkup: response.InlineCancelKeyboard);
                            return Ok();
                        }
                        else
                        {
                            await botClient.SendTextMessageAsync(chatId,
                                "Преподаватель не найден\nВведи ФИО преподавателя в формате Фамилия И. О. или Фамилия либо нажми Отменить",
                                parseMode: ParseMode.Default, replyMarkup: response.InlineCancelKeyboard);
                            return Ok();
                        }
                    }

                    if (message.Text.Contains("Помощь"))
                    {
                        new ErrorLoggingDB().AddErrorInLog(chatId, "Help", message.Text, "Unknown", DateTime.Now);
                        await botClient.SendTextMessageAsync(chatId,
                            "Обращение было успешно зарегистировано. Спасибо!", parseMode: ParseMode.Default);
                        return Ok();
                    }

                    if (message.Text == "Сбросить")
                    {
                        mode.HWSwitch(chatId, false);
                        mode.TeacherScheduleSwitch(chatId, false);
                        message.Text = @"/start";

                        foreach (var command in commands)
                        {
                            if (command.Contains(message))
                            {
                                await command.Execute(message, botClient);
                                return Ok();
                            }
                        }
                    }

                    //Основной режим 
                        if (String.IsNullOrEmpty(userDb.CheckUserElements(chatId, "university")) && scheduleDb.IsUniversityExist(message.Text))
                        {
                            string[][] facilities = response.FacilitiesArray(chatId, message.Text);

                            //await botClient.SendTextMessageAsync(chatId, "Теперь выбери факультет", parseMode: Telegram.Bot.Types.Enums.ParseMode.Default, replyMarkup: (Telegram.Bot.Types.ReplyMarkups.IReplyMarkup)KeybordController.GetKeyboard(unn, count));
                            await botClient.SendStickerAsync(chatId, facSticker, replyMarkup: keybord.GetKeyboard(facilities));

                        return Ok();
                        }
                        else if (String.IsNullOrEmpty(userDb.CheckUserElements(chatId, "university")) &&
                                 !scheduleDb.IsUniversityExist(message.Text))
                        {
                            await botClient.SendTextMessageAsync(chatId, "Неправильный ввод! Попробуй еще раз");
                            return Ok();
                    }
                        if (String.IsNullOrEmpty(userDb.CheckUserElements(chatId, "facility")) &&
                            scheduleDb.IsFacilityExist(userDb.CheckUserElements(chatId, "university"), message.Text))
                        {
                            string[][] courses = response.CoursesArray(chatId, message.Text);

                            //await botClient.SendTextMessageAsync(chatId, "Теперь выбери курс", parseMode: Telegram.Bot.Types.Enums.ParseMode.Default, replyMarkup: (Telegram.Bot.Types.ReplyMarkups.IReplyMarkup)KeybordController.GetKeyboard(unn, count));
                            await botClient.SendStickerAsync(chatId, courseSticker, replyMarkup: keybord.GetKeyboard(courses));
                            return Ok();
                        }
                        else if (String.IsNullOrEmpty(userDb.CheckUserElements(chatId, "facility")) &&
                                 !scheduleDb.IsFacilityExist(userDb.CheckUserElements(chatId, "university"),
                                     message.Text))
                        {
                            await botClient.SendTextMessageAsync(chatId, "Неправильный ввод! Попробуй еще раз");
                            return Ok();
                    }

                        if (String.IsNullOrEmpty(userDb.CheckUserElements(chatId, "course")) && scheduleDb.IsCourseExist(
                                userDb.CheckUserElements(chatId, "university"), userDb.CheckUserElements(chatId, "facility"),
                                message.Text))
                        {
                            string[][] groups = response.GroupsArray(chatId, message.Text);

                            //await botClient.SendTextMessageAsync(chatId, "Теперь выбери группу", parseMode: Telegram.Bot.Types.Enums.ParseMode.Default, replyMarkup: (Telegram.Bot.Types.ReplyMarkups.IReplyMarkup)KeybordController.GetKeyboard(unn, count));
                            await botClient.SendStickerAsync(chatId, groupSticker, replyMarkup: keybord.GetKeyboard(groups));
                            return Ok();
                        }
                        else if (String.IsNullOrEmpty(userDb.CheckUserElements(chatId, "course")) && !scheduleDb.IsCourseExist(
                                     userDb.CheckUserElements(chatId, "university"),
                                     userDb.CheckUserElements(chatId, "facility"),
                                     message.Text))
                        {
                            await botClient.SendTextMessageAsync(chatId, "Неправильный ввод! Попробуй еще раз");
                            return Ok();
                    }

                        if (String.IsNullOrEmpty(userDb.CheckUserElements(chatId, "group")) && scheduleDb.IsGroupExist(
                                userDb.CheckUserElements(chatId, "university"), userDb.CheckUserElements(chatId, "facility"),
                                userDb.CheckUserElements(chatId, "course"), message.Text))
                        {
                            response.LetsWork(chatId, message.Text);

                            // await botClient.SendTextMessageAsync(chatId, "Отлично, можем работать!", parseMode: Telegram.Bot.Types.Enums.ParseMode.Default, replyMarkup: (Telegram.Bot.Types.ReplyMarkups.IReplyMarkup)KeybordController.GetKeyboard(unn, 2));
                            await botClient.SendStickerAsync(chatId, workSticker, replyMarkup: response.TelegramMainKeyboard);
                            return Ok();
                        }
                        else if (String.IsNullOrEmpty(userDb.CheckUserElements(chatId, "group")) && !scheduleDb.IsGroupExist(
                                     userDb.CheckUserElements(chatId, "university"), userDb.CheckUserElements(chatId, "facility"),
                                     userDb.CheckUserElements(chatId, "course"), message.Text))
                    {
                        await botClient.SendTextMessageAsync(chatId, "Неправильный ввод! Попробуй еще раз");
                        return Ok();
                    }

                        if (message.Text == "Сегодня" && !String.IsNullOrEmpty(userDb.CheckUserElements(chatId, "group")))
                        {
                            string result = response.Today(chatId);

                            if (!result.Equals("Учебы нет"))
                            {
                                await botClient.SendTextMessageAsync(chatId, result, parseMode: ParseMode.Default, replyMarkup: response.TelegramMainKeyboard);
                            }
                            else
                            {

                                //await botClient.SendTextMessageAsync(chatId, "Учебы нет, отдыхай", parseMode: Telegram.Bot.Types.Enums.ParseMode.Default);
                                await botClient.SendStickerAsync(chatId, relaxSticker, replyMarkup: response.TelegramMainKeyboard);
                            }

                            
                            return Ok();
                        }

                        if (message.Text == "Завтра" && !String.IsNullOrEmpty(userDb.CheckUserElements(chatId, "group")))
                        {

                            string result = response.Tommorrow(chatId);

                            if (!result.Equals("Учебы нет"))
                            {
                                await botClient.SendTextMessageAsync(chatId, result, ParseMode.Default, replyMarkup: response.TelegramMainKeyboard);
                            }
                            else
                            {
                                //await botClient.SendTextMessageAsync(chatId, "Учебы нет, отдыхай", parseMode: Telegram.Bot.Types.Enums.ParseMode.Default);
                                await botClient.SendStickerAsync(chatId, relaxSticker, replyMarkup: response.TelegramMainKeyboard);
                            }

                            return Ok();
                        }

                        if (message.Text == "Расписание")
                        {
                            await botClient.SendTextMessageAsync(chatId, "Выбери неделю и день", ParseMode.Default,
                                replyMarkup: response.InlineScheduleKeyboard);
                            return Ok();
                        }

                        if (message.Text == "Добавить ДЗ" && !String.IsNullOrEmpty(userDb.CheckUserElements(chatId, "group")))
                        {
                            await botClient.SendTextMessageAsync(chatId, "Выбери дату\n \nСегодня " + response.DateConverter(DateTime.Now), ParseMode.Default,
                                replyMarkup: response.InlineAddingHomeworkKeyboard);
                            return Ok();
                        }
                        if (message.Text == "Что задали?" && !String.IsNullOrEmpty(userDb.CheckUserElements(chatId, "group")))
                        {
                            await botClient.SendTextMessageAsync(chatId, "Выбери дату\n \nСегодня " + response.DateConverter(DateTime.Now), ParseMode.Default,
                                replyMarkup: response.InlineWatchingHomeworkKeyboard);
                            return Ok();
                        }
                    if (message.Text == "Расписание преподавателя")
                    {
                        mode.TeacherScheduleSwitch(chatId, true);
                        await botClient.SendTextMessageAsync(chatId, "Введи ФИО преподавателя в формате Фамилия И. О. или Фамилия либо нажми Отменить", parseMode: ParseMode.Default, replyMarkup: response.InlineCancelKeyboard);
                        return Ok();
                    }
                    if (message.Text == "О пользователе")
                        {
                            await botClient.SendTextMessageAsync(chatId, response.UserInfo(chatId), parseMode: ParseMode.Default, replyMarkup: response.TelegramMainKeyboard);
                            return Ok();
                        }
                    if (message.Text == "Сообщить о неверном расписании" && !String.IsNullOrEmpty(userDb.CheckUserElements(chatId, "group")))
                    {
                        ErrorLoggingDB errorLoggingDb = new ErrorLoggingDB();
                        errorLoggingDb.AddErrorInLog(chatId, "ScheduleError", message.Text, userDb.CheckUserElements(chatId, "university"), DateTime.Now);

                        await botClient.SendTextMessageAsync(chatId, "Спасибо за помощь!\nМы скоро исправим это", parseMode: ParseMode.Default, replyMarkup: response.TelegramMainKeyboard);
                        return Ok();
                    }

                    if (message.Text.Contains("Спасибо"))
                        {
                            await botClient.SendTextMessageAsync(chatId, "Всегда пожалуйста 😉", parseMode: ParseMode.Default, replyMarkup: response.TelegramMainKeyboard);
                            return Ok();
                        
                        }


                        await botClient.SendTextMessageAsync(chatId, "Извините, такой команды я не знаю", parseMode: ParseMode.Default);
                    return Ok();
                }

                //Callback Query
                else if (update.Type == UpdateType.CallbackQuery)
                {
                    long chatId = update.CallbackQuery.Message.Chat.Id;

                    loggingDb.AddRecordInLog(chatId, update.CallbackQuery.Data, DateTime.Now);

                    Schedule schedule = new Schedule();
                    HomeWorkLogic homeWork = new HomeWorkLogic();
                    ResponseBuilder response = new ResponseBuilder("Telegram");
                    ModeSelection mode = new ModeSelection();

                    int a = Convert.ToInt32(Char.GetNumericValue(update.CallbackQuery.Data[0]));
                    int b = Convert.ToInt32(Char.GetNumericValue(update.CallbackQuery.Data[1]));
                    int c = Convert.ToInt32(Char.GetNumericValue(update.CallbackQuery.Data[2]));

                    if (a == 0)
                    {
                        mode.HWSwitch(chatId,false);
                        mode.TeacherScheduleSwitch(chatId, false);
                        await botClient.EditMessageTextAsync(chatId,
                            update.CallbackQuery.Message.MessageId, "Ввод отменен");
                        await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                    }
                    else if (a == 1 || a == 2)
                    {
                        string result = schedule.ScheduleOnTheDay(chatId, a, b, "Telegram");
                        if (result != update.CallbackQuery.Message.Text)
                            await botClient.EditMessageTextAsync(chatId,
                            update.CallbackQuery.Message.MessageId, result, replyMarkup: response.InlineScheduleKeyboard);
                        await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                    }
                    else if (a == 3)
                    {
                        if (c == 0)
                        {
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, AddHomework(chatId,b), replyMarkup: response.InlineCancelKeyboard);
                        }
                        else if (c == 1)
                        {
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, AddHomework(chatId,-b), replyMarkup: response.InlineCancelKeyboard);
                        }

                        await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                    }
                    else if (a == 4)
                    {
                        string result = String.Empty;
                        if (c == 0)
                        {
                            result = homeWork.SendHomework(chatId, b, "Telegram");
                        }
                        else if (c == 1)
                        {
                            result = homeWork.SendHomework(chatId, -b, "Telegram");
                        }

                        if (result != update.CallbackQuery.Message.Text)
                             await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, result, replyMarkup: response.InlineWatchingHomeworkKeyboard);
                        
                        

                        await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);


                    }
                    else if (a == 5 || a == 6)
                    {
                        string result = schedule.TeacherScheduleOnTheDay(chatId,mode.GetTeacherName(chatId), a-4, b, "Telegram");
                        if (result != update.CallbackQuery.Message.Text)
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, result, replyMarkup: response.InlineTeacherScheduleKeyboard);
                        await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                    }
                }

                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                if (update != null)
                {
                    ErrorLoggingDB errorLoggingDb = new ErrorLoggingDB();

                    if (update.Type == UpdateType.Message)
                    {
                        var message = update.Message;
                        var chatId = message.Chat.Id;
                        var botClient = await Bot.GetBotClientAsync();

                        await botClient.SendTextMessageAsync(chatId, "Хм, что то пошло не так\nЕсли у вас возникают проблемы, просто напишите боту о своей проблеме, снабжая вопрос надписью 'Помощь', и мы постараемся помочь вам.", parseMode: Telegram.Bot.Types.Enums.ParseMode.Default);

                        errorLoggingDb.AddErrorInLog(chatId, "Message", message.Text, e.Message, DateTime.Now);

                    }
                    else if (update.Type == UpdateType.CallbackQuery)
                    {
                        long chatId = update.CallbackQuery.Message.Chat.Id;
                        var botClient = await Bot.GetBotClientAsync();

                        await botClient.EditMessageTextAsync(chatId,
                        update.CallbackQuery.Message.MessageId, "Хм, что-то пошло не так\nЕсли у вас возникают проблемы, просто напишите боту о своей проблеме, снабжая вопрос надписью 'Помощь', и мы постараемся помочь вам.");

                        errorLoggingDb.AddErrorInLog(chatId, "CallbackQuery", update.CallbackQuery.Data, e.Source + ": " + e.Message, DateTime.Now);

                        await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                    }


                }

                return Ok();
            }
        }

        private string AddHomework(long chatId,int daysfromtoday)
        {
            DateTime now = DateTime.Now.Date;
            ResponseBuilder response = new ResponseBuilder("Telegram");
            ModeSelection mode = new ModeSelection();
            if (daysfromtoday < 0)
            {
                mode.HWSwitch(chatId,true, response.DateConverter(now.Subtract(new TimeSpan(-daysfromtoday, 0, 0, 0))));   
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

        public async void SendMessages(List<long> users, string message, TelegramBotClient botClient )
        {
            foreach (long user in users)
            {
                await botClient.SendTextMessageAsync(user, message);

            }
        }
    }
}
