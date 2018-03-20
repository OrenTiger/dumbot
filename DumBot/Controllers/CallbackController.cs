using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using DumBot.Models.Callback;
using DumBot.Services;
using Newtonsoft.Json;

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
                    if (callbackEvent.Group_id != _serverConfirmationGroupId)
                    {
                        _logger.LogWarning($"Callback server confirmation failed. Group ids are mismatch. GroupId: {callbackEvent.Group_id}");
                        return BadRequest();
                    }

                    return Ok(new { Value = _serverConfirmationReplyString });
                case CallbackEventType.NewMessage:

                    var message = JsonConvert.DeserializeObject<MessageModel>(callbackEvent.Object.ToString());

                    await _botService.HandleMessageAsync(message.Body, message.User_id);

                    return Ok("ok");
                default:
                    return Ok("ok");
            }
        }
    }
}
