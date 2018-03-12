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
        public string Post([FromBody]CallbackEventModel callbackEvent)
        {
            if (string.IsNullOrEmpty(callbackEvent.Type))
            {
                HttpContext.Response.StatusCode = 400;
                return string.Empty;
            }

            switch (callbackEvent.Type)
            {
                case CallbackEventType.Confirmation:
                    return callbackEvent.Group_id == _serverConfirmationGroupId
                        ? _serverConfirmationReplyString
                        : string.Empty;
                case CallbackEventType.NewMessage:
                    int userId = JObject.Parse(callbackEvent.Object.ToString())["from_id"].Value<int>();
                    _botService.SendMessage(userId, "Test message");
                    return "ok";
                default:
                    return "ok";
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
