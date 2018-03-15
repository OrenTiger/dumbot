using System.Threading.Tasks;
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

        // GET /callback
        [HttpGet]
        public string Get()
        {
            return "ok";
        }

        /// <summary>
        /// POST: /callback
        /// Handle incoming event
        /// </summary>
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
                        return Ok(new { Value = _serverConfirmationReplyString });
                    else return BadRequest();
                case CallbackEventType.NewMessage:
                    string message = JObject.Parse(callbackEvent.Object.ToString())["body"].Value<string>();
                    int userId = JObject.Parse(callbackEvent.Object.ToString())["user_id"].Value<int>();

                    await _botService.HandleMessageAsync(message, userId);

                    return Ok("ok");
                default:
                    return Ok("ok");
            }
        }
    }
}
