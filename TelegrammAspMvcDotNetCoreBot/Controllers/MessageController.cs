using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InputFiles;
using TelegrammAspMvcDotNetCoreBot.DB;
using TelegrammAspMvcDotNetCoreBot.Logic;
using TelegrammAspMvcDotNetCoreBot.Models.Telegramm;

namespace TelegrammAspMvcDotNetCoreBot.Controllers
{
    [Route("api/message/update")]
    public class MessageController : Controller
    {
        private static bool Dz { get; set; } = false;
        private static string Date { get; set; } = String.Empty;
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
            DateTime startTime = DateTime.Now;
            LoggingDB loggingDb = new LoggingDB();

            try
            {
                if (update == null)
                {
                    return Ok();
                }

                var botClient = await Bot.GetBotClientAsync();

                if (update.Type == UpdateType.Message)
                {
                    var message = update.Message;
                    var chatId = message.Chat.Id;
                    var commands = Bot.Commands;

                    //await botClient.SendTextMessageAsync(chatId, "Бот на профилактике.\nПлановая дата окончания: 12.04.19 03:00", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                    //return Ok();

                    foreach (var command in commands)
                    {
                        if (command.Contains(message) || (!userDb.CheckUser(chatId)))
                        {
                            await command.Execute(message, botClient);
                            return Ok();
                        }
                    }

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

                    ResponseBulder response = new ResponseBulder("Telegram");
                    ScheduleDB scheduleDb = new ScheduleDB();
                    HomeWorkDB homeWorkDb = new HomeWorkDB();

                    TelegramKeyboard keybord = new TelegramKeyboard();

                    //Режим добавления ДЗ
                    if (Dz)
                    {
                        homeWorkDb.AddHomeWork(userDb.CheckUserElements(chatId, "university"),
                            userDb.CheckUserElements(chatId, "facility"), userDb.CheckUserElements(chatId, "course"),
                            userDb.CheckUserElements(chatId, "group"), Date, message.Text);
                        Dz = false;
                        Date = String.Empty;
                        await botClient.SendTextMessageAsync(chatId, "Задание было успешно добавлено", ParseMode.Markdown);
                        return Ok();
                    }

                    if (message.Text.Contains("Помощь"))
                    {
                        new ErrorLoggingDB().AddErrorInLog(chatId, "Help", message.Text, "Unknown", DateTime.Now);
                        await botClient.SendTextMessageAsync(chatId,
                            "Обращение было успешно зарегистировано. Спасибо!", parseMode: ParseMode.Markdown);
                        return Ok();
                    }

                    //Основной режим 
                    if (userDb.CheckUserElements(chatId, "university") == "" && scheduleDb.IsUniversityExist(message.Text))
                    {
                        string[][] facilities = response.FacilitiesArray(chatId, message.Text);

                        //await botClient.SendTextMessageAsync(chatId, "Теперь выбери факультет", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: (Telegram.Bot.Types.ReplyMarkups.IReplyMarkup)KeybordController.GetKeyboard(unn, count));
                        await botClient.SendStickerAsync(chatId, facSticker, replyMarkup: keybord.GetKeyboard(facilities));

                        return Ok();
                    }

                    if (userDb.CheckUserElements(chatId, "facility") == "" &&
                        scheduleDb.IsFacilityExist(userDb.CheckUserElements(chatId, "university"), message.Text))
                    {
                        string[][] courses = response.CoursesArray(chatId, message.Text);

                        //await botClient.SendTextMessageAsync(chatId, "Теперь выбери курс", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: (Telegram.Bot.Types.ReplyMarkups.IReplyMarkup)KeybordController.GetKeyboard(unn, count));
                        await botClient.SendStickerAsync(chatId, courseSticker, replyMarkup: keybord.GetKeyboard(courses));
                        return Ok();
                    }

                    if (userDb.CheckUserElements(chatId, "course") == "" && scheduleDb.IsCourseExist(
                            userDb.CheckUserElements(chatId, "university"), userDb.CheckUserElements(chatId, "facility"),
                            message.Text))
                    {
                        string[][] groups = response.GroupsArray(chatId, message.Text);

                        //await botClient.SendTextMessageAsync(chatId, "Теперь выбери группу", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: (Telegram.Bot.Types.ReplyMarkups.IReplyMarkup)KeybordController.GetKeyboard(unn, count));
                        await botClient.SendStickerAsync(chatId, groupSticker, replyMarkup: keybord.GetKeyboard(groups));
                        return Ok();
                    }

                    if (userDb.CheckUserElements(chatId, "group") == "" && scheduleDb.IsGroupExist(
                            userDb.CheckUserElements(chatId, "university"), userDb.CheckUserElements(chatId, "facility"),
                            userDb.CheckUserElements(chatId, "course"), message.Text))
                    {
                        response.LetsWork(chatId, message.Text);

                        // await botClient.SendTextMessageAsync(chatId, "Отлично, можем работать!", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown, replyMarkup: (Telegram.Bot.Types.ReplyMarkups.IReplyMarkup)KeybordController.GetKeyboard(unn, 2));
                        await botClient.SendStickerAsync(chatId, workSticker, replyMarkup: response.TelegramMainKeyboard);
                        return Ok();
                    }

                    if (message.Text == "Сегодня" && userDb.CheckUserElements(chatId, "group") != "")
                    {
                        string result = response.Today(chatId);

                        if (!result.Equals("Учебы нет"))
                        {
                            await botClient.SendTextMessageAsync(chatId, result, parseMode: ParseMode.Markdown, replyMarkup: response.TelegramMainKeyboard);
                        }
                        else
                        {

                            //await botClient.SendTextMessageAsync(chatId, "Учебы нет, отдыхай", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                            await botClient.SendStickerAsync(chatId, relaxSticker, replyMarkup: response.TelegramMainKeyboard);
                        }

                        loggingDb.AddRecordInLog(chatId, message.Text + " <Time of evaluation> = " + (DateTime.Now - startTime).Seconds, startTime);
                        return Ok();
                    }

                    if (message.Text == "Завтра" && userDb.CheckUserElements(chatId, "group") != "")
                    {

                        string result = response.Tommorrow(chatId);

                        if (!result.Equals("Учебы нет"))
                        {
                            await botClient.SendTextMessageAsync(chatId, result, ParseMode.Markdown, replyMarkup: response.TelegramMainKeyboard);
                        }
                        else
                        {
                            //await botClient.SendTextMessageAsync(chatId, "Учебы нет, отдыхай", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);
                            await botClient.SendStickerAsync(chatId, relaxSticker, replyMarkup: response.TelegramMainKeyboard);
                        }

                        loggingDb.AddRecordInLog(chatId, message.Text + " <Time of evaluation> = " + (DateTime.Now - startTime).Seconds, startTime);
                        return Ok();
                    }

                    if (message.Text == "Расписание")
                    {
                        await botClient.SendTextMessageAsync(chatId, "Выбери неделю и день", ParseMode.Markdown,
                            replyMarkup: response.InlineScheduleKeyboard);
                        return Ok();
                    }

                    if (message.Text == "Добавить ДЗ" && userDb.CheckUserElements(chatId, "group") != "")
                    {
                        await botClient.SendTextMessageAsync(chatId, "Выбери дату\n \nСегодня " + response.DateConverter(DateTime.Now), ParseMode.Markdown,
                            replyMarkup: response.InlineAddingHomeworkKeyboard);
                        return Ok();
                    }
                    if (message.Text == "Что задали?" && userDb.CheckUserElements(chatId, "group") != "")
                    {
                        await botClient.SendTextMessageAsync(chatId, "Выбери дату\n \nСегодня " + response.DateConverter(DateTime.Now), ParseMode.Markdown,
                            replyMarkup: response.InlineWatchingHomeworkKeyboard);
                        return Ok();
                    }
                    if (message.Text == "О пользователе")
                    {
                        await botClient.SendTextMessageAsync(chatId, response.UserInfo(chatId), parseMode: ParseMode.Markdown, replyMarkup: response.TelegramMainKeyboard);
                        return Ok();
                    }

                    if (message.Text == "Сбросить")
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



                        await botClient.SendTextMessageAsync(chatId, "Извините, такой команды я не знаю", parseMode: ParseMode.Markdown);

                        loggingDb.AddRecordInLog(chatId, message.Text + " <Time of evaluation> = " + (DateTime.Now - startTime).Seconds, startTime);
                        return Ok();
                    }
                }

                //Callback Query
                else if (update.Type == UpdateType.CallbackQuery)
                {
                    long chatId = update.CallbackQuery.Message.Chat.Id;

                    Schedule schedule = new Schedule();
                    HomeWorkLogic homeWork = new HomeWorkLogic();
                    ResponseBulder response = new ResponseBulder("Telegram");

                    int a = Convert.ToInt32(Char.GetNumericValue(update.CallbackQuery.Data[0]));
                    int b = Convert.ToInt32(Char.GetNumericValue(update.CallbackQuery.Data[1]));
                    int c = Convert.ToInt32(Char.GetNumericValue(update.CallbackQuery.Data[2]));

                    if (a == 0)
                    {
                        Dz = false;
                        Date = String.Empty;
                        await botClient.EditMessageTextAsync(chatId,
                            update.CallbackQuery.Message.MessageId, "Ввод задания отменен");
                        await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                    }
                    else if (a == 1 || a == 2)
                    {
                        string result = schedule.ScheduleOnTheDay(chatId, a, b, "Telegram");
                        await botClient.EditMessageTextAsync(chatId,
                            update.CallbackQuery.Message.MessageId, result, replyMarkup: response.InlineScheduleKeyboard);
                        await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                    }
                    else if (a == 3)
                    {
                        if (c == 0)
                        {
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, AddHomework(b), replyMarkup: response.InlineHomeworkCancelKeyboard);
                        }
                        else if (c == 1)
                        {
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, AddHomework(-b), replyMarkup: response.InlineHomeworkCancelKeyboard);
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

                        if (result == update.CallbackQuery.Message.Text)
                        {
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                        }
                        else
                        {
                            await botClient.EditMessageTextAsync(chatId,
                                update.CallbackQuery.Message.MessageId, result, replyMarkup: response.InlineWatchingHomeworkKeyboard);
                            await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id);
                        }

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

                        await botClient.SendTextMessageAsync(chatId, "Хм, что то пошло не так\nЕсли у вас возникают проблемы, просто напишите боту о своей проблеме, снабжая вопрос надписью 'Помощь', и мы постараемся помочь вам.", parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown);

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

        private string AddHomework(int daysfromtoday)
        {
            DateTime now = DateTime.Now.Date;
            ResponseBulder response = new ResponseBulder("Telegram");
            Dz = true;
            if (daysfromtoday < 0)
            {
                Date = response.DateConverter(now.Subtract(new TimeSpan(-daysfromtoday, 0, 0, 0)));
            }
            else if (daysfromtoday == 0)
            {
                Date = response.DateConverter(now);
            }
            else if (daysfromtoday > 0)
            {
                Date = response.DateConverter(now.AddDays(daysfromtoday));
            }

            return "Введите текст домашнего задания и отправьте его как обычное сообщение";
        }


    }
}
