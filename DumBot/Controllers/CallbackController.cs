using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using DumBot.Models;
using DumBot.Services;

namespace DumBot.Controllers
{
    [Route("[controller]")]
    public class CallbackController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IBotService _botService;
        private readonly ILogger<CallbackController> _logger;

        private readonly int _serverConfirmationGroupId;
        private readonly string _serverConfirmationReplyString;
        
        public CallbackController(IConfiguration configuration, IBotService botService,
            ILogger<CallbackController> logger)
        {
            _configuration = configuration;
            _botService = botService;
            _logger = logger;

            _serverConfirmationGroupId = int.Parse(_configuration["ServerConfirmationGroupId"]);
            _serverConfirmationReplyString = _configuration["ServerConfirmationReplyString"];
        }

        // GET api/values
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody]CallbackEventModel callbackEvent)
        {
            if (string.IsNullOrEmpty(callbackEvent.Type))
            {
                return BadRequest();
            }

            switch (callbackEvent.Type)
            {
                case CallbackEventType.Confirmation:
                    if (callbackEvent.Group_id == _serverConfirmationGroupId)
                    {
                        return Ok(new { Value = _serverConfirmationReplyString });
                    }
                    else
                    {
                        return BadRequest();
                    }
                case CallbackEventType.NewMessage:
                    string message = JObject.Parse(callbackEvent.Object.ToString())["body"].Value<string>();
                    int userId = JObject.Parse(callbackEvent.Object.ToString())["user_id"].Value<int>();

                    if (message.ToLowerInvariant().Contains(BotCommands.Hi.ToLowerInvariant()))
                    {
                        string userName = await _botService.GetUserNameAsync(userId);
                        _botService.SendMessageAsync(userId, $"{BotCommands.Hi}, {userName}");
                    }
                    else
                    {
                        _botService.SendMessageAsync(userId, "Test message");
                    }

                    return Ok("ok");
                default:
                    return Ok("ok");
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
