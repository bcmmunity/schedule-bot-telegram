using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TelegrammAspMvcDotNetCoreBot.DB;
using TelegrammAspMvcDotNetCoreBot.Logic;
using YandexDialogs;
using YandexDialogs.Request;
using YandexDialogs.Response;

namespace TelegrammAspMvcDotNetCoreBot.Controllers
{
    [Route("api/messagealice/update")]
    public class YandexController : ControllerBase
    {

        // GET: /<controller>/
        [HttpGet]
        public string Get()
        {
            return "Method GET unavailable"; ;
        }

        private int _messageId;
        private string _sessionId;
        private string _userId;

        [HttpPost]
        public IActionResult Callback([FromBody] dynamic response)
        {
            try
            {
                _messageId = response.session.message_id;
                _sessionId = response.session.session_id;
                _userId = response.session.user_id;

                SnUserDb userDb = new SnUserDb(_userId);

                if (!userDb.IsAliceUserInitialized())
                {
                    string id = String.Empty;
                    foreach (string token in response.request.nlu.tokens)
                    {
                         if (token == "помощь" || token == "умеешь" || token == "можешь")
                         {
                             return Ok(GetYandexJson(
                                 "Для использования бота сначала нужно авторизоваться, назвав айди, взятый из бота Вконтакте или телеграмм из вкладки О пользователе. После этого вам будут доступны такие команды как расписание на сегодня и завтра, а так же сброс пользователя командой \"Сбросить\"",
                                 true));
                         }
                         else if(!Int32.TryParse(token,out _))
                            continue;
                        id += token;
                    }

                    if (string.IsNullOrEmpty(id))
                    {
                        return Ok(GetYandexJson(
                            "Для того, чтобы начать работу с навыком необходимо авторизоваться. Для этого назовите ваш айди, взятый из бота Вконтакте или телеграмм из вкладки О пользователе",
                            false));
                    }

                    long chatId = Convert.ToInt32(id);

                    if (userDb.IsAliceUserExists(chatId))
                    {
                        userDb.CreateAliceUser(chatId);
                        return Ok(GetYandexJson(
                            "Пользователь привязан успешно!\nТеперь достаточно лишь сказать, какие пары сегодня или сколько пар завтра, и Алиса ответит на ваш вопрос",
                            false));
                    }
                    else
                    {
                        return Ok(GetYandexJson("Пользователь не был найден в базе данных. Пожалуйста, повторите",
                            false));
                    }
                }
                else
                {
                    foreach (string token in response.request.nlu.tokens)
                    {
                        if (token == "сегодня")
                        {
                            ResponseBuilder responseBuilder = new ResponseBuilder(_userId);
                            return Ok(GetYandexJson(responseBuilder.Today(userDb.GetAliceUserId(_userId), true), true));
                        }
                        else if (token == "завтра")
                        {
                            ResponseBuilder responseBuilder = new ResponseBuilder(_userId);
                            return Ok(GetYandexJson(responseBuilder.Tommorrow(userDb.GetAliceUserId(_userId), true), true));
                        }
                        else if (token.Contains("сброс"))
                        {
                            userDb.DeleteAliceUser(_userId);
                            return Ok(GetYandexJson("Пользователь успешно сброшен",true));
                        }
                        else if (token == "помощь" || token == "умеешь" || token == "можешь")
                        {
                            return Ok(GetYandexJson(
                                "Вы можете спросить расписание на сегодня и завтра, а так же сбросить пользователя командой \"Сбросить\"",
                                true));
                        }
                    }
                }

                return Ok(GetYandexJson("Извините, ваш запрос не распознан. Повторите пожалуйста или скажите \"помощь \"",false));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);

                
                    ErrorLoggingDB errorLoggingDb = new ErrorLoggingDB();

                    errorLoggingDb.AddErrorInLog(100, "YandexAPI", "Alice", e.Source + ": " + e.Message,
                        DateTime.Now);

                return Ok(GetYandexJson("К сожалению, что то пошло не так", true));
            }
        }

        private ToYandex GetYandexJson(string message, bool endSession)
        {
            return new ToYandex
            {
                Response = new Response
                {
                    Text = message,
                    End_session = endSession
                },
                Session = new YandexDialogs.Request.Session()
                {
                    Message_id = _messageId,
                    Session_id = _sessionId,
                    User_id = _userId
                },
                Version = "1.0"

            };
        }
    }
}