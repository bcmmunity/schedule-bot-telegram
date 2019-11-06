using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http2;
using Microsoft.Extensions.Configuration;
using Telegram.Bot.Types.Enums;
using TelegrammAspMvcDotNetCoreBot.DB;
using TelegrammAspMvcDotNetCoreBot.Models;
using TelegrammAspMvcDotNetCoreBot.Models.Telegramm;
using VkNet.Abstractions;
using VkNet.Model.RequestParams;

namespace TelegrammAspMvcDotNetCoreBot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SendingController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IVkApi _vkApi;

        public SendingController(IVkApi vkApi, IConfiguration configuration)
        {
            _vkApi = vkApi;
            _configuration = configuration;
        }
        [HttpGet]
        public ActionResult<string> Get()
        {
            SendingPost p = new SendingPost
            {
                SocialNetwork = "Telegram",
                University = "НИТУ МИСиС",
                Facility = "ИТАСУ",
                Course = "3",
                Group = "БИСТ-17-1 1 подгруппа",
                Message = "Проверка метода"
            };
            return Ok(p);
        }

        // POST api/values
        [HttpPost]
        public async Task<OkResult> Send([FromBody] SendingPost postResponse)
        {
            try
            {
                if (postResponse.Key != _configuration["Root:Key"])
                    return Ok();
                
                var botClient = await Bot.GetBotClientAsync();
               
                if (!string.IsNullOrEmpty(postResponse.SocialNetwork))
                {
                    SnUserDb db = new SnUserDb(postResponse.SocialNetwork);
                    List<SnUser> usersForSending = db.GetUsers(postResponse.University,
                        postResponse.Facility, postResponse.Course, postResponse.Group);

                    if (postResponse.SocialNetwork == "Telegram")
                        foreach (var snUser in usersForSending)
                        {
                           
                            await botClient.SendTextMessageAsync(snUser.SocialNetworkId,
                                postResponse.Message,
                                parseMode: ParseMode.Default);
                        }
                    else if (postResponse.SocialNetwork == "Vk")
                        foreach (var snUser in usersForSending)
                        {
                            _vkApi.Messages.Send(new MessagesSendParams
                            {
                                RandomId = new DateTime().Millisecond,
                                PeerId = snUser.SocialNetworkId,
                                Message = postResponse.Message
                            });
                        }

                }
                else
                {
                    SnUserDb db = new SnUserDb("Telegram");
                    List<SnUser> usersForSending = db.GetUsers(postResponse.University,
                        postResponse.Facility, postResponse.Course, postResponse.Group);
                    foreach (var snUser in usersForSending)
                    {
                        await botClient.SendTextMessageAsync(snUser.SocialNetworkId,
                            postResponse.Message,
                            parseMode: ParseMode.Default);
                    }

                    db = new SnUserDb("Vk");
                    usersForSending = db.GetUsers(postResponse.University,
                        postResponse.Facility, postResponse.Course, postResponse.Group);
                    foreach (var snUser in usersForSending)
                    {
                        _vkApi.Messages.Send(new MessagesSendParams
                        {
                            RandomId = new DateTime().Millisecond,
                            PeerId = snUser.SocialNetworkId,
                            Message = postResponse.Message
                        });
                    }
                }

                return Ok();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                ErrorLoggingDB errorLoggingDb = new ErrorLoggingDB();
                errorLoggingDb.AddErrorInLog(10101010, "Sending", "", e.Source + ": " + e.Message, DateTime.Now);
            }
            return Ok();
        }
    }
}